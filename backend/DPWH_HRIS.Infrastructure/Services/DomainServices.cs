using Microsoft.EntityFrameworkCore;
using DPWH_HRIS.Application.Interfaces;
using DPWH_HRIS.Domain.Entities;
using DPWH_HRIS.Infrastructure.Data;

namespace DPWH_HRIS.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;

    public EmployeeService(AppDbContext context) => _context = context;

    public async Task<PagedResult<EmployeeListDto>> GetPagedAsync(EmployeeFilter filter, int page, int pageSize)
    {
        var query = _context.Employees
            .Include(e => e.Office)
            .Include(e => e.Position)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(e =>
                e.FirstName.ToLower().Contains(term) ||
                e.LastName.ToLower().Contains(term) ||
                e.EmployeeNumber.ToLower().Contains(term) ||
                (e.EmailAddress != null && e.EmailAddress.ToLower().Contains(term)));
        }

        if (filter.EmploymentType.HasValue)
            query = query.Where(e => e.EmploymentType == filter.EmploymentType.Value);

        if (filter.OfficeId.HasValue)
            query = query.Where(e => e.OfficeId == filter.OfficeId.Value);

        if (filter.Status.HasValue)
            query = query.Where(e => e.EmploymentStatus == filter.Status.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(e => e.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EmployeeListDto(
                e.Id,
                e.EmployeeNumber,
                $"{e.FirstName} {(e.MiddleName != null ? e.MiddleName[0] + ". " : "")}{e.LastName}{(e.Suffix != null ? " " + e.Suffix : "")}",
                e.Office != null ? e.Office.Name : null,
                e.Position != null ? e.Position.Title : null,
                e.EmploymentType.ToString(),
                e.EmploymentStatus.ToString(),
                e.PhotoPath))
            .ToListAsync();

        return new PagedResult<EmployeeListDto>(items, total, page, pageSize);
    }

    public async Task<EmployeeDetailDto?> GetByIdAsync(Guid id)
    {
        var e = await _context.Employees
            .Include(e => e.Office)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (e == null) return null;

        return new EmployeeDetailDto(
            e.Id, e.EmployeeNumber, e.FirstName, e.MiddleName, e.LastName, e.Suffix,
            e.Gender.ToString(), e.DateOfBirth, e.PlaceOfBirth, e.CivilStatus.ToString(),
            e.Citizenship, e.BloodType?.ToString(), e.Height, e.Weight,
            e.ContactNumber, e.EmailAddress, e.ResidentialAddress, e.PermanentAddress,
            e.GSISNumber, e.PagIBIGNumber, e.PhilHealthNumber, e.SSSNumber, e.TINNumber,
            e.EmploymentType.ToString(), e.OfficeId, e.Office?.Name, e.PositionId,
            e.Position?.Title, e.DateHired, e.EmploymentStatus.ToString(), e.PhotoPath);
    }

    public async Task<Employee> CreateAsync(CreateEmployeeDto dto)
    {
        var employee = new Employee
        {
            EmployeeNumber = dto.EmployeeNumber,
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName,
            LastName = dto.LastName,
            Suffix = dto.Suffix,
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            PlaceOfBirth = dto.PlaceOfBirth,
            CivilStatus = dto.CivilStatus,
            Citizenship = dto.Citizenship ?? "Filipino",
            EmploymentType = dto.EmploymentType,
            OfficeId = dto.OfficeId,
            PositionId = dto.PositionId,
            DateHired = dto.DateHired,
            ContactNumber = dto.ContactNumber,
            EmailAddress = dto.EmailAddress,
            EmploymentStatus = Domain.Enums.EmploymentStatus.Active,
            CreatedBy = "System",
            CreatedDate = DateTime.UtcNow
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateAsync(Guid id, UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees.FindAsync(id)
            ?? throw new KeyNotFoundException($"Employee {id} not found.");

        employee.FirstName = dto.FirstName;
        employee.MiddleName = dto.MiddleName;
        employee.LastName = dto.LastName;
        employee.Suffix = dto.Suffix;
        employee.Gender = dto.Gender;
        employee.DateOfBirth = dto.DateOfBirth;
        employee.PlaceOfBirth = dto.PlaceOfBirth;
        employee.CivilStatus = dto.CivilStatus;
        employee.Citizenship = dto.Citizenship;
        employee.EmploymentType = dto.EmploymentType;
        employee.OfficeId = dto.OfficeId;
        employee.PositionId = dto.PositionId;
        employee.EmploymentStatus = dto.Status;
        employee.ContactNumber = dto.ContactNumber;
        employee.EmailAddress = dto.EmailAddress;
        employee.ResidentialAddress = dto.ResidentialAddress;
        employee.PermanentAddress = dto.PermanentAddress;
        employee.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;
        employee.IsDeleted = true;
        employee.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<EmployeeListDto>> SearchAsync(string query)
    {
        var filter = new EmployeeFilter(query, null, null, null);
        var result = await GetPagedAsync(filter, 1, 20);
        return result.Items;
    }
}

public class LeaveService : ILeaveService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;

    public LeaveService(AppDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<PagedResult<LeaveApplicationDto>> GetApplicationsAsync(Guid? employeeId, Domain.Enums.ApprovalStatus? status, int page, int pageSize)
    {
        var query = _context.LeaveApplications.Include(l => l.Employee).AsQueryable();
        if (employeeId.HasValue) query = query.Where(l => l.EmployeeId == employeeId.Value);
        if (status.HasValue) query = query.Where(l => l.Status == status.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(l => l.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new LeaveApplicationDto(
                l.Id,
                l.Employee != null ? $"{l.Employee.FirstName} {l.Employee.LastName}" : "",
                l.LeaveType.ToString(),
                l.DateFrom,
                l.DateTo,
                l.TotalDays,
                l.Status.ToString(),
                l.Reason,
                l.CreatedDate))
            .ToListAsync();

        return new PagedResult<LeaveApplicationDto>(items, total, page, pageSize);
    }

    public async Task<LeaveApplication> ApplyAsync(CreateLeaveApplicationDto dto)
    {
        var application = new LeaveApplication
        {
            EmployeeId = dto.EmployeeId,
            LeaveType = dto.LeaveType,
            DateFrom = dto.DateFrom,
            DateTo = dto.DateTo,
            TotalDays = (decimal)(dto.DateTo - dto.DateFrom).TotalDays + 1,
            Reason = dto.Reason,
            Status = Domain.Enums.ApprovalStatus.Pending,
            CreatedBy = dto.EmployeeId.ToString(),
            CreatedDate = DateTime.UtcNow
        };

        await _context.LeaveApplications.AddAsync(application);
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<bool> ApproveAsync(Guid applicationId, Guid approverId)
    {
        var app = await _context.LeaveApplications.FindAsync(applicationId);
        if (app == null) return false;
        app.Status = Domain.Enums.ApprovalStatus.Approved;
        app.ApproverId = approverId;
        app.ApprovalDate = DateTime.UtcNow;
        app.ModifiedDate = DateTime.UtcNow;

        // Update leave balance
        var balance = await _context.LeaveBalances.FirstOrDefaultAsync(b => b.EmployeeId == app.EmployeeId && b.LeaveType == app.LeaveType && b.Year == DateTime.UtcNow.Year);
        if (balance != null)
        {
            balance.UsedCredits += app.TotalDays;
            balance.RemainingCredits = balance.TotalCredits - balance.UsedCredits;
        }

        await _context.SaveChangesAsync();

        // Notify employee
        var employee = await _context.Employees.FindAsync(app.EmployeeId);
        if (employee?.UserId != null)
        {
            await _notificationService.CreateAsync(employee.UserId.Value, "Leave Application Approved",
                $"Your {app.LeaveType} leave application has been approved.", Domain.Enums.NotificationType.LeaveStatus,
                "LeaveApplication", applicationId.ToString());
        }

        return true;
    }

    public async Task<bool> DisapproveAsync(Guid applicationId, Guid approverId, string? reason)
    {
        var app = await _context.LeaveApplications.FindAsync(applicationId);
        if (app == null) return false;
        app.Status = Domain.Enums.ApprovalStatus.Disapproved;
        app.ApproverId = approverId;
        app.ApprovalDate = DateTime.UtcNow;
        app.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<LeaveBalanceSummaryDto> GetBalancesAsync(Guid employeeId, int year)
    {
        var balances = await _context.LeaveBalances
            .Where(b => b.EmployeeId == employeeId && b.Year == year)
            .Select(b => new LeaveBalanceItem(b.LeaveType.ToString(), b.TotalCredits, b.UsedCredits, b.RemainingCredits))
            .ToListAsync();

        return new LeaveBalanceSummaryDto(employeeId, year, balances);
    }
}

public class WorkflowService : IWorkflowService
{
    private readonly AppDbContext _context;

    public WorkflowService(AppDbContext context) => _context = context;

    public async Task<WorkflowTransaction> InitiateAsync(string transactionType, Guid requesterId, string entityType, string entityId, int maxLevel)
    {
        var wf = new WorkflowTransaction
        {
            TransactionType = transactionType,
            RequesterId = requesterId,
            Status = Domain.Enums.ApprovalStatus.Pending,
            Level = 1,
            MaxLevel = maxLevel,
            EntityType = entityType,
            EntityId = entityId,
            CreatedBy = requesterId.ToString(),
            CreatedDate = DateTime.UtcNow
        };
        await _context.WorkflowTransactions.AddAsync(wf);
        await _context.SaveChangesAsync();
        return wf;
    }

    public async Task<bool> ApproveAsync(Guid transactionId, Guid approverId, string? remarks)
    {
        var wf = await _context.WorkflowTransactions.FindAsync(transactionId);
        if (wf == null) return false;

        if (wf.Level >= wf.MaxLevel)
            wf.Status = Domain.Enums.ApprovalStatus.Approved;
        else
            wf.Level++;

        wf.Remarks = remarks;
        wf.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DisapproveAsync(Guid transactionId, Guid disapproverId, string? remarks)
    {
        var wf = await _context.WorkflowTransactions.FindAsync(transactionId);
        if (wf == null) return false;
        wf.Status = Domain.Enums.ApprovalStatus.Disapproved;
        wf.Remarks = remarks;
        wf.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAsync(Guid transactionId, Guid userId, string? remarks)
    {
        var wf = await _context.WorkflowTransactions.FindAsync(transactionId);
        if (wf == null) return false;
        wf.Status = Domain.Enums.ApprovalStatus.Cancelled;
        wf.Remarks = remarks;
        wf.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<WorkflowTransaction?> GetStatusAsync(Guid transactionId)
        => await _context.WorkflowTransactions.FindAsync(transactionId);
}
