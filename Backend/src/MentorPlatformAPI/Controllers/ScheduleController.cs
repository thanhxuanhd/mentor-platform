using Contract.Dtos.Schedule.Requests;
using Application.Services.Schedule;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MentorPlatformAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;


    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

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
    public async Task<IActionResult> GetScheduleSettings(GetScheduleSettingsRequest request)
    {
        var currentMentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        request.MentorId = Guid.TryParse(currentMentorId, out var mentorId) ? mentorId : Guid.Empty;
        var result = await _scheduleService.GetScheduleSettingsAsync(request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScheduleSettings request)
    {
        var result = await _scheduleService.CreateAsync(request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateScheduleSettings request)
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