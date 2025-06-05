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
    IScheduleRepository scheduleRepository,
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
            var schedules = await scheduleRepository.GetByIdAsync(mentorAvailableTimeSlot.ScheduleId);
            var user = await userRepository.GetUserDetailAsync(schedules!.MentorId);
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
        var sessions = sessionBookingRepository.GetAllSessionsByTimeSlotId(timeSlotId);

        var userBookingRequests =
            await sessionBookingRepository.ToListAsync(
                sessions.Select(mats => mats.ToSessionSlotStatusResponse()));

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

        if (bookingSession.TimeSlot.Schedules.Mentor.Status != UserStatus.Active)
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
                EmailConstants.BodyMeetingBookingConfirmationEmail(learner.FullName,
                    new DateTime(timeSlot.Date, timeSlot.StartTime),
                    timeSlot.Schedules.Mentor.FullName));

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

        // if (bookingSession.TimeSlot.Schedules.Mentor.Status != UserStatus.Active)
        // {
        //     return Result.Failure<SessionSlotStatusResponse>(
        //         "Selected slot is unavailable.",
        //         HttpStatusCode.BadRequest);
        // }

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

    public async Task<Result<List<LearnerSessionBookingResponse>>> GetAllBooking()
    {
        var sessionList = await sessionBookingRepository.GetAllBookingAsync();

        var resultList = sessionList
        .OrderByDescending(s => s.TimeSlot.Date)
        .ThenByDescending(s => s.TimeSlot.StartTime)
        .Select(s => new LearnerSessionBookingResponse
        {
            Id = s.Id,
            TimeSlotId = s.TimeSlotId,
            LearnerId = s.LearnerId,
            Status = s.Status.ToString(),
            Date = s.TimeSlot.Date,
            StartTime = s.TimeSlot.StartTime,
            EndTime = s.TimeSlot.EndTime,
            FullNameLearner = s.Learner.FullName,
            PreferredCommunicationMethod = s.Learner.PreferredCommunicationMethod.ToString()
        })
        .ToList();

        return Result.Success(resultList, HttpStatusCode.OK);
    }

    public async Task<Result<LearnerSessionBookingResponse>> GetSessionsBookingByIdAsync(Guid id)
    {
        var session = await sessionBookingRepository.GetByIdAsync(id);
        if (session == null)
        {
            return Result.Failure<LearnerSessionBookingResponse>($"Session with id {id} not found.", HttpStatusCode.NotFound);
        }

        var result = new LearnerSessionBookingResponse
        {
            TimeSlotId = session.TimeSlotId,
            LearnerId = session.LearnerId,
            Status = session.Status.ToString(),
            Date = session.TimeSlot.Date,
            StartTime = session.TimeSlot.StartTime,
            EndTime = session.TimeSlot.EndTime
        };

        return Result.Success(result, HttpStatusCode.OK);
    }


    public async Task<Result<bool>> UpdateStatusSessionAsync(Guid id, SessionBookingRequest request)
    {
        var sessionList = await sessionBookingRepository.GetAllBookingAsync();
        var session = sessionList.FirstOrDefault(s => s.Id == id);

        if (session == null)
        {
            return Result.Failure<bool>($"Session with id {id} not found.", HttpStatusCode.NotFound);
        }

        session.Status = (SessionStatus)request.Status;
        string subject = string.Empty;
        string body = string.Empty;

        if (request.Status == SessionStatus.Approved)
        {
            subject = EmailConstants.SUBJECT_SESSION_ACCEPTED;
            body = EmailConstants.BodySessionAcceptedEmail(id);
            var conflictingSessions = sessionList.Where(s =>
                s.Id != session.Id &&
                s.Status == SessionStatus.Pending &&
                s.TimeSlot.Date == session.TimeSlot.Date &&
                s.TimeSlot.StartTime == session.TimeSlot.StartTime &&
                s.TimeSlot.EndTime == session.TimeSlot.EndTime
            ).ToList();

            foreach (var conflict in conflictingSessions)
            {
                conflict.Status = SessionStatus.Canceled;

                var conflictUser = await userRepository.GetByIdAsync(conflict.LearnerId);
                if (conflictUser != null)
                {
                    var cancelSubject = EmailConstants.SUBJECT_SESSION_CANCELLED;
                    var cancelBody = EmailConstants.BodySessionCancelledEmail(conflict.Id);
                    await emailService.SendEmailAsync(conflictUser.Email, cancelSubject, cancelBody);
                }

                sessionBookingRepository.Update(conflict);
            }
        }
        else if (request.Status == SessionStatus.Canceled)
        {
            subject = EmailConstants.SUBJECT_SESSION_CANCELLED;
            body = EmailConstants.BodySessionAcceptedEmail(id);
        }

        if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(body))
        {
            var user = await userRepository.GetByIdAsync(session.LearnerId);
            if (user == null)
            {
                return Result.Failure<bool>($"User with id {session.LearnerId} not found.", HttpStatusCode.NotFound);
            }

            var emailResult = await emailService.SendEmailAsync(user.Email, subject, body);
            if (!emailResult)
            {
                return Result.Failure<bool>("Failed to send email.", HttpStatusCode.InternalServerError);
            }
        }

        sessionBookingRepository.Update(session);
        await sessionBookingRepository.SaveChangesAsync();

        return Result.Success(true, HttpStatusCode.OK);
    }

    public async Task<Result<bool>> UpdateRecheduleSessionAsync(Guid id, SessionUpdateRecheduleRequest request)
    {
        var sessionList = await sessionBookingRepository.GetAllBookingAsync();
        var session = sessionList.FirstOrDefault(s => s.Id == id);

        if (session == null)
        {
            return Result.Failure<bool>($"Session with id {id} not found.", HttpStatusCode.NotFound);
        }

        session.TimeSlot.Date = request.Date;
        session.TimeSlot.StartTime = request.StartTime;
        session.TimeSlot.EndTime = request.EndTime;
        session.Status = SessionStatus.Rescheduled;

        var user = await userRepository.GetByIdAsync(session.LearnerId);
        if (user == null)
        {
            return Result.Failure<bool>($"User with id {session.LearnerId} not found.", HttpStatusCode.NotFound);
        }

        string subject = EmailConstants.SUBJECT_SESSION_RESCHEDULED;
        string body = EmailConstants.BodySessionRescheduledEmail(id, request.Date, request.StartTime, request.EndTime, request.Reason);

        var emailResult = await emailService.SendEmailAsync(user.Email, subject, body);
        if (!emailResult)
        {
            return Result.Failure<bool>("Failed to send reschedule email.", HttpStatusCode.InternalServerError);
        }

        sessionBookingRepository.Update(session);
        await sessionBookingRepository.SaveChangesAsync();

        return Result.Success(true, HttpStatusCode.OK);
    }


}