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
    public IQueryable<MentorAvailableTimeSlot> GetAvailableMentorsAsync()
    {
        var query = _context.TimeSlots
            .OrderBy(mats => mats.Id)
            .Where(mats => mats.Status == SessionStatus.Available)
            .Include(mats => mats.Mentor)
            .ThenInclude(u => u.UserExpertises)
            .ThenInclude(ue => ue.Expertise)
            .Where(mats => mats.Mentor.Status == UserStatus.Active)
            .Include(mats => mats.Schedule);

        return query.AsSplitQuery().AsQueryable();
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
            Status = BookingStatus.Pending,
            LearnerId = learnerId,
            TimeSlot = timeSlot
        };

        timeSlot.Bookings.Add(bookingSession);
        timeSlot.Status = SessionStatus.Processing;

        return bookingSession;
    }
}