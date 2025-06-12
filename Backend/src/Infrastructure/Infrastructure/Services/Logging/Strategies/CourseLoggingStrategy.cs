using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services.Logging.Strategies;

public class CourseLoggingStrategy : IEntityLoggingStrategy
{
    public string GetLoggingAction(EntityEntry entry, User? claimUser)
    {
        if (!IsLoggingState(entry))
            return string.Empty;

        var course = (Course)entry.Entity;
        var fullName = claimUser?.FullName ?? $"{claimUser!.Id} (name not found)";
        var role = claimUser.RoleId switch
        {
            (int)UserRole.Admin => nameof(UserRole.Admin),
            (int)UserRole.Mentor => nameof(UserRole.Mentor),
            (int)UserRole.Learner => nameof(UserRole.Learner),
            _ => throw new InvalidOperationException("Unrecognized role")
        };

        if (claimUser.RoleId != (int)UserRole.Mentor)
        {
            return $"Unauthorized user {fullName} with role {role} make changes to the Course {course.Title}";
        }

        return entry.State switch
        {
            EntityState.Added => $"Mentor {fullName} added the new Course {course.Title} to the system",
            EntityState.Modified => $"Mentor {fullName} updated the Course {course.Title} in the system",
            _ => $"Mentor {fullName} deleted the new Course {course.Title} from the system"
        };
    }

    public bool IsLoggingState(EntityEntry entry)
    {
        return entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted;
    }
}