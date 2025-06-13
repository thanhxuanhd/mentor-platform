using Contract.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Logging.Strategies;

public class MentorApplicationLoggingStrategy(IServiceScopeFactory serviceScopeFactory) : IEntityLoggingStrategy
{
    public string GetLoggingAction(EntityEntry entry, User? claimUser)
    {
        if (!IsLoggingState(entry))
            return string.Empty;

        var mentorApplication = (MentorApplication)entry.Entity;
        var mentorId = mentorApplication.MentorId;
        var userName = claimUser?.FullName ?? $"{claimUser!.Id} (not found)";

        if (entry.State == EntityState.Added)
        {
            return $"Mentor {userName} submitted a new mentor application";
        }

        if (claimUser.RoleId == (int)UserRole.Mentor)
        {
            return $"Mentor {userName} updated their mentor application";
        }

        var currentStatus = mentorApplication.Status;

        using var scope = serviceScopeFactory.CreateScope();
        var userRepository = scope.ServiceProvider.GetService<IUserRepository>();
        var mentor = userRepository!.GetByIdAsync(mentorId).GetAwaiter().GetResult();
        var mentorName = mentor?.FullName ?? $"{mentorId} (not found)";

        return currentStatus switch
        {
            ApplicationStatus.WaitingInfo =>
                $"Admin {userName} request Mentor Application info for mentor {mentorName}. Note: {mentorApplication.Note}",
            ApplicationStatus.Approved or ApplicationStatus.Rejected =>
                $"Admin {userName} {currentStatus.ToString()} Mentor Application for mentor {mentorName}. Note: {mentorApplication.Note}",
            _ => ""
        };
    }

    public bool IsLoggingState(EntityEntry entry)
    {
        return entry is { Entity: MentorApplication, State: EntityState.Added or EntityState.Modified };
    }
}