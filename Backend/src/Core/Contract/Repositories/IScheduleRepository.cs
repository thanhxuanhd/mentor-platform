using Domain.Entities;

namespace Contract.Repositories;

public interface IScheduleRepository : IBaseRepository<Schedules, Guid>
{
    Task<Schedules?> GetScheduleSettingsAsync(Guid mentorId, DateOnly weekStartDate, DateOnly weekEndDate);
}

