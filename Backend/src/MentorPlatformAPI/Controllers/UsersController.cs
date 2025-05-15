using Application.Services.Authentication;
using Application.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MentorPlatformAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await userService.GetUserByIdAsync(id);

        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize]
    [HttpGet("email{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var result = await userService.GetUserByEmailAsync(email);

        return StatusCode((int)result.StatusCode, result);
    }

}