using Application.Services.Availabilities;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AvailabilitiesController(IAvailabilityService availabilityService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAvailabilitiesAsync()
    {
        var result = await availabilityService.GetAllAvailabilitiesAsync();

        return StatusCode((int)result.StatusCode, result);
    }
}