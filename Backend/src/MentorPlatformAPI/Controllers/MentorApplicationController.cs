using System.Security.Claims;
using Application.Services.MentorApplication;
using Contract.Dtos.MentorApplication.Requests;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Route("api/mentor-application")]
[ApiController]
public class MentorApplicationController(IMentorApplicationService mentorApplicationService) : ControllerBase
{
    [HttpGet("mentor-applications")]
    public async Task<IActionResult> GetAllMentorApplications([FromQuery] FilterMentorApplicationRequest request)
    {
        var result = await mentorApplicationService.GetAllMentorApplicationsAsync(request);
        return StatusCode((int)result.StatusCode, result);
    }

    //[Authorize(Roles = "Admin,Mentor")]
    [HttpGet("mentor-applications/{applicationId}")]
    public async Task<IActionResult> GetMentorApplicationById(Guid applicationId)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await mentorApplicationService.GetMentorApplicationByIdAsync(currentUserId, applicationId);
        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize(Roles = "Admin,Mentor")]
    [HttpPut("mentor-applications/{applicationId}")]
    public async Task<IActionResult> EditMentorApplication(Guid applicationId, [FromBody] UpdateMentorApplicationRequest request)
    {
        var result = await mentorApplicationService.EditMentorApplicationAsync(applicationId, request);
        return StatusCode((int)result.StatusCode, result);
    }
}
