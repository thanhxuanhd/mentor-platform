using Domain.Entities;

namespace Contract.Repositories;

public interface ISessionsRepository : IBaseRepository<Sessions, Guid>
{
    Task<Sessions?> GetByIdAsync(Guid id);
    IQueryable<Sessions> GetSessionsByLearnerId(Guid learnerId);
    void MentorAcceptBookingSession(Sessions bookingSession, Guid learnerId);
    void MentorCancelBookingSession(Sessions bookingSession, Guid learnerId);
}