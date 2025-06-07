using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class ActivityLogRepository(ApplicationDbContext context) : BaseRepository<ActivityLog, Guid>(context), IActivityLogRepository
{
    
}