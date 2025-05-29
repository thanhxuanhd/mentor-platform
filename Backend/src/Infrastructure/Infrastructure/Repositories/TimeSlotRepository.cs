using Contract.Dtos.Timeslot.Request;
using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class TimeSlotRepository(ApplicationDbContext context) : BaseRepository<MentorAvailableTimeSlot, Guid>(context), ITimeSlotRepository
{
    public Task<SessionParametersDto> GetSessionParametersAsync(Guid mentorId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<object>> GetTimeSlotsByMentorAndDateRangeAsync(Guid mentorId, DateTime startOfWeek, DateTime endOfWeek)
    {
        throw new NotImplementedException();
    }

    public Task<WorkHoursDto> GetWorkHoursAsync(Guid mentorId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasBookedSessionsAsync(Guid mentorId)
    {
        throw new NotImplementedException();
    }

    public Task SaveWeeklyAvailabilityAsync(SaveWeeklyAvailabilityRequest request)
    {
        throw new NotImplementedException();
    }

    public Task UpdateSessionParametersAsync(UpdateSessionParametersRequest request)
    {
        throw new NotImplementedException();
    }

    public Task UpdateWorkHoursAsync(UpdateWorkHoursRequest request)
    {
        throw new NotImplementedException();
    }
}
