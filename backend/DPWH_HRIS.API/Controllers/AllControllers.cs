using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DPWH_HRIS.Application.Interfaces;

namespace DPWH_HRIS.API.Controllers;

/// <summary>Notification management — in-app notifications for users.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService) => _notificationService = notificationService;

    [HttpGet("unread")]
    public async Task<IActionResult> GetUnread()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var notifications = await _notificationService.GetUnreadAsync(userId);
        return Ok(new { success = true, data = notifications });
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(new { success = true, data = count });
    }

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return Ok(new { success = true });
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _notificationService.MarkAllAsReadAsync(userId);
        return Ok(new { success = true });
    }
}

/// <summary>Audit trail — system activity logs.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "System Administrator,HR Administrator")]
public class AuditTrailController : ControllerBase
{
    private readonly IAuditTrailService _auditService;

    public AuditTrailController(IAuditTrailService auditService) => _auditService = auditService;

    [HttpGet]
    public async Task<IActionResult> GetLogs(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? userId,
        [FromQuery] string? module,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var logs = await _auditService.GetLogsAsync(from, to, userId, module, page, pageSize);
        return Ok(new { success = true, data = logs });
    }
}

/// <summary>Export controller — PDF, Excel, CSV, Word export endpoints.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;

    public ExportController(IExportService exportService) => _exportService = exportService;

    [HttpPost("pdf")]
    public async Task<IActionResult> ExportPdf([FromBody] ExportRequest request)
    {
        var data = request.Data.Select(d => d).ToList();
        var bytes = await _exportService.ExportToPdfAsync(data, request.Title, request.Columns);
        return File(bytes, "application/pdf", $"{request.Title}_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpPost("excel")]
    public async Task<IActionResult> ExportExcel([FromBody] ExportRequest request)
    {
        var bytes = await _exportService.ExportToExcelAsync(request.Data, request.Title, request.Password);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{request.Title}_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    [HttpPost("csv")]
    public async Task<IActionResult> ExportCsv([FromBody] ExportRequest request)
    {
        var bytes = await _exportService.ExportToCsvAsync(request.Data);
        return File(bytes, "text/csv", $"{request.Title}_{DateTime.Now:yyyyMMdd}.csv");
    }
}

public record ExportRequest(string Title, IEnumerable<string> Columns, IEnumerable<Dictionary<string, object>> Data, string? Password = null);

/// <summary>Report controller — scaffold for SSRS/Crystal Reports integration.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService) => _reportService = reportService;

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableReports()
    {
        var reports = await _reportService.GetAvailableReportsAsync();
        return Ok(new { success = true, data = reports });
    }

    [HttpPost("generate/{reportName}")]
    public async Task<IActionResult> Generate(string reportName, [FromBody] Dictionary<string, object>? parameters)
    {
        var bytes = await _reportService.GenerateAsync(reportName, parameters ?? new Dictionary<string, object>());
        return File(bytes, "application/pdf", $"{reportName}_{DateTime.Now:yyyyMMdd}.pdf");
    }
}

/// <summary>File upload controller — handles document uploads.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FileUploadController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    public FileUploadController(IFileStorageService fileStorageService) => _fileStorageService = fileStorageService;

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromQuery] string entityType, [FromQuery] string entityId)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { success = false, message = "No file provided." });

        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var ext = Path.GetExtension(file.FileName).ToLower();
        var allowed = new[] { ".pdf", ".docx", ".xlsx", ".csv", ".jpg", ".jpeg", ".png" };
        if (!allowed.Contains(ext))
            return BadRequest(new { success = false, message = "File type not allowed." });

        await using var stream = file.OpenReadStream();
        var attachment = await _fileStorageService.SaveAttachmentAsync(
            stream, file.FileName, file.ContentType, file.Length, entityType, entityId, userId);

        return Ok(new { success = true, data = attachment });
    }
}

