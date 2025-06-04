using Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services.Logging.Strategies;

public class CourseLoggingStrategy : IEntityLoggingStrategy
{
    public string GetLoggingAction(EntityEntry entry, User? user)
    {
        throw new NotImplementedException();
    }

    public bool IsLoggingState(EntityEntry entry)
    {
        throw new NotImplementedException();
    }
}