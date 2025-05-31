using Application.Services.MentorApplications;
using Contract.Dtos.MentorApplications.Requests;
using Contract.Dtos.Users.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await mentorApplicationService.GetMentorApplicationByIdAsync(currentUserId, applicationId);

        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize(Roles = "Mentor")]
    [HttpPost]
    public async Task<IActionResult> MentorSubmission([FromForm] MentorSubmissionRequest submission)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await mentorApplicationService.CreateMentorApplicationAsync(Guid.Parse(userId), submission, Request);

        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize(Roles = "Mentor")]
    [HttpGet("{mentorId}/mentor")]
    public async Task<IActionResult> GetMentorApplicationByMentorId(Guid mentorId)
    {
        var result = await mentorApplicationService.GetListMentorApplicationByMentorIdAsync(mentorId);

        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{applicationId}/request-info")]
    public async Task<IActionResult> RequestApplicationInfo(Guid applicationId, RequestApplicationInfoRequest request)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await mentorApplicationService.RequestApplicationInfoAsync(adminId, applicationId, request);

        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{applicationId}/status")]
    public async Task<IActionResult> UpdateApplicationStatus(Guid applicationId, UpdateApplicationStatusRequest request)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await mentorApplicationService.UpdateApplicationStatusAsync(adminId, applicationId, request);

        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize(Roles = "Mentor")]
    [HttpPut("{applicationId}")]
    public async Task<IActionResult> EditMentorApplication(Guid applicationId, [FromForm] UpdateMentorApplicationRequest request)
    {
        var result = await mentorApplicationService.EditMentorApplicationAsync(applicationId, request, Request);

        return StatusCode((int)result.StatusCode, result);
    }
}
