using System.Security.Claims;
using Application.Services.SessionBooking;
using Contract.Dtos.SessionBooking.Requests;
using Infrastructure.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

//[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SessionBookingController(
    ISessionBookingService sessionBookingService)
    : ControllerBase
{
    [HttpGet("available-timeslots")]
    //[Authorize(Policy = RequiredRole.Admin)]
    public async Task<IActionResult> GetAllAvailableTimeSlot([FromQuery] AvailableTimeSlotListRequest request)
    {
        var result = await sessionBookingService.GetAllAvailableTimeSlotAsync(request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("available-timeslots/{mentorId:guid}")]
    [Authorize(Policy = RequiredRole.Admin)]
    public async Task<IActionResult> GetAllAvailableTimeSlotByMentorAsync(Guid mentorId,
        [FromQuery] AvailableTimeSlotListRequest request)
    {
        var result = await sessionBookingService.GetAllAvailableTimeSlotByMentorAsync(mentorId, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("available-mentors")]
    public async Task<IActionResult> GetAllAvailableMentorForBooking(
        [FromQuery] AvailableMentorForBookingListRequest request)
    {
        var result = await sessionBookingService.GetAllAvailableMentorForBookingAsync(request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("available-mentors/timeslots/{mentorId:guid}")]
    public async Task<IActionResult> GetAllAvailableTimeSlotByMentorAndDateAsync(Guid mentorId,
        [FromQuery] AvailableTimeSlotByDateListRequest request)
    {
        // TODO: resource owner authorization + admin
        var result = await sessionBookingService.GetAllAvailableTimeSlotByMentorAndDateAsync(mentorId, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("timeslots/requests/{timeSlotId:guid}")]
    public async Task<IActionResult> GetAllBookingRequestByTimeSlot(Guid timeSlotId)
    {
        // TODO: resource owner authorization
        var result = await sessionBookingService.GetAllBookingRequestByTimeSlot(timeSlotId);
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

    [HttpGet("request/get")]
    //[Authorize(Policy = RequiredRole.Mentor)]
    public async Task<IActionResult> GetAllBooking()
    {
        var result = await sessionBookingService.GetAllBooking();
        return StatusCode((int)result.StatusCode, result);
    }
    [HttpGet("request/get/{id}")]
    public async Task<IActionResult> GetSessionsBookingById(Guid id)
    {
        var result = await sessionBookingService.GetSessionsBookingByIdAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStatusSession(Guid id, [FromBody] SessionBookingRequest request)
    {
        var result = await sessionBookingService.UpdateStatusSessionAsync(id, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateRecheduleSession(Guid id, [FromBody] SessionUpdateRecheduleRequest request)
    {
        var result = await sessionBookingService.UpdateRecheduleSessionAsync(id, request);
        return StatusCode((int)result.StatusCode, result);
    }
}