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
    public async Task<IActionResult> GetAllTimeSlotByMentorAsync(Guid mentorId,
        [FromQuery] AvailableTimeSlotListRequest request)
    {
        // TODO: Admin + Resource Owner
        var result = await sessionBookingService.GetAllTimeSlotByMentorAsync(mentorId, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("available-mentors")]
    public async Task<IActionResult> GetAllAvailableMentorForBooking()
    {
        var result = await sessionBookingService.GetAllAvailableMentorForBookingAsync();
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("available-mentors/timeslots/{mentorId:guid}")]
    [Authorize(Policy = RequiredRole.Learner)]
    public async Task<IActionResult> GetAllTimeSlotByMentorAndDate(Guid mentorId,
        [FromQuery] AvailableTimeSlotByDateListRequest request)
    {
        var learnerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await sessionBookingService.GetAllTimeSlotByMentorAndDateAsync(mentorId, learnerId, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("timeslots/requests/{timeSlotId:guid}")]
    public async Task<IActionResult> GetAllBookingRequestByTimeSlot(Guid timeSlotId)
    {
        // TODO: resource owner authorization
        var result = await sessionBookingService.GetAllBookingRequestByTimeSlot(timeSlotId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("timeslots/requests/me")]
    [Authorize(Policy = RequiredRole.Learner)]
    public async Task<IActionResult> GetAllBookingRequestByLearner()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await sessionBookingService.GetAllBookingRequestByLearnerId(userId);
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
    public async Task<IActionResult> CancelBooking(Guid bookingId)
    {
        // TODO: resource owner authorization + learner that made the booking request
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await sessionBookingService.CancelBookingAsync(bookingId, userId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("request/get")]
    //[Authorize(Policy = RequiredRole.Mentor)]
    public async Task<IActionResult> GetAllBookingbyMentor(Guid mentorId)
    {
        var result = await sessionBookingService.GetAllBooking(mentorId);
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
    public async Task<IActionResult> UpdateRecheduleSession(Guid id, [FromBody] SessionUpdateRescheduleRequest request)
    {
        var result = await sessionBookingService.UpdateRescheduleSessionAsync(id, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("available-mentors/timeslots/get/{mentorId}")]
    //[Authorize(Policy = RequiredRole.Learner)]
    public async Task<IActionResult> GetAllTimeSlotByMentor(Guid mentorId, DateOnly date)
        {
        var result = await sessionBookingService.GetAllTimeSlotByMentorAsync(mentorId, date);
        return StatusCode((int)result.StatusCode, result);
    }
}