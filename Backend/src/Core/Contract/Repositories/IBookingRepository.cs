using Domain.Entities;

namespace Contract.Repositories;

public interface IBookingRepository : IBaseRepository<Booking, Guid>
{
    Task<Booking?> GetByIdAsync(Guid id);
    void MentorAcceptBookingSession(Booking bookingSession, Guid learnerId);
    void MentorCancelBookingSession(Booking bookingSession, Guid learnerId);
}