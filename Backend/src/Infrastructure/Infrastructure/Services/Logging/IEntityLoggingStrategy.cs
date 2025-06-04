using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services.Logging;

public interface IEntityLoggingStrategy
{
    string GetLoggingAction(EntityEntry entry);
    bool IsLoggingState(EntityEntry entry);
}