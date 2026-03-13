using DPWH_HRIS.Domain.Entities;

namespace DPWH_HRIS.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string username, string password, string ipAddress);
    Task<AuthResult> RefreshTokenAsync(string refreshToken, string ipAddress);
    Task<bool> LogoutAsync(Guid userId);
    Task<User?> GetCurrentUserAsync(Guid userId);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
}

public record AuthResult(bool Success, string? Token, string? RefreshToken, string? Message, UserDto? User);

public record UserDto(Guid Id, string Username, string Email, string? RoleName, string? OfficeName, bool IsActive);

public interface IActiveDirectoryService
{
    /// <summary>Scaffold for DPWH Active Directory integration. To be implemented when AD credentials are provided.</summary>
    Task<bool> AuthenticateAsync(string username, string password);
    Task<ADUserInfo?> GetUserInfoAsync(string username);
}

public record ADUserInfo(string Username, string Email, string DisplayName, string? Department, string ObjectGuid);

public interface IEmailService
{
    /// <summary>Scaffold for SMTP/Exchange email integration. Configure SMTP settings in appsettings.json.</summary>
    Task SendAsync(string to, string subject, string body, bool isHtml = true);
    Task SendToMultipleAsync(IEnumerable<string> recipients, string subject, string body);
}

public interface IExportService
{
    Task<byte[]> ExportToPdfAsync<T>(IEnumerable<T> data, string title, IEnumerable<string> columns);
    Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, string? password = null);
    Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data);
    Task<byte[]> ExportToWordAsync(string content, string title);
}

public interface IWorkflowService
{
    Task<WorkflowTransaction> InitiateAsync(string transactionType, Guid requesterId, string entityType, string entityId, int maxLevel);
    Task<bool> ApproveAsync(Guid transactionId, Guid approverId, string? remarks);
    Task<bool> DisapproveAsync(Guid transactionId, Guid disapproverId, string? remarks);
    Task<bool> CancelAsync(Guid transactionId, Guid userId, string? remarks);
    Task<WorkflowTransaction?> GetStatusAsync(Guid transactionId);
}

public interface INotificationService
{
    Task CreateAsync(Guid userId, string title, string message, Domain.Enums.NotificationType type, string? entityType = null, string? entityId = null);
    Task<IEnumerable<Notification>> GetUnreadAsync(Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync(Guid userId);
}

public interface IAuditTrailService
{
    Task LogAsync(Guid? userId, string action, string module, string? entityName = null, string? entityId = null, string? oldValues = null, string? newValues = null, string? ipAddress = null, string? userAgent = null);
    Task<IEnumerable<AuditTrail>> GetLogsAsync(DateTime? from, DateTime? to, Guid? userId, string? module, int page, int pageSize);
}

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream fileStream, string fileName, string entityType, string entityId);
    Task<Stream?> GetAsync(string filePath);
    Task<bool> DeleteAsync(string filePath);
    Task<FileAttachment> SaveAttachmentAsync(Stream fileStream, string fileName, string fileType, long fileSize, string entityType, string entityId, Guid uploadedBy);
}

public interface IReportService
{
    /// <summary>Scaffold for SSRS/Crystal Reports integration. Configure SSRS server URL in appsettings.json.</summary>
    Task<byte[]> GenerateAsync(string reportName, IDictionary<string, object> parameters);
    Task<IEnumerable<string>> GetAvailableReportsAsync();
}

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryStatsAsync();
    Task<IEnumerable<AnnouncementDto>> GetAnnouncementsAsync(int count = 5);
    Task<IEnumerable<MemorandumDto>> GetMemorandumsAsync(int count = 5);
    Task<IEnumerable<DownloadableFormDto>> GetDownloadableFormsAsync();
}

public record DashboardSummaryDto(int PermanentCount, int ContractualCount, int CasualCount, int JobOrderCount, int TotalEmployees);
public record AnnouncementDto(Guid Id, string Title, string Content, string? ImagePath, DateTime? StartDate, DateTime? EndDate, string? PostedBy);
public record MemorandumDto(Guid Id, string Title, string? ReferenceNumber, DateTime DateIssued, string? FilePath);
public record DownloadableFormDto(Guid Id, string FormName, string? Description, string? Category, string FilePath, string? Version);

