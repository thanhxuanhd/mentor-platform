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

    [Authorize(Roles = "Mentor")]
    [HttpPost("mentor-submission")]
    public async Task<IActionResult> MentorSubmission([FromForm] MentorSubmissionRequest submission)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await mentorApplicationService.CreateMentorApplicationAsync(Guid.Parse(userId), submission, Request);

        return StatusCode((int)result.StatusCode, result);
    }

}
