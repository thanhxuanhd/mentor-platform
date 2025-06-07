using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services.Logging.Strategies;

public class UserLoggingStrategy : IEntityLoggingStrategy
{
    public bool IsLoggingState(EntityEntry entry)
    {
        return entry is { Entity: User, State: EntityState.Added or EntityState.Modified };
    }

    public string GetLoggingAction(EntityEntry entry, User? u)
    {
        if (!IsLoggingState(entry))
            return string.Empty;

        var user = (User)entry.Entity;
        var userId = user.Id;
        var fullName = user?.FullName ?? $"{user!.Id} (name not found)";
        var role = user.RoleId switch
        {
            (int)UserRole.Admin => nameof(UserRole.Admin),
            (int)UserRole.Mentor => nameof(UserRole.Mentor),
            (int)UserRole.Learner => nameof(UserRole.Learner),
            _ => throw new InvalidOperationException("Unrecognized role")
        };

        if (entry.State == EntityState.Added && user.RoleId == (int)UserRole.Admin)
        {
            return $"New User {fullName} registered to the system as role {role}";
        }

        if (entry.State != EntityState.Modified)
            return string.Empty;

        var originalRole = entry.OriginalValues.GetValue<int>("RoleId");
        var currentRole = entry.CurrentValues.GetValue<int>("RoleId");

        return originalRole != currentRole
            ? $"User {fullName} registered to the system as role {role}"
            : $"User {fullName} modified profile";
    }
}