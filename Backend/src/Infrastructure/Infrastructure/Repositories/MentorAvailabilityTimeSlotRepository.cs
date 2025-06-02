using Contract.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;

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
}
