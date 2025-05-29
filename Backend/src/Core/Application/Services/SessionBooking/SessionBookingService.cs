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
    IBookingRepository bookingRepository,
    IMentorAvailableTimeSlotRepository mentorAvailableTimeSlotRepository,
    IEmailService emailService) : ISessionBookingService
{
    public async Task<Result<PaginatedList<AvailableMentorForBookingResponse>>> GetAllAvailableMentorsAsync(
        AvailableMentorForBookingListRequest request)
    {
        var mentorAvailableTimeSlots = mentorAvailableTimeSlotRepository.GetAvailableMentorsAsync();
        var availableMentorForBooking =
            await mentorAvailableTimeSlotRepository.ToPaginatedListAsync(
                mentorAvailableTimeSlots.Select(mats => mats.ToAvailableMentorForBookingResponse()),
                request.PageSize, request.PageIndex);

        return Result.Success(availableMentorForBooking, HttpStatusCode.OK);
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
        if (timeSlot == null || timeSlot.Mentor.Status != UserStatus.Active)
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
        if (timeSlot.Bookings.Any(b => b.LearnerId == learner.Id) ||
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

        var bookingSession = await bookingRepository.GetByIdAsync(bookingSessionId);
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

    private async Task<Result<SessionSlotStatusResponse>> AcceptBookingInternalAsync(Booking bookingSession,
        User learner)
    {
        var timeSlot = bookingSession.TimeSlot;
        bookingRepository.MentorAcceptBookingSession(bookingSession, learner.Id);
        await bookingRepository.SaveChangesAsync();

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

        return Result.Success(bookingSession.ToSessionSlotStatusResponse(), HttpStatusCode.OK);
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

        var bookingSession = await bookingRepository.GetByIdAsync(bookingSessionId);
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

    private async Task<Result<SessionSlotStatusResponse>> CancelBookingInternalAsync(Booking bookingSession,
        User cancellingLearner)
    {
        var timeSlot = bookingSession.TimeSlot;
        bookingRepository.MentorCancelBookingSession(bookingSession, cancellingLearner.Id);
        await bookingRepository.SaveChangesAsync();

        // TODO: sending email?

        return Result.Success(bookingSession.ToSessionSlotStatusResponse(), HttpStatusCode.OK);
    }
}