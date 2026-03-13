using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DPWH_HRIS.Application.Interfaces;
using DPWH_HRIS.Domain.Enums;

namespace DPWH_HRIS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeaveController(ILeaveService leaveService) => _leaveService = leaveService;

    /// <summary>Get leave applications with optional filters.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? employeeId,
        [FromQuery] ApprovalStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _leaveService.GetApplicationsAsync(employeeId, status, page, pageSize);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Apply for leave.</summary>
    [HttpPost]
    public async Task<IActionResult> Apply([FromBody] CreateLeaveApplicationDto dto)
    {
        var application = await _leaveService.ApplyAsync(dto);
        return CreatedAtAction(nameof(GetAll), new { employeeId = dto.EmployeeId }, new { success = true, data = application });
    }

    /// <summary>Approve a leave application.</summary>
    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "System Administrator,HR Administrator,HR Staff,Department Head/Supervisor")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var approverId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _leaveService.ApproveAsync(id, approverId);
        return result ? Ok(new { success = true }) : NotFound();
    }

    /// <summary>Disapprove a leave application.</summary>
    [HttpPost("{id:guid}/disapprove")]
    [Authorize(Roles = "System Administrator,HR Administrator,HR Staff,Department Head/Supervisor")]
    public async Task<IActionResult> Disapprove(Guid id, [FromBody] DisapproveRequest request)
    {
        var approverId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _leaveService.DisapproveAsync(id, approverId, request.Reason);
        return result ? Ok(new { success = true }) : NotFound();
    }

    /// <summary>Get leave balances for an employee.</summary>
    [HttpGet("balance/{employeeId:guid}")]
    public async Task<IActionResult> GetBalance(Guid employeeId, [FromQuery] int? year)
    {
        var result = await _leaveService.GetBalancesAsync(employeeId, year ?? DateTime.Now.Year);
        return Ok(new { success = true, data = result });
    }
}

public record DisapproveRequest(string? Reason);
