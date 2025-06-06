using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services.Logging.Strategies;

public class MentorApplicationLoggingStrategy : IEntityLoggingStrategy
{
    public string GetLoggingAction(EntityEntry entry, User? user)
    {
        if (!IsLoggingState(entry))
            return string.Empty;

        var mentorApplication = (MentorApplication)entry.Entity;
        if (user?.RoleId is (int)UserRole.Admin)
        {
            return "";
        }

        return "";
    }

    public bool IsLoggingState(EntityEntry entry)
    {
        return entry is { Entity: MentorApplication, State: EntityState.Added or EntityState.Modified };
    }
}