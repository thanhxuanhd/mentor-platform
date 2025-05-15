using Application.Services.Users;
using Contract.Dtos.Users.Paginations;
using Contract.Dtos.Users.Requests;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MentorPlatformAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    //[Authorize]
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
    [Route("{userId}")]
    public async Task<IActionResult> EditUser(Guid userId, [FromBody] EditUserRequest request)
    {
        var result = await userService.EditUserAsync(userId, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut]
    [Route("status/{userId}")]
    public async Task<IActionResult> ChangeUserStatus(Guid userId)
    {
        var result = await userService.ChangeUserStatusAsync(userId);
        return StatusCode((int)result.StatusCode, result);
    }
}