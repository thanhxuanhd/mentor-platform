using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services.Logging.Strategies;

public class CourseLoggingStrategy : IEntityLoggingStrategy
{
    public string GetLoggingAction(EntityEntry entry, User? user)
    {
        if (!IsLoggingState(entry))
            return string.Empty;

        var course = (Course)entry.Entity;
        var fullName = user!.FullName ?? "unknown";
        var role = user.RoleId switch
        {
            (int)UserRole.Admin => nameof(UserRole.Admin),
            (int)UserRole.Mentor => nameof(UserRole.Mentor),
            (int)UserRole.Learner => nameof(UserRole.Learner),
            _ => throw new InvalidOperationException("Unrecognized role")
        };

        if (user.RoleId != (int)UserRole.Mentor)
        {
            return $"Unauthorized user {user.Id} with role {role} make changes to the Course {course.Title}";
        }

        return entry.State switch
        {
            EntityState.Added => $"Mentor {fullName} added the new Course {course.Title} to the system",
            EntityState.Modified => $"Mentor {fullName} updated the Course {course.Title} to the system",
            _ => $"Mentor {fullName} deleted the new Course {course.Title} to the system"
        };
    }

    public bool IsLoggingState(EntityEntry entry)
    {
        return entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted;
    }
}