public interface IEmployeeService
{
    Task<PagedResult<EmployeeListDto>> GetPagedAsync(EmployeeFilter filter, int page, int pageSize);
    Task<EmployeeDetailDto?> GetByIdAsync(Guid id);
    Task<Employee> CreateAsync(CreateEmployeeDto dto);
    Task<Employee> UpdateAsync(Guid id, UpdateEmployeeDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<EmployeeListDto>> SearchAsync(string query);
}

public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public record EmployeeFilter(string? SearchTerm, Domain.Enums.EmploymentType? EmploymentType, Guid? OfficeId, Domain.Enums.EmploymentStatus? Status);
public record EmployeeListDto(Guid Id, string EmployeeNumber, string FullName, string? OfficeName, string? PositionTitle, string EmploymentType, string Status, string? PhotoPath);
public record EmployeeDetailDto(Guid Id, string EmployeeNumber, string FirstName, string? MiddleName, string LastName, string? Suffix, string Gender, DateTime DateOfBirth, string? PlaceOfBirth, string CivilStatus, string? Citizenship, string? BloodType, decimal? Height, decimal? Weight, string? ContactNumber, string? EmailAddress, string? ResidentialAddress, string? PermanentAddress, string? GSISNumber, string? PagIBIGNumber, string? PhilHealthNumber, string? SSSNumber, string? TINNumber, string EmploymentType, Guid? OfficeId, string? OfficeName, Guid? PositionId, string? PositionTitle, DateTime DateHired, string Status, string? PhotoPath);
public record CreateEmployeeDto(string EmployeeNumber, string FirstName, string? MiddleName, string LastName, string? Suffix, Domain.Enums.Gender Gender, DateTime DateOfBirth, string? PlaceOfBirth, Domain.Enums.CivilStatus CivilStatus, string? Citizenship, Domain.Enums.EmploymentType EmploymentType, Guid? OfficeId, Guid? PositionId, DateTime DateHired, string? ContactNumber, string? EmailAddress);
public record UpdateEmployeeDto(string FirstName, string? MiddleName, string LastName, string? Suffix, Domain.Enums.Gender Gender, DateTime DateOfBirth, string? PlaceOfBirth, Domain.Enums.CivilStatus CivilStatus, string? Citizenship, Domain.Enums.EmploymentType EmploymentType, Guid? OfficeId, Guid? PositionId, Domain.Enums.EmploymentStatus Status, string? ContactNumber, string? EmailAddress, string? ResidentialAddress, string? PermanentAddress);

public interface ILeaveService
{
    Task<PagedResult<LeaveApplicationDto>> GetApplicationsAsync(Guid? employeeId, Domain.Enums.ApprovalStatus? status, int page, int pageSize);
    Task<LeaveApplication> ApplyAsync(CreateLeaveApplicationDto dto);
    Task<bool> ApproveAsync(Guid applicationId, Guid approverId);
    Task<bool> DisapproveAsync(Guid applicationId, Guid approverId, string? reason);
    Task<LeaveBalanceSummaryDto> GetBalancesAsync(Guid employeeId, int year);
}

public record LeaveApplicationDto(Guid Id, string EmployeeFullName, string LeaveType, DateTime DateFrom, DateTime DateTo, decimal TotalDays, string Status, string? Reason, DateTime CreatedDate);
public record CreateLeaveApplicationDto(Guid EmployeeId, Domain.Enums.LeaveType LeaveType, DateTime DateFrom, DateTime DateTo, string? Reason);
public record LeaveBalanceSummaryDto(Guid EmployeeId, int Year, IEnumerable<LeaveBalanceItem> Balances);
public record LeaveBalanceItem(string LeaveType, decimal TotalCredits, decimal UsedCredits, decimal RemainingCredits);
