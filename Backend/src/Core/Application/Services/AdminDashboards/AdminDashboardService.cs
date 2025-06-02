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
            .Where(s => s.TimeSlot.Date >= startOfWeekDateOnly);

        var pendingApplications = mentorApplicationRepository.GetAll()
            .Where(ma => ma.Status == ApplicationStatus.Submitted);

        var totalAdmins = await userRepository.CountAsync(activeAdmins);
        var totalMentors = await userRepository.CountAsync(activeMentors);
        var totalLearners = await userRepository.CountAsync(activeLearners);
        var totalResources = await resourceRepository.CountAsync(activeResources);
        var totalSessionsThisWeek = await sessionRepository.CountAsync(sessionsThisWeek);
        var totalPendingApplications = await mentorApplicationRepository.CountAsync(pendingApplications);

        var response = new AdminDashboardResponse
        {
            TotalUsers = totalAdmins + totalLearners + totalMentors,
            TotalMentors = totalMentors,
            TotalLearners = totalLearners,
            TotalResources = totalResources,
            SessionsThisWeek = totalSessionsThisWeek,
            PendingApplications = totalPendingApplications
        };

        return Result.Success(response, HttpStatusCode.OK);
    }
}