using System.Net;
using Contract.Dtos.AdminDashboard.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Enums;

namespace Application.Services.AdminDashboards;

public class AdminDashboardService(
    IUserRepository userRepository,
    IMentorApplicationRepository mentorApplicationRepository,
    ISessionsRepository sessionRepository,
    ICourseResourceRepository resourceRepository) : IAdminDashboardService
{
    public async Task<Result<AdminDashboardResponse>> GetAdminDashboardAsync()
    {
        var now = DateTime.Now;
        var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);
        var startOfWeekDateOnly = DateOnly.FromDateTime(startOfWeek);

        var activeUsersQuery = userRepository
            .GetAll()
            .Where(u => u.Status == UserStatus.Active);

        var activeMentors = activeUsersQuery
            .Where(u => u.RoleId == (int)UserRole.Mentor)
            .Where(u => u.MentorApplications.Any(ma => ma.Status == ApplicationStatus.Approved));

        var activeLearners = activeUsersQuery
            .Where(u => u.RoleId == (int)UserRole.Learner);

        var activeAdmins = activeUsersQuery
            .Where(u => u.RoleId == (int)UserRole.Admin);

        var activeResources = resourceRepository.GetAll()
            .Where(c => c.Course.Status != CourseStatus.Archived);

        var sessionsThisWeek = sessionRepository.GetAll()
            .Where(s => s.TimeSlot.Date >= startOfWeekDateOnly)
            .Where(s => s.Status == SessionStatus.Completed || s.Status == SessionStatus.Approved);

        var pendingApplications = mentorApplicationRepository.GetAll()
            .Where(ma => ma.Status == ApplicationStatus.Submitted);

        var topResourceTypes = activeResources
            .GroupBy(r => r.ResourceType)
            .Select(g => new ResourceTypeCountResponse
            {
                ResourceType = g.Key.ToString(),
                Count = g.Count()
            })
            .OrderByDescending(ft => ft.Count)
            .Take(2);

        var totalAdmins = await userRepository.CountAsync(activeAdmins);
        var totalMentors = await userRepository.CountAsync(activeMentors);
        var totalLearners = await userRepository.CountAsync(activeLearners);
        var totalResources = await resourceRepository.CountAsync(activeResources);
        var totalSessionsThisWeek = await sessionRepository.CountAsync(sessionsThisWeek);
        var totalPendingApplications = await mentorApplicationRepository.CountAsync(pendingApplications);
        var topResourceTypesList = await resourceRepository.ToListAsync(topResourceTypes);

        var response = new AdminDashboardResponse
        {
            TotalUsers = totalAdmins + totalLearners + totalMentors,
            TotalMentors = totalMentors,
            TotalLearners = totalLearners,
            TotalResources = totalResources,
            SessionsThisWeek = totalSessionsThisWeek,
            PendingApplications = totalPendingApplications,
            ResourceTypeCounts = topResourceTypesList
        };

        return Result.Success(response, HttpStatusCode.OK);
    }
}