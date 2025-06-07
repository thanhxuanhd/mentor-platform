using Contract.Dtos.Schedule.Requests;
using Application.Services.Schedule;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MentorPlatformAPI.Controllers;
[Authorize(Roles = "Mentor")]
[Route("api/schedules")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet("{mentorId}/settings")]
    public async Task<IActionResult> GetScheduleSettings(Guid mentorId, [FromQuery] GetScheduleSettingsRequest request)
    {
        var result = await _scheduleService.GetScheduleSettingsAsync(mentorId, request);
        return StatusCode((int)result.StatusCode, result);
    }
    
    [HttpPost("{mentorId}/settings")]
    public async Task<IActionResult> UpdateScheduleSettings(Guid mentorId, [FromBody] SaveScheduleSettingsRequest request)
    {
        var userIdString = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || mentorId != Guid.Parse(userIdString))
        {
            return Forbid("You are not allow to update this mentor's schedule settings.");
        }

        var result = await _scheduleService.SaveScheduleSettingsAsync(mentorId, request);
        return StatusCode((int)result.StatusCode, result);
    }
}