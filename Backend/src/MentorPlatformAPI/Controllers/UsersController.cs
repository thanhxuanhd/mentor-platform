using Application.Services.Users;
using Contract.Dtos.Users.Paginations;
using Contract.Dtos.Users.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Infrastructure.Services.Authorization;

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
    [HttpGet]
    [Route("filter")]
    public async Task<IActionResult> FilterUser([FromQuery] UserFilterPagedRequest request)
    {
        var result = await userService.FilterUserAsync(request);
        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize]
    [HttpPut]
    [Route("{userId}")]
    public async Task<IActionResult> EditUser(Guid userId, [FromBody] EditUserRequest request)
    {
        var result = await userService.EditUserAsync(userId, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize]
    [HttpPut]
    [Route("status/{userId}")]
    public async Task<IActionResult> ChangeUserStatus(Guid userId)
    {
        var result = await userService.ChangeUserStatusAsync(userId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut]
    [Route("{userId}/detail")]
    public async Task<IActionResult> EditUserDetailAsync(Guid userId, [FromBody] EditUserProfileRequest request)
    {
        var result = await userService.EditUserDetailAsync(userId, request);

        return StatusCode((int)result.StatusCode, result);
    }

    //[Authorize]
    [HttpGet]
    [Route("{userId}/detail")]
    public async Task<IActionResult> GetUserDetailAsync(Guid userId)
    {
        var result = await userService.GetUserDetailAsync(userId);

        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize]
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var result = await userService.GetUserByEmailAsync(email);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("request-forgot-password/{email}")]
    public async Task<IActionResult> ForgotPasswordRequest(string email)
    {
        var result = await userService.ForgotPasswordRequest(email);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("avatar/{userId}")]
    public async Task<IActionResult> UploadAvatar(Guid userId, IFormFile file)
    {
        var request = Request;
        var result = await userService.UploadAvatarAsync(userId, request, file);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("avatar")]
    public IActionResult RemoveAvatar(string imageUrl)
    {
        var result = userService.RemoveAvatar(imageUrl);

        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize(Roles = "Mentor")]
    [HttpPost("mentor-documents")]
    public async Task<IActionResult> UploadMentorDocument(IFormFile file)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var request = Request;
        var result = await userService.UploadDocumentAsync(Guid.Parse(userId!), request, file);

        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize(Roles = "Mentor")]
    [HttpDelete("mentor-documents")]
    public async Task<IActionResult> RemoveMentorDocument(string documentUrl)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await userService.RemoveDocumentAsync(Guid.Parse(userId!), documentUrl);

        return StatusCode((int)result.StatusCode, result);
    }
    
    [Authorize]
    [HttpGet]
    [Route("mentors")]
    public async Task<IActionResult> GetAllMentor()
    {
        var result = await userService.GetAllMentorAsync();
        return StatusCode((int)result.StatusCode, result);
    }
}