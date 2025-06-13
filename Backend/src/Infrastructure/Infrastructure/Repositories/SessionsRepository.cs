using Contract.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Infrastructure.Repositories;

public class SessionsRepository(ApplicationDbContext context)
    : BaseRepository<Sessions, Guid>(context), ISessionsRepository
{
    private static bool HasOverlappingSessionTime(Sessions existingSession, Sessions newSession)
    {
        var source = existingSession.TimeSlot;
        var target = newSession.TimeSlot;
        return existingSession.Id != newSession.Id
               && existingSession.Status == SessionStatus.Approved
               && existingSession.TimeSlot.Date == newSession.TimeSlot.Date
               && (
                   (source.StartTime <= target.StartTime && source.EndTime > target.StartTime)
                   || (source.StartTime < target.EndTime && source.EndTime >= target.EndTime)
               );
    }

    public new IQueryable<Sessions> GetAll()
    {
        return _context.Sessions
            .AsSplitQuery()
            .Include(s => s.TimeSlot)
            .ThenInclude(mats => mats.Schedules);
    }

    public async Task<Sessions?> GetByIdAsync(Guid id)
    {
        return await _context.Sessions
            .AsSplitQuery()
            .Include(s => s.Learner)
            .Include(s => s.TimeSlot)
            .ThenInclude(mats => mats.Schedules)
            .ThenInclude(s => s.Mentor)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public IQueryable<Sessions> GetSessionsByLearnerId(Guid learnerId)
    {
        return _context.Sessions
            .Include(s => s.TimeSlot)
                .ThenInclude(mats => mats.Schedules)
                    .ThenInclude(s => s.Mentor)
                        .ThenInclude(m => m.UserExpertises)
                            .ThenInclude(ue => ue.Expertise)
            .Where(s => s.LearnerId == learnerId);
    }

    public Sessions AddNewBookingSession(MentorAvailableTimeSlot timeSlot, SessionType sessionType, Guid learnerId)
    {
        if (timeSlot.Sessions.Any(sessions => sessions.Status is SessionStatus.Approved or SessionStatus.Completed))
        {
            throw new Exception("Cannot add new booking session at this time.");
        }

        var bookingSession = new Sessions
        {
            Status = SessionStatus.Pending,
            LearnerId = learnerId,
            TimeSlot = timeSlot,
            Type = sessionType,
            BookedOn = DateTime.UtcNow,
        };

        timeSlot.Sessions.Add(bookingSession);

        return bookingSession;
    }

    public void CancelBookingSession(Sessions bookingSession, Guid learnerId)
    {
        var timeSlot = bookingSession.TimeSlot;
        if (bookingSession.Status is not SessionStatus.Pending)
        {
            throw new Exception("Cannot cancel this booking session.");
        }

        bookingSession.Status = SessionStatus.Cancelled;
    }

    public void MentorAcceptBookingSession(Sessions bookingSession, Guid learnerId)
    {
        Debug.Assert(_context.Entry(bookingSession.TimeSlot).Reference(t => t.Sessions).IsLoaded);
        if (bookingSession.TimeSlot.Sessions.Any(s => s.Status is SessionStatus.Approved or SessionStatus.Completed))
        {
            throw new Exception("Cannot accept this booking session at this time.");
        }

        Debug.Assert(_context.Entry(bookingSession.Learner).Reference(t => t.Sessions).IsLoaded);
        var hasOverlappingSession =
            bookingSession.Learner.Sessions!.Any(s => HasOverlappingSessionTime(s, bookingSession));

        if (hasOverlappingSession)
        {
            throw new Exception(
                "Cannot accept this booking session. The learner has another session scheduled during this time range.");
        }

        bookingSession.Status = SessionStatus.Approved;
    }

    public IQueryable<Sessions> GetAllBookingAsync()
    {
        return _context.Sessions
            .Include(s => s.Learner)
            .Include(s => s.TimeSlot)
                .ThenInclude(ts => ts.Schedules)
                    .ThenInclude(sch => sch.Mentor);
    }

    public async Task<List<Sessions>> GetByTimeSlotAsync(Guid timeslotId)
    {
        return await _context.Sessions
            .Include(s => s.TimeSlot)
            .ThenInclude(ts => ts.Schedules)
            .ThenInclude(sc => sc.Mentor)
            .Where(s => s.Id.Equals(timeslotId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Sessions>> GetLearnerUpcomingSessionsAsync(Guid userId)
    {
        var learnerSessions = _context.Sessions
            .Include(s => s.TimeSlot)
                .ThenInclude(mats => mats.Schedules)
                    .ThenInclude(s => s.Mentor)
            .Where(s => s.LearnerId == userId);

        var approvedSessions = learnerSessions
            .Where(s => s.Status == SessionStatus.Approved || s.Status == SessionStatus.Rescheduled);

        var now = DateTime.Now;

        var upcomingSessions = await approvedSessions
            .Where(s => s.TimeSlot.Date >= DateOnly.FromDateTime(now) ||
                        (s.TimeSlot.Date == DateOnly.FromDateTime(now) && s.TimeSlot.StartTime > TimeOnly.FromDateTime(now)))
            .OrderBy(s => s.TimeSlot.Date).ThenBy(s => s.TimeSlot.StartTime)
            .ToListAsync();

        return upcomingSessions;
    }
}