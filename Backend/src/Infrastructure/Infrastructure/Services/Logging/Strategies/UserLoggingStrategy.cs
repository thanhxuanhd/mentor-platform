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

    public string GetLoggingAction(EntityEntry entry, User? claimUser)
    {
        if (!IsLoggingState(entry))
            return string.Empty;

        var claimName = claimUser?.FullName ?? $"{claimUser!.Id} (name not found)";

        var entryUser = (User)entry.Entity;
        var entryName = entryUser?.FullName ?? $"{entryUser!.Id} (name not found)";
        var entryRole = GetRoleName(entryUser.RoleId);

        var originalRole = entry.OriginalValues.GetValue<int>("RoleId");
        var currentRole = entry.CurrentValues.GetValue<int>("RoleId");
        var originalStatus = entry.OriginalValues.GetValue<UserStatus>("Status");
        var currentStatus = entry.CurrentValues.GetValue<UserStatus>("Status");

        if (claimUser.RoleId == (int)UserRole.Admin)
        {
            if (entry.State == EntityState.Added)
            {
                return claimUser.Id == entryUser.Id
                    ? $"Admin {entryName} created their account."
                    : $"Admin {claimName} created a {entryRole} account. User {entryName}.";
            }

            if (originalRole != currentRole)
            {
                return $"Admin {claimName} modified account {entryName} from role {originalRole} to {currentRole}.";
            }

            return originalStatus != currentStatus 
                ? $"Admin {claimName} change the account {entryName} status from {originalStatus} to {currentStatus}." 
                : $"Admin {claimName} modified account {entryName}";
        }

        return originalRole != currentRole
            ? $"User {entryName} registered to the system as role {entryRole}"
            : $"User {entryName} modified profile";
    }

    private static string? GetRoleName(int roleId)
    {
        return roleId switch
        {
            (int)UserRole.Admin => nameof(UserRole.Admin),
            (int)UserRole.Mentor => nameof(UserRole.Mentor),
            (int)UserRole.Learner => nameof(UserRole.Learner),
            _ => null
        };
    }
}