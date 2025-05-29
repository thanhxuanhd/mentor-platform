using Contract.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BookingRepository(ApplicationDbContext context)
    : BaseRepository<Booking, Guid>(context), IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Bookings
            .Include(b => b.TimeSlot)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public void MentorAcceptBookingSession(Booking bookingSession, Guid learnerId)
    {
        var timeSlot = bookingSession.TimeSlot;
        if (timeSlot.Status is not SessionStatus.Processing)
        {
            throw new Exception("Cannot accept this booking session.");
        }

        bookingSession.Status = BookingStatus.Accepted;
        timeSlot.Status = SessionStatus.Confirmed;
    }

    public void MentorCancelBookingSession(Booking bookingSession, Guid learnerId)
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