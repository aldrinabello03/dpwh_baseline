using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DPWH_HRIS.Application.Interfaces;

namespace DPWH_HRIS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService) => _dashboardService = dashboardService;

    /// <summary>Get summary statistics for the dashboard (employee counts by type).</summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummaryStats()
    {
        var stats = await _dashboardService.GetSummaryStatsAsync();
        return Ok(new { success = true, data = stats });
    }

    /// <summary>Get active announcements for the dashboard.</summary>
    [HttpGet("announcements")]
    public async Task<IActionResult> GetAnnouncements([FromQuery] int count = 5)
    {
        var data = await _dashboardService.GetAnnouncementsAsync(count);
        return Ok(new { success = true, data });
    }

    /// <summary>Get latest memorandums.</summary>
    [HttpGet("memorandums")]
    public async Task<IActionResult> GetMemorandums([FromQuery] int count = 5)
    {
        var data = await _dashboardService.GetMemorandumsAsync(count);
        return Ok(new { success = true, data });
    }

    /// <summary>Get list of downloadable forms.</summary>
    [HttpGet("forms")]
    public async Task<IActionResult> GetDownloadableForms()
    {
        var data = await _dashboardService.GetDownloadableFormsAsync();
        return Ok(new { success = true, data });
    }
}
