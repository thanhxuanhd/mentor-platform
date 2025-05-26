using Application.Services.Application;
using Contract.Dtos.MentorApplication.Requests;
using Microsoft.AspNetCore.Http;
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
}
