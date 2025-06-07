using System.Globalization;
using System.Net;
using Application.Helpers;
using Contract.Dtos.AdminDashboard.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Enums;

namespace Application.Services.AdminDashboards;

public class AdminDashboardService(
    IUserRepository userRepository,
    IMentorApplicationRepository mentorApplicationRepository,
    ISessionsRepository sessionRepository,
    ICourseResourceRepository resourceRepository,
    ICourseRepository courseRepository) : IAdminDashboardService
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

        var activeResources = resourceRepository.GetAll();

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

    public async Task<byte[]> GetMentorApplicationReportCurrentYearAsync()
    {
        var currentYear = DateTime.Now.Year;
        var currentMonth = DateTime.Now.Month;
        var approvedApps = mentorApplicationRepository.GetAll()
            .Where(ma => ma.Status == ApplicationStatus.Approved && ma.ReviewedAt.HasValue &&
                         ma.ReviewedAt.Value.Year == currentYear)
            .GroupBy(ma => ma.ReviewedAt!.Value.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() });

        var rejectedApps = mentorApplicationRepository.GetAll()
            .Where(ma => ma.Status == ApplicationStatus.Rejected && ma.ReviewedAt.HasValue &&
                         ma.ReviewedAt.Value.Year == currentYear)
            .GroupBy(ma => ma.ReviewedAt!.Value.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() });

        var totalApps = mentorApplicationRepository.GetAll()
            .Where(ma => ma.SubmittedAt.Year == currentYear)
            .GroupBy(ma => ma.SubmittedAt.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() });

        var monthlyApprovedApps = await mentorApplicationRepository.ToListAsync(approvedApps);
        var monthlyRejectedApps = await mentorApplicationRepository.ToListAsync(rejectedApps);
        var monthlyTotalApps = await mentorApplicationRepository.ToListAsync(totalApps);

        var monthlyCounts = Enumerable.Range(1, 12)
            .Where(month => month <= currentMonth)
            .Select(month => new MonthlyApplicationReportResponse 
        {
            Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
            TotalApplications = monthlyTotalApps.FirstOrDefault(a => a.Month == month)?.Count ?? 0,
            ApprovedApplications = monthlyApprovedApps.FirstOrDefault(a => a.Month == month)?.Count ?? 0, 
            RejectedApplications = monthlyRejectedApps.FirstOrDefault(a => a.Month == month)?.Count ?? 0
        }).ToList();

        return ExportExcelFileHelper.ExportToExcelAsync(monthlyCounts, $"{currentYear}_MentorApplicationReport");
    }

    public async Task<byte[]> GetMentorActivityReportAsync()
    {
        var mentors = userRepository.GetAll()
            .Where(u => u.RoleId == (int)UserRole.Mentor);

        var sessionCounts = sessionRepository.GetAll()
            .GroupBy(s => s.TimeSlot.Schedules.MentorId)
            .Select(g => new { MentorId = g.Key, Count = g.Count() });

        var courseCounts = courseRepository.GetAll()
            .GroupBy(c => c.MentorId)
            .Select(g => new { MentorId = g.Key, Count = g.Count() });

        var resourceCounts = resourceRepository.GetAll()
            .GroupBy(r => r.Course.MentorId)
            .Select(g => new { MentorId = g.Key, Count = g.Count() });

        var mentorList = await userRepository.ToListAsync(mentors);
        var totalSessions = await sessionRepository.ToListAsync(sessionCounts);
        var totalCourses = await courseRepository.ToListAsync(courseCounts);
        var totalResources = await resourceRepository.ToListAsync(resourceCounts);

        var mentorActivityReport = mentorList.Select(m => new MentorActivityReportResponse
        {
            MentorName = m.FullName,
            Email = m.Email,
            PhoneNumber = m.PhoneNumber!,
            Status = m.Status.ToString(),
            TotalSessions = totalSessions.FirstOrDefault(sc => sc.MentorId == m.Id)?.Count ?? 0,
            TotalCourses = totalCourses.FirstOrDefault(cc => cc.MentorId == m.Id)?.Count ?? 0,
            TotalResources = totalResources.FirstOrDefault(rc => rc.MentorId == m.Id)?.Count ?? 0
        }).ToList();

        return ExportExcelFileHelper.ExportToExcelAsync(mentorActivityReport, "MentorActivityReport");
    }
}