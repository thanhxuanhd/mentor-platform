using System.Security.Claims;
using Application.Services.MentorApplication;
using Contract.Dtos.MentorApplication.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Route("api/mentor-application")]
[ApiController]
public class MentorApplicationController(IMentorApplicationService mentorApplicationService) : ControllerBase
{
    // Mentor can also use this route to display all of their applications
    [Authorize(Roles = "Admin,Mentor")]
    [HttpGet("mentor-applications")]
    public async Task<IActionResult> GetAllMentorApplications([FromQuery] FilterMentorApplicationRequest request)
    {
        var result = await mentorApplicationService.GetAllMentorApplicationsAsync(request);
        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize(Roles = "Admin,Mentor")]
    [HttpGet("mentor-applications/{applicationId}")]
    public async Task<IActionResult> GetMentorApplicationById(Guid applicationId)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await mentorApplicationService.GetMentorApplicationByIdAsync(currentUserId, applicationId);
        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("mentor-applications/{applicationId}/request-info")]
    public async Task<IActionResult> RequestApplicationInfo(Guid applicationId, RequestApplicationInfoRequest request)
    {
        var result = await mentorApplicationService.RequestApplicationInfoAsync(applicationId, request);
        return StatusCode((int)result.StatusCode, result);
    }
}
