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

    // TODO: Delete this
    // [HttpGet("{id}")]
    // public async Task<IActionResult> GetScheduleById(Guid id)
    // {
    //     var result = await _scheduleService.GetScheduleByIdAsync(id);
    //     return StatusCode((int)result.StatusCode, result);
    // }
    // [HttpGet]
    // public async Task<IActionResult> GetAll()
    // {
    //     var result = await _scheduleService.GetAllAsync();
    //     return StatusCode((int)result.StatusCode, result);
    // }

    [HttpGet("settings")]
    public async Task<IActionResult> GetScheduleSettings([FromQuery] GetScheduleSettingsRequest request)
    {
        var currentMentorId = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var mentorId) ? mentorId : Guid.Empty;
        var result = await _scheduleService.GetScheduleSettingsAsync(currentMentorId, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateScheduleSettingsRequest request)
    {
        var result = await _scheduleService.UpdateAsync(id, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _scheduleService.DeleteAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }
}