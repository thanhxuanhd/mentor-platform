using Domain.Entities;
using Domain.Enums;

namespace Contract.Repositories;

public interface ISessionsRepository : IBaseRepository<Sessions, Guid>
{
    Task<Sessions?> GetByIdAsync(Guid id);
    Sessions AddNewBookingSession(MentorAvailableTimeSlot timeSlot, SessionType sessionType, Guid learnerId);
    void CancelBookingSession(Sessions bookingSession, Guid learnerId);
    void MentorAcceptBookingSession(Sessions bookingSession, Guid learnerId);
}