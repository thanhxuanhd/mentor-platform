using Application.Services.Authentication;
using Application.Services.Users;
using Contract.Dtos.Users.Paginations;
using Contract.Dtos.Users.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await userService.GetUserByIdAsync(id);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet]
    [Route("filter")]
    public async Task<IActionResult> FilterUser([FromQuery] UserFilterPagedRequest request)
    {
        var result = await userService.FilterUserAsync(request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut]
    [Route("update/{userId}")]
    public async Task<IActionResult> EditUser(Guid userId, [FromBody] EditUserRequest request)
    {
        var result = await userService.EditUserAsync(userId, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut]
    [Route("change-status/{userId}")]
    public async Task<IActionResult> ChangeUserStatus(Guid userId)
    {
        var result = await userService.ChangeUserStatusAsync(userId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var result = await userService.GetUserByEmailAsync(email);

        return StatusCode((int)result.StatusCode, result);
    }

}