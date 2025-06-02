using System.Net;
using Contract.Dtos.SessionBooking.Requests;
using Contract.Dtos.SessionBooking.Response;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.SessionBooking;

public class SessionBookingService(
    IUserRepository userRepository,
    ISessionBookingRepository sessionBookingRepository,
    IScheduleRepository scheduleRepository,
    IMentorAvailableTimeSlotRepository mentorAvailableTimeSlotRepository,
    IEmailService emailService) : ISessionBookingService
{
    public async Task<Result<PaginatedList<AvailableTimeSlotResponse>>> GetAllAvailableTimeSlotAsync(
        AvailableTimeSlotListRequest request)
    {
        var mentorAvailableTimeSlots = mentorAvailableTimeSlotRepository.GetAvailableTimeSlot();
        var availableTimeSlot =
            await mentorAvailableTimeSlotRepository.ToPaginatedListAsync(
                mentorAvailableTimeSlots.Select(mats => mats.ToAvailableTimeSlotResponse()),
                request.PageSize, request.PageIndex);

        return Result.Success(availableTimeSlot, HttpStatusCode.OK);
    }

    public async Task<Result<PaginatedList<AvailableTimeSlotResponse>>> GetAllAvailableTimeSlotByMentorAsync(
        Guid mentorId, AvailableTimeSlotListRequest request)
    {
        var mentorAvailableTimeSlots = mentorAvailableTimeSlotRepository.GetAvailableTimeSlot();

        var availableTimeSlot =
            await mentorAvailableTimeSlotRepository.ToPaginatedListAsync(
                mentorAvailableTimeSlots
                    .Where(mats => mats.Schedules.MentorId == mentorId)
                    .Select(mats => mats.ToAvailableTimeSlotResponse()),
                request.PageSize, request.PageIndex);

        return Result.Success(availableTimeSlot, HttpStatusCode.OK);
    }

    public async Task<Result<PaginatedList<AvailableMentorForBookingResponse>>> GetAllAvailableMentorForBookingAsync(
        AvailableMentorForBookingListRequest request)
    {
        var mentorAvailableTimeSlots = mentorAvailableTimeSlotRepository.GetAvailableMentorForBooking();

        var availableMentorForBooking =
            await mentorAvailableTimeSlotRepository.ToPaginatedListAsync(mentorAvailableTimeSlots, request.PageSize,
                request.PageIndex);

        List<AvailableMentorForBookingResponse> availableMentorForBookingWithMentorDetails = [];
        foreach (var mentorAvailableTimeSlot in availableMentorForBooking.Items)
        {
            var user = await userRepository.GetUserDetailAsync(mentorAvailableTimeSlot.Schedules.MentorId);
            var schedule = await scheduleRepository.GetByIdAsync(mentorAvailableTimeSlot.ScheduleId);
            availableMentorForBookingWithMentorDetails.Add(
                SessionBookingExtensions.CreateAvailableMentorForBookingResponse(user!, schedule!));
        }

        return Result.Success(
            new PaginatedList<AvailableMentorForBookingResponse>(availableMentorForBookingWithMentorDetails,
                availableMentorForBooking.TotalCount,
                availableMentorForBooking.PageIndex,
                availableMentorForBooking.PageSize),
            HttpStatusCode.OK);
    }

    public async Task<Result<SessionSlotStatusResponse>> RequestBookingAsync(CreateSessionBookingRequest request,
        Guid requestingLearnerId)
    {
        var user = await userRepository.GetByIdAsync(requestingLearnerId);
        if (user == null)
        {
            return Result.Failure<SessionSlotStatusResponse>(
                "User not found.",
                HttpStatusCode.NotFound);
        }

        var timeSlot = await mentorAvailableTimeSlotRepository.GetByIdAsync(request.TimeSlotId);
        if (timeSlot == null || timeSlot.Schedules.Mentor.Status != UserStatus.Active)
        {
            return Result.Failure<SessionSlotStatusResponse>(
                "Selected slot is unavailable.",
                HttpStatusCode.BadRequest);
        }

        return await RequestBookingInternalAsync(timeSlot, user);
    }

    private async Task<Result<SessionSlotStatusResponse>> RequestBookingInternalAsync(
        MentorAvailableTimeSlot timeSlot,
        User learner)
    {
        if (timeSlot.Sessions.Any(b => b.LearnerId == learner.Id) ||
            timeSlot.Status is SessionStatus.Expired or SessionStatus.Confirmed)
        {
            return Result.Failure<SessionSlotStatusResponse>(
                $"Selected slot in {timeSlot.StartTime} - {timeSlot.EndTime} by {timeSlot.Mentor.FullName} is rejected.",
                HttpStatusCode.BadRequest);
        }

        if (timeSlot.Status is SessionStatus.Processing)
        {
            var reservedBooking = timeSlot.Bookings.MaxBy(b => b.BookedDateTime);
            return Result.Failure<SessionSlotStatusResponse>(
                reservedBooking!.LearnerId == learner.Id
                    ? "You have already booked this slot."
                    : "Selected slot is booked by others, please select an other slot!", HttpStatusCode.Conflict);
        }

        var bookingSession = mentorAvailableTimeSlotRepository.AddNewBookingSession(timeSlot, learner.Id);
        await mentorAvailableTimeSlotRepository.SaveChangesAsync();

        return Result.Success(bookingSession.ToSessionSlotStatusResponse(), HttpStatusCode.OK);
    }

    public async Task<Result<SessionSlotStatusResponse>> AcceptBookingAsync(Guid bookingSessionId,
        Guid acceptingLearnerId)
    {
        var user = await userRepository.GetByIdAsync(acceptingLearnerId);
        if (user == null)
        {
            return Result.Failure<SessionSlotStatusResponse>(
                "User not found.",
                HttpStatusCode.NotFound);
        }

        var bookingSession = await sessionBookingRepository.GetByIdAsync(bookingSessionId);
        if (bookingSession == null)
        {
            return Result.Failure<SessionSlotStatusResponse>(
                "Booking Session not found.",
                HttpStatusCode.NotFound);
        }

        if (bookingSession.TimeSlot.Mentor.Status != UserStatus.Active)
        {
            return Result.Failure<SessionSlotStatusResponse>(
                "Selected slot is unavailable.",
                HttpStatusCode.BadRequest);
        }

        return await AcceptBookingInternalAsync(bookingSession, user);
    }

    private async Task<Result<SessionSlotStatusResponse>> AcceptBookingInternalAsync(Sessions sessionsSession,
        User learner)
    {
        var timeSlot = sessionsSession.TimeSlot;
        sessionBookingRepository.MentorAcceptBookingSession(sessionsSession, learner.Id);
        await sessionBookingRepository.SaveChangesAsync();

        var mailSent =
            await emailService.SendEmailAsync(learner.Email,
                EmailConstants.SUBJECT_MEETING_BOOKING_CONFIRMATION,
                EmailConstants.BodyMeetingBookingConfirmationEmail(learner.FullName, timeSlot.StartTime,
                    timeSlot.Mentor.FullName));

        if (!mailSent)
        {
            return Result.Failure<SessionSlotStatusResponse>(
                "Failed to send email",
                HttpStatusCode.InternalServerError);
        }

        return Result.Success(sessionsSession.ToSessionSlotStatusResponse(), HttpStatusCode.OK);
    }

    public async Task<Result<SessionSlotStatusResponse>> CancelBookingAsync(Guid bookingSessionId,
        Guid cancellingLearnerId)
    {
        var user = await userRepository.GetByIdAsync(cancellingLearnerId);
        if (user == null)
        {
            return Result.Failure<SessionSlotStatusResponse>(
                "User not found.",
                HttpStatusCode.NotFound);
        }

        var bookingSession = await sessionBookingRepository.GetByIdAsync(bookingSessionId);
        if (bookingSession == null)
        {
            return Result.Failure<SessionSlotStatusResponse>(
                "Booking Session not found.",
                HttpStatusCode.NotFound);
        }

        if (bookingSession.TimeSlot.Mentor.Status != UserStatus.Active)
        {
            return Result.Failure<SessionSlotStatusResponse>(
                "Selected slot is unavailable.",
                HttpStatusCode.BadRequest);
        }

        return await CancelBookingInternalAsync(bookingSession, user);
    }

    private async Task<Result<SessionSlotStatusResponse>> CancelBookingInternalAsync(Sessions sessionsSession,
        User cancellingLearner)
    {
        var timeSlot = sessionsSession.TimeSlot;
        sessionBookingRepository.MentorCancelBookingSession(sessionsSession, cancellingLearner.Id);
        await sessionBookingRepository.SaveChangesAsync();

        // TODO: sending email?

        return Result.Success(sessionsSession.ToSessionSlotStatusResponse(), HttpStatusCode.OK);
    }
}