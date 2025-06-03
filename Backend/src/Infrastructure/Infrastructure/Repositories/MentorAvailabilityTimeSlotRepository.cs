using Contract.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MentorAvailabilityTimeSlotRepository(ApplicationDbContext context) : BaseRepository<MentorAvailableTimeSlot, Guid>(context), IMentorAvailabilityTimeSlotRepository
{
    public void DeletePendingAndCancelledTimeSlots(Guid scheduleSettingsId)
    {
        var deleteTimeSlots = _context.MentorAvailableTimeSlots
            .Where(x => x.ScheduleId == scheduleSettingsId && !x.Sessions!.Any(s => s.Status == SessionStatus.Approved || s.Status == SessionStatus.Completed || s.Status == SessionStatus.Rescheduled));

        _context.MentorAvailableTimeSlots.RemoveRange(deleteTimeSlots);
    }
    
    public List<MentorAvailableTimeSlot> GetConfirmedTimeSlots(Guid scheduleSettingsId)
    {
        var confirmedTimeSlots = _context.MentorAvailableTimeSlots
            .Where(x => x.ScheduleId == scheduleSettingsId && x.Sessions!.Any(s => s.Status == SessionStatus.Approved || s.Status == SessionStatus.Completed || s.Status == SessionStatus.Rescheduled))
            .ToList();

        return confirmedTimeSlots;
    }
    
    public IQueryable<MentorAvailableTimeSlot> GetAvailableTimeSlot()
    {
        var query = _context.MentorAvailableTimeSlots
            .OrderBy(mats => mats.Id)
            .Include(mats => mats.Sessions)
            .Include(mats => mats.Schedules)
            .ThenInclude(mats => mats.Mentor)
            .Where(mats => mats.Sessions.All(sessions =>
                sessions.Status != SessionStatus.Approved && sessions.Status != SessionStatus.Completed));

        return query.AsQueryable();
    }

    public IQueryable<MentorAvailableTimeSlot> GetAvailableMentorForBooking()
    {
        var query = _context.MentorAvailableTimeSlots
            .Include(mats => mats.Schedules)
            .Include(mats => mats.Sessions)
            .Where(mats => mats.Sessions.All(sessions => sessions.Status != SessionStatus.Approved && sessions.Status != SessionStatus.Completed))
            .GroupBy(
                mats => mats.Schedules.MentorId,
                mats => mats,
                (mentorId, mentorAvailableTimeSlots) => mentorAvailableTimeSlots
                    .OrderBy(ts => ts.StartTime)
                    .First()
            );

        return query.AsQueryable();
    }

    public async Task<MentorAvailableTimeSlot?> GetByIdAsync(Guid id)
    {
        return await _context.MentorAvailableTimeSlots
            .Include(mats => mats.Schedules)
            .Include(mats => mats.Sessions)
            .FirstOrDefaultAsync(mt => mt.Id == id);
    }

    public Sessions AddNewBookingSession(MentorAvailableTimeSlot timeSlot, Guid learnerId)
    {
        if (timeSlot.Sessions.Any(sessions => sessions.Status is SessionStatus.Approved or SessionStatus.Completed))
        {
            throw new Exception("Cannot add new booking session.");
        }

        var bookingSession = new Sessions
        {
            Status = SessionStatus.Pending,
            LearnerId = learnerId,
            TimeSlot = timeSlot
        };

        timeSlot.Sessions.Add(bookingSession);

        return bookingSession;
    }
}
