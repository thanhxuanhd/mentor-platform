using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services.Logging.Strategies;

public class CategoryLoggingStrategy : IEntityLoggingStrategy
{
    public string GetLoggingAction(EntityEntry entry, User? user)
    {
        if (!IsLoggingState(entry))
            return string.Empty;

        var category = (Category)entry.Entity;
        var role = user!.RoleId switch
        {
            (int)UserRole.Admin => nameof(UserRole.Admin),
            (int)UserRole.Mentor => nameof(UserRole.Mentor),
            (int)UserRole.Learner => nameof(UserRole.Learner),
            _ => throw new InvalidOperationException("Unrecognized role")
        };

        if (user.RoleId != (int)UserRole.Admin)
        {
            return $"Unauthorized user {user.Id} with role {role} make changes to the Category {category.Name}";
        }

        return entry.State switch
        {
            EntityState.Added => $"Admin {user.Id} added the new Category {category.Name} to the system",
            EntityState.Modified => $"Admin {user.Id} updated the Category {category.Name} to the system",
            _ => $"Admin {user.Id} deleted the new Category {category.Name} to the system"
        };
    }

    public bool IsLoggingState(EntityEntry entry)
    {
        return entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted;
    }
}