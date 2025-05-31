using Contract.Dtos.Timeslot.Request;
using Domain.Entities;

namespace Contract.Repositories;

public interface ITimeSlotRepository : IBaseRepository<MentorAvailableTimeSlot, Guid>
{
    Task<SessionParametersDto> GetSessionParametersAsync(Guid mentorId);
    Task<IEnumerable<object>> GetTimeSlotsByMentorAndDateRangeAsync(Guid mentorId, DateTime startOfWeek, DateTime endOfWeek);
    Task<WorkHoursDto> GetWorkHoursAsync(Guid mentorId);
    Task<bool> HasBookedSessionsAsync(Guid mentorId);
    Task SaveWeeklyAvailabilityAsync(SaveWeeklyAvailabilityRequest request);
    Task UpdateSessionParametersAsync(UpdateSessionParametersRequest request);
    Task UpdateWorkHoursAsync(UpdateWorkHoursRequest request);
}

