using Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services.Logging;

public interface IEntityLoggingStrategy
{
    string GetLoggingAction(EntityEntry entry, User? user);
    bool IsLoggingState(EntityEntry entry);
}