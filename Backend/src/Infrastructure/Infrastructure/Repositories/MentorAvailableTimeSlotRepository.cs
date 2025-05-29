using Contract.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MentorAvailableTimeSlotRepository(ApplicationDbContext context)
    : BaseRepository<MentorAvailableTimeSlot, Guid>(context), IMentorAvailableTimeSlotRepository
{
    public IQueryable<MentorAvailableTimeSlot> GetAvailableTimeSlot()
    {
        var query = _context.TimeSlots
            .OrderBy(mats => mats.Id)
            .Where(mats => mats.Status == SessionStatus.Available)
            .Include(mats => mats.Mentor)
            .Where(mats => mats.Mentor.Status == UserStatus.Active);

        return query.AsQueryable();
    }

    public IQueryable<MentorAvailableTimeSlot> GetAvailableMentorForBooking()
    {
        var query = _context.TimeSlots
            .Where(mats => mats.Status == SessionStatus.Available)
            .Where(mats => mats.Mentor.Status == UserStatus.Active)
            .GroupBy(
                mats => mats.MentorId,
                mats => mats,
                (mentorId, timeSlots) => timeSlots
                    .OrderBy(ts => ts.StartTime)
                    .First()
            );

        return query.AsQueryable();
    }

    public async Task<MentorAvailableTimeSlot?> GetByIdAsync(Guid id)
    {
        return await _context.TimeSlots
            .Include(mats => mats.Mentor)
            .Include(mats => mats.Bookings)
            .FirstOrDefaultAsync(mt => mt.Id == id);
    }

    public Booking AddNewBookingSession(MentorAvailableTimeSlot timeSlot, Guid learnerId)
    {
        if (timeSlot.Status is not SessionStatus.Available)
        {
            throw new Exception("Cannot add new booking session.");
        }

        var bookingSession = new Booking
        {
            BookedDateTime = DateTime.Now,
            Status = BookingStatus.Pending,
            LearnerId = learnerId,
            TimeSlot = timeSlot
        };

        timeSlot.Bookings.Add(bookingSession);
        timeSlot.Status = SessionStatus.Processing;

        return bookingSession;
    }
}