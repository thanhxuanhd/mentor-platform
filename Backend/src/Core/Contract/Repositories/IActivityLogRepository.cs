using Domain.Entities;

namespace Contract.Repositories;

public interface IActivityLogRepository : IBaseRepository<ActivityLog, Guid>
{
}