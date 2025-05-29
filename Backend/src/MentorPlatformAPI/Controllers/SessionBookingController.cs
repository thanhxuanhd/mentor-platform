using System.Security.Claims;
using Application.Services.SessionBooking;
using Contract.Dtos.SessionBooking.Requests;
using Infrastructure.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SessionBookingController(
    ISessionBookingService sessionBookingService)
    : ControllerBase
{
    [HttpGet("available-mentors")]
    public async Task<IActionResult> GetAllAvailableMentors([FromQuery] AvailableMentorForBookingListRequest request)
    {
        var result = await sessionBookingService.GetAllAvailableMentorsAsync(request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("request")]
    [Authorize(Policy = RequiredRole.Learner)]
    public async Task<IActionResult> RequestBooking([FromBody] CreateSessionBookingRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await sessionBookingService.RequestBookingAsync(request, userId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("request/{bookingId:guid}/accept")]
    [Authorize(Policy = RequiredRole.Mentor)]
    public async Task<IActionResult> AcceptBooking(Guid bookingId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await sessionBookingService.AcceptBookingAsync(bookingId, userId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("request/{bookingId:guid}/cancel")]
    [Authorize(Policy = RequiredRole.Mentor)]
    public async Task<IActionResult> CancelBooking(Guid bookingId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await sessionBookingService.CancelBookingAsync(bookingId, userId);
        return StatusCode((int)result.StatusCode, result);
    }
}