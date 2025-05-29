using Domain.Entities;

namespace Contract.Repositories;

public interface ITimeSlotRepository : IBaseRepository<Schedule, Guid>
{
}
