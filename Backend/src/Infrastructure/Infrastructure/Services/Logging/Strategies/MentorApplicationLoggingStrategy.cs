using Contract.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Logging.Strategies;

public class MentorApplicationLoggingStrategy(IServiceProvider serviceProvider) : IEntityLoggingStrategy
{
    public string GetLoggingAction(EntityEntry entry, User? user)
    {
        if (!IsLoggingState(entry))
            return string.Empty;

        var mentorApplication = (MentorApplication)entry.Entity;
        var mentorId = mentorApplication.MentorId;
        User? mentor = null;
        var userName = user?.FullName ?? $"{user!.Id} (not found)";

        if (entry.State == EntityState.Added)
        {
            return $"Mentor {userName} submitted a new mentor application";
        }

        if (user.RoleId == (int)UserRole.Mentor)
        {
            return $"Mentor {userName} updated their mentor application";
        }

        var currentStatus = mentorApplication.Status;

        var userRepository = serviceProvider.GetService<IUserRepository>();
        mentor = userRepository!.GetByIdAsync(mentorId).GetAwaiter().GetResult();
        var mentorName = mentor?.FullName ?? $"{mentorId} (not found)";

        switch (currentStatus)
        {
            case ApplicationStatus.WaitingInfo:
                return $"Admin {userName} request Mentor Application info for mentor {mentorName}. Note: {mentorApplication.Note}";
            case ApplicationStatus.Approved or ApplicationStatus.Rejected:
                return $"Admin {userName} {currentStatus.ToString()} Mentor Application for mentor {mentorName}. Note: {mentorApplication.Note}";
        }

        return "";
    }

    public bool IsLoggingState(EntityEntry entry)
    {
        return entry is { Entity: MentorApplication, State: EntityState.Added or EntityState.Modified };
    }
}