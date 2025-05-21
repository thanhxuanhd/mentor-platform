using Application.Services.Expertises;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExpertisesController(IExpertiseService expertiseService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllExpertisesAsync()
    {
        var result = await expertiseService.GetAllExpertisesAsync();

        return StatusCode((int)result.StatusCode, result);
    }
}