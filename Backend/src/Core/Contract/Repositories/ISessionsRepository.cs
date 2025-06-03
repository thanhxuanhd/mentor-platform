using Domain.Entities;

namespace Contract.Repositories;

public interface ISessionsRepository : IBaseRepository<Sessions, Guid>
{
    IQueryable<Sessions> GetAllSessionsByTimeSlotId(Guid timeSlotId);
    Task<Sessions?> GetByIdAsync(Guid id);
    void MentorAcceptBookingSession(Sessions bookingSession, Guid learnerId);
    void MentorCancelBookingSession(Sessions bookingSession, Guid learnerId);
}