/// <summary>Workflow controller — multi-level approval engine.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkflowController : ControllerBase
{
    private readonly IWorkflowService _workflowService;

    public WorkflowController(IWorkflowService workflowService) => _workflowService = workflowService;

    [HttpPost("initiate")]
    public async Task<IActionResult> Initiate([FromBody] InitiateWorkflowRequest request)
    {
        var requesterId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var wf = await _workflowService.InitiateAsync(request.TransactionType, requesterId, request.EntityType, request.EntityId, request.MaxLevel);
        return Ok(new { success = true, data = wf });
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] WorkflowActionRequest request)
    {
        var approverId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _workflowService.ApproveAsync(id, approverId, request.Remarks);
        return result ? Ok(new { success = true }) : NotFound();
    }

    [HttpPost("{id:guid}/disapprove")]
    public async Task<IActionResult> Disapprove(Guid id, [FromBody] WorkflowActionRequest request)
    {
        var approverId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _workflowService.DisapproveAsync(id, approverId, request.Remarks);
        return result ? Ok(new { success = true }) : NotFound();
    }

    [HttpGet("{id:guid}/status")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        var status = await _workflowService.GetStatusAsync(id);
        return status != null ? Ok(new { success = true, data = status }) : NotFound();
    }
}

public record InitiateWorkflowRequest(string TransactionType, string EntityType, string EntityId, int MaxLevel);
public record WorkflowActionRequest(string? Remarks);

/// <summary>Personnel Info controller — PDS, employee directory, org chart.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PersonnelInfoController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public PersonnelInfoController(IEmployeeService employeeService) => _employeeService = employeeService;

    [HttpGet("directory")]
    public async Task<IActionResult> GetDirectory([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var filter = new EmployeeFilter(search, null, null, Domain.Enums.EmploymentStatus.Active);
        var result = await _employeeService.GetPagedAsync(filter, page, pageSize);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("{id:guid}/pds")]
    public async Task<IActionResult> GetPDS(Guid id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        return employee != null ? Ok(new { success = true, data = employee }) : NotFound();
    }
}

/// <summary>Payroll controller — payroll processing, deductions, contributions.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "System Administrator,HR Administrator,HR Staff")]
public class PayrollController : ControllerBase
{
    [HttpGet]
    public IActionResult GetPayrollRecords([FromQuery] string? period, [FromQuery] int page = 1)
        => Ok(new { success = true, message = "Payroll processing module. See DPWH Payroll System integration guide.", data = new List<object>() });

    [HttpGet("{id:guid}/payslip")]
    [AllowAnonymous]
    public IActionResult GetPayslip(Guid id)
        => Ok(new { success = true, message = "Payslip generation requires payroll data.", data = new { employeeId = id } });
}

/// <summary>Attendance controller — DTR management.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    [HttpGet]
    public IActionResult GetDTR([FromQuery] Guid? employeeId, [FromQuery] DateTime? month)
        => Ok(new { success = true, message = "DTR module. Integrates with biometric devices.", data = new List<object>() });
}

/// <summary>Recruitment controller — job postings, applicants, onboarding.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecruitmentController : ControllerBase
{
    [HttpGet("job-postings")]
    public IActionResult GetJobPostings()
        => Ok(new { success = true, data = new List<object>() });

    [HttpGet("applicants")]
    public IActionResult GetApplicants([FromQuery] Guid? jobPostingId)
        => Ok(new { success = true, data = new List<object>() });
}

/// <summary>Performance controller — IPCR, DPCR, OPCR, coaching journals.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PerformanceController : ControllerBase
{
    [HttpGet]
    public IActionResult GetReviews([FromQuery] Guid? employeeId, [FromQuery] string? reviewType)
        => Ok(new { success = true, data = new List<object>() });
}

/// <summary>Learning & Development controller — training, scholarships, e-learning.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LearningDevController : ControllerBase
{
    [HttpGet("courses")]
    public IActionResult GetCourses()
        => Ok(new { success = true, data = new List<object>() });

    [HttpGet("scholarships")]
    public IActionResult GetScholarships()
        => Ok(new { success = true, data = new List<object>() });
}

/// <summary>Offboarding controller — separation, exit interviews, clearance.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OffboardingController : ControllerBase
{
    [HttpGet]
    public IActionResult GetSeparations()
        => Ok(new { success = true, data = new List<object>() });
}

/// <summary>User management controller — user CRUD, roles, permissions.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "System Administrator")]
public class UserManagementController : ControllerBase
{
    [HttpGet("users")]
    public IActionResult GetUsers()
        => Ok(new { success = true, data = new List<object>() });

    [HttpGet("roles")]
    public IActionResult GetRoles()
        => Ok(new { success = true, data = new List<object>() });
}

/// <summary>Data library controller — standard HR data libraries.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DataLibraryController : ControllerBase
{
    [HttpGet("{category}")]
    public IActionResult GetByCategory(string category)
        => Ok(new { success = true, data = new List<object>() });
}
