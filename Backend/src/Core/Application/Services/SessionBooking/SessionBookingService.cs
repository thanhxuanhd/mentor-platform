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
    ISessionsRepository sessionBookingRepository,
    IMentorAvailabilityTimeSlotRepository mentorAvailableTimeSlotRepository,
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

    public async Task<Result<List<AvailableMentorForBookingResponse>>> GetAllAvailableMentorForBookingAsync()
    {
        var mentorAvailableTimeSlots = mentorAvailableTimeSlotRepository.GetAvailableMentorForBooking();

        var availableMentorForBooking =
            await mentorAvailableTimeSlotRepository.ToListAsync(mentorAvailableTimeSlots);

        List<AvailableMentorForBookingResponse> availableMentorForBookingWithMentorDetails = [];
        foreach (var mentorAvailableTimeSlot in availableMentorForBooking)
        {
            var schedules = mentorAvailableTimeSlot.Schedules;
            var user = await userRepository.GetUserDetailAsync(schedules.MentorId);
            availableMentorForBookingWithMentorDetails.Add(
                SessionBookingExtensions.CreateAvailableMentorForBookingResponse(user!, schedules));
        }

        return Result.Success(availableMentorForBookingWithMentorDetails, HttpStatusCode.OK);
    }

    public async Task<Result<List<AvailableTimeSlotResponse>>> GetAllAvailableTimeSlotByMentorAndDateAsync(
        Guid mentorId, AvailableTimeSlotByDateListRequest request)
    {
        var mentorAvailableTimeSlots = mentorAvailableTimeSlotRepository.GetAvailableTimeSlot();

        var availableTimeSlot =
            await mentorAvailableTimeSlotRepository.ToListAsync(
                mentorAvailableTimeSlots
                    .Where(mats => mats.Schedules.MentorId == mentorId)
                    .Where(mats => mats.Date == request.Date)
                    .Select(mats => mats.ToAvailableTimeSlotResponse()));

        return Result.Success(availableTimeSlot, HttpStatusCode.OK);
    }

    public async Task<Result<List<SessionSlotStatusResponse>>> GetAllBookingRequestByTimeSlot(Guid timeSlotId)
    {
        var sessions = sessionBookingRepository.GetAll();

        var userBookingRequests =
            await sessionBookingRepository.ToListAsync(
                sessions
                    .Where(s => s.TimeSlotId == timeSlotId)
                    .Select(s => s.ToSessionSlotStatusResponse()));

        return Result.Success(userBookingRequests, HttpStatusCode.OK);
    }

    public async Task<Result<List<GetAllRequestByLearnerResponse>>> GetAllBookingRequestByLearnerId(Guid learnerId)
    {
        var sessions = sessionBookingRepository.GetSessionsByLearnerId(learnerId);
        var userBookingRequests = await sessionBookingRepository.ToListAsync(
            sessions.Select(s => s.ToGetAllRequestLearnerResponse()));

        return Result.Success(userBookingRequests, HttpStatusCode.OK);
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
        if (timeSlot == null)
        {
            return Result.Failure<SessionSlotStatusResponse>(
                "Booking Session not found.",
                HttpStatusCode.BadRequest);
        }

        return await RequestBookingInternalAsync(timeSlot, user, request.SessionType);
    }

    private async Task<Result<SessionSlotStatusResponse>> RequestBookingInternalAsync(
        MentorAvailableTimeSlot timeSlot,
        User learner,
        SessionType sessionType)
    {
        if (timeSlot.Sessions.Any(b => b.Status is SessionStatus.Approved or SessionStatus.Completed))
        {
            return Result.Failure<SessionSlotStatusResponse>(
                $"Selected slot in {timeSlot.StartTime} - {timeSlot.EndTime} by {timeSlot.Schedules.Mentor.FullName} is rejected.",
                HttpStatusCode.BadRequest);
        }

        if (timeSlot.Sessions.Any(b => b.LearnerId == learner.Id && b.Status == SessionStatus.Pending))
        {
            return Result.Failure<SessionSlotStatusResponse>("You have already booked this slot.",
                HttpStatusCode.Conflict);
        }

        var bookingSession = sessionBookingRepository.AddNewBookingSession(timeSlot, sessionType, learner.Id);
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

        return await AcceptBookingInternalAsync(bookingSession, user);
    }

    private async Task<Result<SessionSlotStatusResponse>> AcceptBookingInternalAsync(Sessions bookingSession,
        User learner)
    {
        var timeSlot = bookingSession.TimeSlot;
        sessionBookingRepository.MentorAcceptBookingSession(bookingSession, learner.Id);
        await sessionBookingRepository.SaveChangesAsync();

        var mailSent =
            await emailService.SendEmailAsync(learner.Email,
                EmailConstants.SUBJECT_MEETING_BOOKING_CONFIRMATION,
                EmailConstants.BodyMeetingBookingConfirmationEmail(learner.FullName,
                    new DateTime(timeSlot.Date, timeSlot.StartTime),
                    timeSlot.Schedules.Mentor.FullName));

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

        var bookingSession = await sessionBookingRepository.GetByIdAsync(bookingSessionId);
        if (bookingSession == null)
        {
            return Result.Failure<SessionSlotStatusResponse>(
                "Booking Session not found.",
                HttpStatusCode.NotFound);
        }

        return await CancelBookingInternalAsync(bookingSession, user);
    }

    private async Task<Result<SessionSlotStatusResponse>> CancelBookingInternalAsync(Sessions bookingSession,
        User cancellingLearner, bool sendMail = false)
    {
        sessionBookingRepository.CancelBookingSession(bookingSession, cancellingLearner.Id);
        await sessionBookingRepository.SaveChangesAsync();

        if (sendMail)
        {
            var mailSent =
                await emailService.SendEmailAsync(cancellingLearner.Email,
                    EmailConstants.SUBJECT_MEETING_BOOKING_CANCELLED,
                    EmailConstants.BodyMeetingBookingConfirmationEmail(cancellingLearner.FullName,
                        new DateTime(bookingSession.TimeSlot.Date, bookingSession.TimeSlot.EndTime),
                        bookingSession.TimeSlot.Schedules.Mentor.FullName));

            if (!mailSent)
            {
                return Result.Failure<SessionSlotStatusResponse>(
                    "Failed to send email",
                    HttpStatusCode.InternalServerError);
            }
        }

        return Result.Success(bookingSession.ToSessionSlotStatusResponse(), HttpStatusCode.OK);
    }
}