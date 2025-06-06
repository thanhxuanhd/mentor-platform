using Application.Services.AdminDashboards;
using Infrastructure.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Route("api/dashboards/admin")]
[ApiController]
public class AdminDashboardController(IAdminDashboardService adminDashboardService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAdminDashboard()
    {
        var result = await adminDashboardService.GetAdminDashboardAsync();

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("/report/monthly-mentor-application")]
    public async Task<IActionResult> GetMentorApplicationReport()
    {
        var currentYear = DateTime.Now.Year;
        var resultBytes = await adminDashboardService.GetMentorApplicationReportCurrentYearAsync();

        return File(resultBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{currentYear}_MentorApplicationReport");
    }

    [HttpPost("/report/mentor-activity")]
    public async Task<IActionResult> GetMentorActivityReport()
    {
        var resultBytes = await adminDashboardService.GetMentorActivityReportAsync();

        return File(resultBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MentorActivityReport");
    }
}