using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DPWH_HRIS.Application.Interfaces;
using DPWH_HRIS.Domain.Enums;

namespace DPWH_HRIS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IAuditTrailService _auditService;

    public EmployeeController(IEmployeeService employeeService, IAuditTrailService auditService)
    {
        _employeeService = employeeService;
        _auditService = auditService;
    }

    /// <summary>Get paginated, filtered, sorted list of employees.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] EmploymentType? employmentType,
        [FromQuery] Guid? officeId,
        [FromQuery] EmploymentStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var filter = new EmployeeFilter(search, employmentType, officeId, status);
        var result = await _employeeService.GetPagedAsync(filter, page, pageSize);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Get employee details by ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _employeeService.GetByIdAsync(id);
        return result != null ? Ok(new { success = true, data = result }) : NotFound(new { success = false, message = "Employee not found." });
    }

    /// <summary>Search employees by name or employee number.</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var results = await _employeeService.SearchAsync(q);
        return Ok(new { success = true, data = results });
    }

    /// <summary>Create a new employee record.</summary>
    [HttpPost]
    [Authorize(Roles = "System Administrator,HR Administrator,HR Staff")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var employee = await _employeeService.CreateAsync(dto);
        await _auditService.LogAsync(
            Guid.TryParse(userId, out var uid) ? uid : null,
            "CREATE", "Employee", "Employee", employee.Id.ToString(),
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, new { success = true, data = employee });
    }

    /// <summary>Update an existing employee record.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "System Administrator,HR Administrator,HR Staff")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var employee = await _employeeService.UpdateAsync(id, dto);
        await _auditService.LogAsync(
            Guid.TryParse(userId, out var uid) ? uid : null,
            "UPDATE", "Employee", "Employee", id.ToString(),
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
        return Ok(new { success = true, data = employee });
    }

    /// <summary>Soft-delete an employee record.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "System Administrator,HR Administrator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _employeeService.DeleteAsync(id);
        return result ? Ok(new { success = true }) : NotFound(new { success = false });
    }
}
