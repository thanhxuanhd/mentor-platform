using Domain.Entities;

namespace Contract.Repositories;

public interface IMentorAvailableTimeSlotRepository : IBaseRepository<MentorAvailableTimeSlot, Guid>
{
    IQueryable<MentorAvailableTimeSlot> GetAvailableMentorsAsync();
    Task<MentorAvailableTimeSlot?> GetByIdAsync(Guid id);
    Booking AddNewBookingSession(MentorAvailableTimeSlot timeSlot, Guid learnerId);
}