using Domain.Entities;

namespace Contract.Repositories;

public interface ISessionBookingRepository : IBaseRepository<Sessions, Guid>
{
    Task<Sessions?> GetByIdAsync(Guid id);
    void MentorAcceptBookingSession(Sessions sessionsSessions, Guid learnerId);
    void MentorCancelBookingSession(Sessions sessionsSessions, Guid learnerId);
}