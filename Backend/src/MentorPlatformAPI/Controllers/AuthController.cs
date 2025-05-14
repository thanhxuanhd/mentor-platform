using Application.Services.Authentication;
using Contract.Dtos.Authentication.Requests;
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

    [HttpPost("github")]
    public async Task<IActionResult> SignInGithub([FromBody] string code)
    {
        var result = await authService.LoginGithubAsync(code);

        return Ok(result);
    }

    [HttpPost("google")]
    public async Task<IActionResult> SignInGoogle([FromBody] string code)
    {
        var result = await authService.LoginGoogleAsync(code);

        return Ok(result);
    }
}