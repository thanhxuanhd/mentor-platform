using Application.Services.AdminDashboards;
using Infrastructure.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Authorize(Roles = RequiredRole.Admin)]
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
}