using Application.Services.Authentication;
using Contract.Dtos.Authentication.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInUser([FromBody] SignInRequest request)
    {
        var result = await authService.LoginAsync(request);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUpUser([FromBody] SignUpRequest request)
    {
        await authService.RegisterAsync(request);

        return Created();
    }

    [Authorize("Admin")]
    [HttpGet("test")]
    public Task<IActionResult> TestAdmin()
    {
        return Task.FromResult<IActionResult>(Ok());
    }

    [HttpGet("ex")]
    public IActionResult Test()
    {
        throw new Exception("Test");
    }
}