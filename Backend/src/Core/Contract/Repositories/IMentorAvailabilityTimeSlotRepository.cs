using Domain.Entities;

namespace Contract.Repositories;

public interface IMentorAvailabilityTimeSlotRepository : IBaseRepository<MentorAvailableTimeSlot, Guid>
{
    void DeletePendingAndCancelledTimeSlots(Guid scheduleSettingsId);
    List<MentorAvailableTimeSlot> GetConfirmedTimeSlots(Guid scheduleSettingsId);
    Task<MentorAvailableTimeSlot?> GetByIdAsync(Guid id);
    IQueryable<MentorAvailableTimeSlot> GetAvailableTimeSlot();
    IQueryable<MentorAvailableTimeSlot> GetAvailableMentorForBooking();
}
