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

    public async Task<Result<PaginatedList<AvailableTimeSlotResponse>>> GetAllTimeSlotByMentorAsync(
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

        var availableMentorForBooking = await mentorAvailableTimeSlotRepository.ToListAsync(mentorAvailableTimeSlots);

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

    public async Task<Result<List<TimeSlotByMentorAndDateResponse>>> GetAllTimeSlotByMentorAndDateAsync(Guid mentorId,
        Guid learnerId,
        AvailableTimeSlotByDateListRequest request)
    {
        var currentDateTime = DateTime.Now;
        var mentorAvailableTimeSlots = mentorAvailableTimeSlotRepository.GetAvailableTimeSlot();

        var availableTimeSlot =
            await mentorAvailableTimeSlotRepository.ToListAsync(
                mentorAvailableTimeSlots
                    .Where(mats => mats.Schedules.MentorId == mentorId)
                    .Where(mats => mats.Date == request.Date)
                    .Where(mats => mats.Date > DateOnly.FromDateTime(currentDateTime) ||
                                (mats.Date == DateOnly.FromDateTime(currentDateTime) &&
                                 mats.StartTime > TimeOnly.FromDateTime(currentDateTime)))

                    .Select(mats => SessionBookingExtensions.CreateTimeSlotByMentorAndDateListResponse(mats, learnerId)));

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
        if (timeSlot.Sessions.Any(s => s.Status is SessionStatus.Approved or SessionStatus.Completed or SessionStatus.Rescheduled))
        {
            return Result.Failure<SessionSlotStatusResponse>(
                $"Selected slot in {timeSlot.StartTime} - {timeSlot.EndTime} by {timeSlot.Schedules.Mentor.FullName} is rejected.",
                HttpStatusCode.Conflict);
        }

        if (timeSlot.Sessions.Any(s => s.LearnerId == learner.Id && s.Status == SessionStatus.Pending))
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

        await emailService.SendEmailAsync(learner.Email,
            EmailConstants.SUBJECT_MEETING_BOOKING_CONFIRMATION,
            EmailConstants.BodyMeetingBookingConfirmationEmail(learner.FullName,
                new DateTime(timeSlot.Date, timeSlot.StartTime),
                timeSlot.Schedules.Mentor.FullName));

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
        User cancellingLearner)
    {
        sessionBookingRepository.CancelBookingSession(bookingSession, cancellingLearner.Id);
        await sessionBookingRepository.SaveChangesAsync();

        await emailService.SendEmailAsync(cancellingLearner.Email,
            EmailConstants.SUBJECT_MEETING_BOOKING_CANCELLED,
            EmailConstants.BodyMeetingBookingCancelledEmail(cancellingLearner.FullName,
                new DateTime(bookingSession.TimeSlot.Date, bookingSession.TimeSlot.EndTime),
                bookingSession.TimeSlot.Schedules.Mentor.FullName));


        return Result.Success(bookingSession.ToSessionSlotStatusResponse(), HttpStatusCode.OK);
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
            var existingApproved = sessionList.FirstOrDefault(s =>
                s.Id != session.Id &&
                s.Status == SessionStatus.Approved &&
                s.TimeSlot.Date == session.TimeSlot.Date &&
                s.TimeSlot.StartTime == session.TimeSlot.StartTime &&
                s.TimeSlot.EndTime == session.TimeSlot.EndTime
            );

            if (existingApproved != null)
            {
                return Result.Failure<bool>(
                    $"A session for the same time slot ({session.TimeSlot.StartTime} - {session.TimeSlot.EndTime}) is already approved.",
                    HttpStatusCode.Conflict
                );
            }

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
                if (conflictUser?.IsReceiveNotification == true)
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

            if (user.IsReceiveNotification)
            {
                var emailResult = await emailService.SendEmailAsync(user.Email, subject, body);
                if (!emailResult)
                {
                    return Result.Failure<bool>("Failed to send email.", HttpStatusCode.InternalServerError);
                }
            }
        }

        sessionBookingRepository.Update(session);
        await sessionBookingRepository.SaveChangesAsync();

        return Result.Success(true, HttpStatusCode.OK);
    }


    public async Task<Result<bool>> UpdateRecheduleSessionAsync(Guid id, SessionUpdateRecheduleRequest request)
    {
        var session = (await sessionBookingRepository.GetAllBookingAsync())
            .FirstOrDefault(s => s.Id == id);

        if (session == null)
        {
            return Result.Failure<bool>($"Session with id {id} not found.", HttpStatusCode.NotFound);
        }

        var newTimeSlot = await mentorAvailableTimeSlotRepository.GetByIdAsync(request.TimeSlotId);
        if (newTimeSlot == null)
        {
            return Result.Failure<bool>("Selected TimeSlot does not exist.", HttpStatusCode.BadRequest);
        }

        if (newTimeSlot.Sessions.Any(b =>
            b.Status is SessionStatus.Approved or SessionStatus.Completed or SessionStatus.Rescheduled))
        {
            return Result.Failure<bool>(
                $"Selected slot in {newTimeSlot.StartTime} - {newTimeSlot.EndTime} by {newTimeSlot.Schedules.Mentor.FullName} is not available.",
                HttpStatusCode.Conflict);
        }

        if (newTimeSlot.Sessions.Any(b => b.LearnerId == session.LearnerId && b.Status == SessionStatus.Pending))
        {
            return Result.Failure<bool>("You already have a booking for this time slot.", HttpStatusCode.Conflict);
        }

        session.TimeSlot = newTimeSlot;
        session.Status = SessionStatus.Rescheduled;

        var user = await userRepository.GetByIdAsync(session.LearnerId);
        if (user == null)
        {
            return Result.Failure<bool>($"User with id {session.LearnerId} not found.", HttpStatusCode.NotFound);
        }

        if (user.IsReceiveNotification)
        {
            string subject = EmailConstants.SUBJECT_SESSION_RESCHEDULED;
            string body = EmailConstants.BodySessionRescheduledEmail(
                id,
                newTimeSlot.Date,
                newTimeSlot.StartTime,
                newTimeSlot.EndTime,
                request.Reason
            );

            var emailResult = await emailService.SendEmailAsync(user.Email, subject, body);
            if (!emailResult)
            {
                return Result.Failure<bool>("Failed to send reschedule email.", HttpStatusCode.InternalServerError);
            }
        }

        sessionBookingRepository.Update(session);
        await sessionBookingRepository.SaveChangesAsync();

        return Result.Success(true, HttpStatusCode.OK);
    }

    public async Task<Result<List<AvailableTimeSlotResponse>>> GetAllTimeSlotByMentorAsync(Guid mentorId, DateOnly date)
    {
        var mentorAvailableTimeSlots = mentorAvailableTimeSlotRepository.GetAvailableTimeSlot();

        var query = mentorAvailableTimeSlots
                .Where(mats => mats.Schedules.MentorId == mentorId && mats.Date == date)
                .Select(mats => mats.ToAvailableTimeSlotResponse());

        var availableTimeSlots = await mentorAvailableTimeSlotRepository.ToListAsync(query);

        return Result.Success(availableTimeSlots, HttpStatusCode.OK);
    }
}