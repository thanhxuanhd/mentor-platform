using Domain.Entities;

namespace Contract.Repositories;

public interface IMentorAvailabilityTimeSlotRepository : IBaseRepository<MentorAvailableTimeSlot, Guid>
{
    void DeletePendingAndCancelledTimeSlots(Guid scheduleSettingsId);
    List<MentorAvailableTimeSlot> GetConfirmedTimeSlots(Guid scheduleSettingsId);
}
