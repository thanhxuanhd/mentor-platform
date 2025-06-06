using Application.Services.MentorDashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Route("api/dashboards/mentor")]
[ApiController]
public class MentorDashboardController(IMentorDashboardService mentorDashboardService) : ControllerBase
{
    [HttpGet("{mentorId}")]
    public async Task<IActionResult> GetDashboard(Guid mentorId)
    {
        var result = await mentorDashboardService.GetMentorDashboardAsync(mentorId);
        return StatusCode((int)result.StatusCode, result);
    }
}
