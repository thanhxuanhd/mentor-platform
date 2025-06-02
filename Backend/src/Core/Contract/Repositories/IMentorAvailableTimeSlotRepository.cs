using Domain.Entities;

namespace Contract.Repositories;

public interface IMentorAvailableTimeSlotRepository : IBaseRepository<MentorAvailableTimeSlot, Guid>
{
    IQueryable<MentorAvailableTimeSlot> GetAvailableTimeSlot();
    IQueryable<MentorAvailableTimeSlot> GetAvailableMentorForBooking();

    Task<MentorAvailableTimeSlot?> GetByIdAsync(Guid id);
    Sessions AddNewBookingSession(MentorAvailableTimeSlot timeSlot, Guid learnerId);
}