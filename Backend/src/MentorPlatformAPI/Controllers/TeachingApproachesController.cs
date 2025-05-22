using Application.Services.TeachingApproaches;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeachingApproachesController(ITeachingApproachService teachingApproachService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllTeachingApproachsAsync()
    {
        var result = await teachingApproachService.GetAllTeachingApproachesAsync();

        return StatusCode((int)result.StatusCode, result);
    }
}