using System.Diagnostics;
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
            .ThenInclude(s => s.Mentor)
            .Include(m => m.Learner)
            .ThenInclude(l => l.Sessions)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public void MentorAcceptBookingSession(Sessions bookingSession, Guid learnerId)
    {
        Debug.Assert(_context.Entry(bookingSession.TimeSlot).Reference(t => t.Sessions).IsLoaded);
        if (bookingSession.TimeSlot.Sessions.Any(s => s.Status is SessionStatus.Approved or SessionStatus.Completed))
        {
            throw new Exception("Cannot accept this booking session at this time.");
        }

        Debug.Assert(_context.Entry(bookingSession.Learner).Reference(t => t.Sessions).IsLoaded);
        if (bookingSession.Learner.Sessions!.Any(s => s.Id != bookingSession.Id && s.Status is SessionStatus.Approved))
        {
            throw new Exception("Cannot accept this booking session. The learner already has another ongoing session.");
        }

        bookingSession.Status = SessionStatus.Approved;
    }

    public void MentorCancelBookingSession(Sessions bookingSession, Guid learnerId)
    {
        Debug.Assert(_context.Entry(bookingSession.TimeSlot).Reference(t => t.Sessions).IsLoaded);
        if (bookingSession.Status is not SessionStatus.Pending)
        {
            throw new Exception("Cannot cancel this booking session at this time.");
        }

        bookingSession.Status = SessionStatus.Canceled;
    }
}