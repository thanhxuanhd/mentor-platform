using Domain.Entities;

namespace Contract.Repositories;

public interface ISessionsRepository : IBaseRepository<Sessions, Guid>
{
    Task<Sessions?> GetByIdAsync(Guid id);
    void MentorAcceptBookingSession(Sessions bookingSession, Guid learnerId);
    void MentorCancelBookingSession(Sessions bookingSession, Guid learnerId);
    Task<IEnumerable<Sessions>> GetLearnerUpcomingSessionsAsync(Guid userId);
}