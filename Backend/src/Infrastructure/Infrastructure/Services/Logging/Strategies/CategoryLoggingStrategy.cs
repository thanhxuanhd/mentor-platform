using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services.Logging.Strategies;

public class CategoryLoggingStrategy : IEntityLoggingStrategy
{
    public string GetLoggingAction(EntityEntry entry, User? claimUser)
    {
        if (!IsLoggingState(entry))
            return string.Empty;

        var category = (Category)entry.Entity;
        var role = claimUser!.RoleId switch
        {
            (int)UserRole.Admin => nameof(UserRole.Admin),
            (int)UserRole.Mentor => nameof(UserRole.Mentor),
            (int)UserRole.Learner => nameof(UserRole.Learner),
            _ => throw new InvalidOperationException("Unrecognized role")
        };
        var fullName = claimUser?.FullName ?? $"{claimUser!.Id} (name not found)";

        if (claimUser.RoleId != (int)UserRole.Admin)
        {
            return $"Unauthorized user {fullName} with role {role} make changes to the Category {category.Name}";
        }

        return entry.State switch
        {
            EntityState.Added => $"Admin {fullName} added the new Category {category.Name} to the system",
            EntityState.Modified => $"Admin {fullName} updated the Category {category.Name} in the system",
            _ => $"Admin {fullName} deleted the Category {category.Name} from the system"
        };
    }

    public bool IsLoggingState(EntityEntry entry)
    {
        return entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted;
    }
}