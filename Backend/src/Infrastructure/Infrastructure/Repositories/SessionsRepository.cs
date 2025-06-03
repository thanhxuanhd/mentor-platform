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
    public async Task<Sessions?> GetByIdAsync(Guid id)
    {
        return await _context.Sessions
            .Include(b => b.TimeSlot)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public void MentorAcceptBookingSession(Sessions bookingSession, Guid learnerId)
    {
        var sessions = bookingSession.TimeSlot.Sessions;
        if (sessions.Any(s => s.Status is SessionStatus.Approved or SessionStatus.Completed))
        {
            throw new Exception("Cannot accept this booking session.");
        }

        bookingSession.Status = SessionStatus.Approved;
        sessions.Add(bookingSession);
    }

    public void MentorCancelBookingSession(Sessions bookingSession, Guid learnerId)
    {
        var sessionFinalized = bookingSession.TimeSlot.Sessions.Any(s => s.Status is SessionStatus.Approved or SessionStatus.Completed);
        if (bookingSession.Status is not SessionStatus.Pending || sessionFinalized)
        {
            throw new Exception("Cannot reject this booking session.");
        }

        bookingSession.Status = SessionStatus.Canceled;
    }
}