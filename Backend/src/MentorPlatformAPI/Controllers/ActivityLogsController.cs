using Application.Services.ActivityLogs;
using Contract.Dtos.ActivityLogs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Route("api/activity-logs")]
[ApiController]
public class ActivityLogsController(IActivityLogService activityLogService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPaginatedActivityLogsAsync([FromQuery] GetActivityLogRequest request)
    {
        var result = await activityLogService.GetPaginatedActivityLogs(request);

        return StatusCode((int)result.StatusCode, result);
    }
}