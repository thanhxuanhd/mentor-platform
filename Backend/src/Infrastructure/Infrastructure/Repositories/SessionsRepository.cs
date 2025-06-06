using Contract.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SessionsRepository(ApplicationDbContext context)
    : BaseRepository<Sessions, Guid>(context), ISessionsRepository
{
    public new IQueryable<Sessions> GetAll()
    {
        return _context.Sessions
            .Include(s => s.TimeSlot)
            .ThenInclude(mats => mats.Schedules)
            .AsSplitQuery();
    }

    public async Task<Sessions?> GetByIdAsync(Guid id)
    {
        return await _context.Sessions
            .Include(s => s.TimeSlot)
            .ThenInclude(mats => mats.Schedules)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public void MentorAcceptBookingSession(Sessions bookingSession, Guid learnerId)
    {
        var isBooked = bookingSession.TimeSlot.Sessions.Any(s => s.Status is SessionStatus.Approved or SessionStatus.Completed);
        if (isBooked)
        {
            throw new Exception("Cannot accept this booking session.");
        }

        bookingSession.Status = SessionStatus.Approved;
    }

    public void MentorCancelBookingSession(Sessions bookingSession, Guid learnerId)
    {
        var timeSlot = bookingSession.TimeSlot;
        if (bookingSession.Status is not SessionStatus.Pending)
        {
            throw new Exception("Cannot reject this booking session.");
        }

        var isBooked = timeSlot.Sessions.Any(s => s.Status is SessionStatus.Approved or SessionStatus.Completed);
        if (isBooked)
        {
            throw new Exception("Cannot reject this booking session.");
        }

        bookingSession.Status = SessionStatus.Cancelled;
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