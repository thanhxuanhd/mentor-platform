using System.Security.Claims;
using Application.Services.MentorApplications;
using Contract.Dtos.MentorApplication.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Route("api/mentor-applications")]
[ApiController]
public class MentorApplicationController(IMentorApplicationService mentorApplicationService) : ControllerBase
{
    // Mentor can also use this route to display all of their applications
    [Authorize(Roles = "Admin,Mentor")]
    [HttpGet]
    public async Task<IActionResult> GetAllMentorApplications([FromQuery] FilterMentorApplicationRequest request)
    {
        var result = await mentorApplicationService.GetAllMentorApplicationsAsync(request);
        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize(Roles = "Admin,Mentor")]
    [HttpGet("{applicationId}")]
    public async Task<IActionResult> GetMentorApplicationById(Guid applicationId)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await mentorApplicationService.GetMentorApplicationByIdAsync(currentUserId, applicationId);
        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{applicationId}/request-info")]
    public async Task<IActionResult> RequestApplicationInfo(Guid applicationId, RequestApplicationInfoRequest request)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await mentorApplicationService.RequestApplicationInfoAsync(adminId, applicationId, request);
        return StatusCode((int)result.StatusCode, result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{applicationId}/status")]
    public async Task<IActionResult> UpdateApplicationStatus(Guid applicationId, UpdateApplicationStatusRequest request)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await mentorApplicationService.UpdateApplicationStatusAsync(adminId, applicationId, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize(Roles = "Mentor")]
    [HttpPut("{applicationId}")]
    public async Task<IActionResult> EditMentorApplication(Guid applicationId, [FromBody] UpdateMentorApplicationRequest request)
    {
        var result = await mentorApplicationService.EditMentorApplicationAsync(applicationId, request);
        return StatusCode((int)result.StatusCode, result);
    }
}
