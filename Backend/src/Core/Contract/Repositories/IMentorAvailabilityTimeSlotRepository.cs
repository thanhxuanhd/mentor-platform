using Domain.Entities;

namespace Contract.Repositories;

public interface IMentorAvailabilityTimeSlotRepository : IBaseRepository<MentorAvailableTimeSlot, Guid>
{
    List<MentorAvailableTimeSlot> DeletePendingAndCancelledTimeSlots(Guid scheduleSettingsId);
    List<MentorAvailableTimeSlot> GetConfirmedTimeSlots(Guid scheduleSettingsId);
}
