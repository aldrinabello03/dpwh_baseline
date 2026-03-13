using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DPWH_HRIS.Application.Interfaces;
using DPWH_HRIS.Domain.Entities;
using DPWH_HRIS.Infrastructure.Data;
using BCrypt.Net;

namespace DPWH_HRIS.Infrastructure.Services;

public class JwtAuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly IAuditTrailService _auditService;

    public JwtAuthService(AppDbContext context, IConfiguration config, IAuditTrailService auditService)
    {
        _context = context;
        _config = config;
        _auditService = auditService;
    }

    public async Task<AuthResult> LoginAsync(string username, string password, string ipAddress)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Office)
            .FirstOrDefaultAsync(u => (u.Username == username || u.Email == username) && u.IsActive);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            await _auditService.LogAsync(null, "LOGIN_FAILED", "Auth", ipAddress: ipAddress);
            return new AuthResult(false, null, null, "Invalid username or password.", null);
        }

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.LastLoginDate = DateTime.UtcNow;
        user.LastLoginIP = ipAddress;
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync();
        await _auditService.LogAsync(user.Id, "LOGIN_SUCCESS", "Auth", ipAddress: ipAddress);

        return new AuthResult(true, token, refreshToken, "Login successful.", new UserDto(user.Id, user.Username, user.Email, user.Role?.Name, user.Office?.Name, user.IsActive));
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken, string ipAddress)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Office)
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);

        if (user == null)
            return new AuthResult(false, null, null, "Invalid or expired refresh token.", null);

        var token = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new AuthResult(true, token, newRefreshToken, "Token refreshed.", new UserDto(user.Id, user.Username, user.Email, user.Role?.Name, user.Office?.Name, user.IsActive));
    }

    public async Task<bool> LogoutAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await _context.SaveChangesAsync();
        await _auditService.LogAsync(userId, "LOGOUT", "Auth");
        return true;
    }

    public async Task<User?> GetCurrentUserAsync(Guid userId)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Office)
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.ModifiedBy = userId.ToString();
        user.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "DefaultKey-DPWH-HRIS-2026-SuperSecret!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "Employee"),
            new Claim("officeId", user.OfficeId?.ToString() ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "DPWH_HRIS",
            audience: _config["Jwt:Audience"] ?? "DPWH_HRIS_Users",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

public class AuditTrailService : IAuditTrailService
{
    private readonly AppDbContext _context;

    public AuditTrailService(AppDbContext context) => _context = context;

    public async Task LogAsync(Guid? userId, string action, string module, string? entityName = null, string? entityId = null, string? oldValues = null, string? newValues = null, string? ipAddress = null, string? userAgent = null)
    {
        await _context.AuditTrails.AddAsync(new AuditTrail
        {
            UserId = userId,
            Action = action,
            Module = module,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IPAddress = ipAddress,
            UserAgent = userAgent,
            Timestamp = DateTime.UtcNow,
            CreatedBy = userId?.ToString(),
            CreatedDate = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditTrail>> GetLogsAsync(DateTime? from, DateTime? to, Guid? userId, string? module, int page, int pageSize)
    {
        var query = _context.AuditTrails.AsQueryable();
        if (from.HasValue) query = query.Where(a => a.Timestamp >= from.Value);
        if (to.HasValue) query = query.Where(a => a.Timestamp <= to.Value);
        if (userId.HasValue) query = query.Where(a => a.UserId == userId.Value);
        if (!string.IsNullOrEmpty(module)) query = query.Where(a => a.Module == module);

        return await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context) => _context = context;

    public async Task CreateAsync(Guid userId, string title, string message, Domain.Enums.NotificationType type, string? entityType = null, string? entityId = null)
    {
        await _context.Notifications.AddAsync(new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            NotificationType = type,
            IsRead = false,
            RelatedEntityType = entityType,
            RelatedEntityId = entityId,
            CreatedBy = userId.ToString(),
            CreatedDate = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notification>> GetUnreadAsync(Guid userId)
        => await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).OrderByDescending(n => n.CreatedDate).Take(20).ToListAsync();

    public async Task<int> GetUnreadCountAsync(Guid userId)
        => await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var n = await _context.Notifications.FindAsync(notificationId);
        if (n != null) { n.IsRead = true; await _context.SaveChangesAsync(); }
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
    }
}

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context) => _context = context;

    public async Task<DashboardSummaryDto> GetSummaryStatsAsync()
    {
        var employees = await _context.Employees.Where(e => !e.IsDeleted && e.EmploymentStatus == Domain.Enums.EmploymentStatus.Active).ToListAsync();
        return new DashboardSummaryDto(
            employees.Count(e => e.EmploymentType == Domain.Enums.EmploymentType.Permanent),
            employees.Count(e => e.EmploymentType == Domain.Enums.EmploymentType.Contractual),
            employees.Count(e => e.EmploymentType == Domain.Enums.EmploymentType.Casual),
            employees.Count(e => e.EmploymentType == Domain.Enums.EmploymentType.JobOrder),
            employees.Count
        );
    }

    public async Task<IEnumerable<AnnouncementDto>> GetAnnouncementsAsync(int count = 5)
    {
        var now = DateTime.UtcNow;
        return await _context.Announcements
            .Include(a => a.PostedByUser)
            .Where(a => a.IsActive && !a.IsDeleted && (a.StartDate == null || a.StartDate <= now) && (a.EndDate == null || a.EndDate >= now))
            .OrderByDescending(a => a.CreatedDate)
            .Take(count)
            .Select(a => new AnnouncementDto(a.Id, a.Title, a.Content, a.ImagePath, a.StartDate, a.EndDate, a.PostedByUser != null ? a.PostedByUser.Username : null))
            .ToListAsync();
    }

    public async Task<IEnumerable<MemorandumDto>> GetMemorandumsAsync(int count = 5)
    {
        return await _context.Memorandums
            .Where(m => !m.IsDeleted)
            .OrderByDescending(m => m.DateIssued)
            .Take(count)
            .Select(m => new MemorandumDto(m.Id, m.Title, m.ReferenceNumber, m.DateIssued, m.FilePath))
            .ToListAsync();
    }

    public async Task<IEnumerable<DownloadableFormDto>> GetDownloadableFormsAsync()
    {
        return await _context.DownloadableForms
            .Where(f => f.IsActive && !f.IsDeleted)
            .OrderBy(f => f.Category).ThenBy(f => f.FormName)
            .Select(f => new DownloadableFormDto(f.Id, f.FormName, f.Description, f.Category, f.FilePath, f.Version))
            .ToListAsync();
    }
}

public class EmailService : IEmailService
{
    public Task SendAsync(string to, string subject, string body, bool isHtml = true)
    {
        // TODO: Configure SMTP settings in appsettings.json under "Email" section
        // Example: Host, Port, Username, Password, FromAddress
        // Use MailKit or System.Net.Mail to send emails
        throw new NotImplementedException("Email service requires SMTP configuration. See appsettings.json Email section.");
    }

    public Task SendToMultipleAsync(IEnumerable<string> recipients, string subject, string body)
        => SendAsync(string.Join(",", recipients), subject, body);
}

public class ActiveDirectoryService : IActiveDirectoryService
{
    public Task<bool> AuthenticateAsync(string username, string password)
    {
        // TODO: Configure LDAP/Active Directory settings in appsettings.json
        // Use System.DirectoryServices.Protocols for AD authentication
        throw new NotImplementedException("Active Directory service requires AD configuration.");
    }

    public Task<ADUserInfo?> GetUserInfoAsync(string username)
    {
        throw new NotImplementedException("Active Directory service requires AD configuration.");
    }
}

public class FileStorageService : IFileStorageService
{
    private readonly AppDbContext _context;
    private readonly string _storageRoot;

    public FileStorageService(AppDbContext context, Microsoft.Extensions.Configuration.IConfiguration config)
    {
        _context = context;
        _storageRoot = config["FileStorage:RootPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(_storageRoot);
    }

    public async Task<string> SaveAsync(Stream fileStream, string fileName, string entityType, string entityId)
    {
        var dir = Path.Combine(_storageRoot, entityType, entityId);
        Directory.CreateDirectory(dir);
        var safeName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var fullPath = Path.Combine(dir, safeName);
        using var fs = File.Create(fullPath);
        await fileStream.CopyToAsync(fs);
        return fullPath;
    }

    public async Task<Stream?> GetAsync(string filePath)
    {
        if (!File.Exists(filePath)) return null;
        return await Task.FromResult<Stream>(File.OpenRead(filePath));
    }

    public async Task<bool> DeleteAsync(string filePath)
    {
        if (File.Exists(filePath)) { File.Delete(filePath); }
        return await Task.FromResult(true);
    }

    public async Task<FileAttachment> SaveAttachmentAsync(Stream fileStream, string fileName, string fileType, long fileSize, string entityType, string entityId, Guid uploadedBy)
    {
        var filePath = await SaveAsync(fileStream, fileName, entityType, entityId);
        var attachment = new FileAttachment
        {
            FileName = fileName,
            FilePath = filePath,
            FileType = fileType,
            FileSize = fileSize,
            EntityType = entityType,
            EntityId = entityId,
            UploadedBy = uploadedBy,
            UploadedDate = DateTime.UtcNow,
            CreatedBy = uploadedBy.ToString(),
            CreatedDate = DateTime.UtcNow
        };
        await _context.FileAttachments.AddAsync(attachment);
        await _context.SaveChangesAsync();
        return attachment;
    }
}

public class ReportService : IReportService
{
    public Task<byte[]> GenerateAsync(string reportName, IDictionary<string, object> parameters)
    {
        // TODO: Configure SSRS server URL in appsettings.json under "SSRS" section
        throw new NotImplementedException("Report service requires SSRS/Crystal Reports configuration.");
    }

    public Task<IEnumerable<string>> GetAvailableReportsAsync()
        => Task.FromResult<IEnumerable<string>>(new[] { "EmployeeMasterlist", "AttendanceSummary", "LeaveReport", "PayrollReport", "PerformanceSummary" });
}
