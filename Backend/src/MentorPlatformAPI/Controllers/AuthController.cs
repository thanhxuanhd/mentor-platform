using Application.Services.Authentication;
using Contract.Dtos.Authentication.Requests;
using Contract.Shared;
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
    public async Task<IActionResult> SignInGithub([FromBody] OAuthSignInRequest request)
    {
        var result = await authService.LoginGithubAsync(request);

        return Ok(result);
    }

    [HttpPost("google")]
    public async Task<IActionResult> SignInGoogle([FromBody] OAuthSignInRequest request)
    {
        var result = await authService.LoginGoogleAsync(request);

        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await authService.ResetPasswordAsync(request);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("check-email")]
    public async Task<IActionResult> CheckEmailExists([FromQuery] string email)
    {
        var result = await authService.CheckEmailExistsAsync(email);

        return StatusCode((int)result.StatusCode, result);
    }

}