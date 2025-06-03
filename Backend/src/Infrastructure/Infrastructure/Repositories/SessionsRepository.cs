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
    public IQueryable<Sessions> GetAllSessionsByTimeSlotId(Guid timeSlotId)
    {
        return _context.Sessions
            .Include(s => s.TimeSlot)
            .ThenInclude(mats => mats.Schedules)
            .Where(s => s.TimeSlotId == timeSlotId);
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
        var timeSlot = bookingSession.TimeSlot;
        var isBooked = timeSlot.SessionId != Guid.Empty
                       || timeSlot.Sessions.Any(s => s.Status is SessionStatus.Approved or SessionStatus.Completed);
        if (isBooked)
        {
            throw new Exception("Cannot accept this booking session.");
        }

        timeSlot.ScheduleId = bookingSession.Id;
        bookingSession.Status = SessionStatus.Approved;
    }

    public void MentorCancelBookingSession(Sessions bookingSession, Guid learnerId)
    {
        var timeSlot = bookingSession.TimeSlot;
        if (bookingSession.Status is not SessionStatus.Pending)
        {
            throw new Exception("Cannot reject this booking session.");
        }

        var isBooked = timeSlot.SessionId != Guid.Empty
                       || timeSlot.Sessions.Any(s => s.Status is SessionStatus.Approved or SessionStatus.Completed);
        if (isBooked)
        {
            throw new Exception("Cannot reject this booking session.");
        }

        bookingSession.Status = SessionStatus.Canceled;
    }
}