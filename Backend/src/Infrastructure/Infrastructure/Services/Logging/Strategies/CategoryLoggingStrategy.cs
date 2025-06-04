using Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services.Logging.Strategies;

public class CategoryLoggingStrategy : IEntityLoggingStrategy
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