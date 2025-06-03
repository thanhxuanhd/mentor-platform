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
        var timeSlot = bookingSession.TimeSlot;
        if (bookingSession.Status is not SessionStatus.Pending)
        {
            throw new Exception("Cannot accept this booking session.");
        }

        bookingSession.Status = SessionStatus.Confirmed;
        timeSlot.Status = SessionStatus.Confirmed;
    }

    public void MentorCancelBookingSession(Sessions bookingSession, Guid learnerId)
    {
        var timeSlot = bookingSession.TimeSlot;
        if (timeSlot.Status is not SessionStatus.Processing)
        {
            throw new Exception("Cannot reject this booking session.");
        }

        bookingSession.Status = BookingStatus.Rejected;
        timeSlot.Status = SessionStatus.Available;
    }
}