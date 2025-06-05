using System.Net;
using Contract.Dtos.MentorDashboard.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.MentorDashboard;

public class MentorDashboardService(IUserRepository userRepository, IScheduleRepository scheduleRepository) : IMentorDashboardService
{
    public async Task<Result<GetMentorDashboardResponse>> GetMentorDashboardAsync(Guid mentorId)
    {
        User? mentor = await userRepository.GetByIdAsync(mentorId, c => c.Courses!);

        if (mentor == null)
        {
            return Result.Failure<GetMentorDashboardResponse>("User not found", HttpStatusCode.NotFound);
        }
        DateOnly weekStartDate;
        var today = DateTime.Now;
        int daysToSubtract = (int)today.DayOfWeek;
        weekStartDate = DateOnly.FromDateTime(today.AddDays(-daysToSubtract));

        DateOnly weekEndDate = weekStartDate.AddDays(6);

        Schedules? upcomingSchedule = await scheduleRepository.GetScheduleSettingsAsync(mentorId, weekStartDate, weekEndDate);

        int pendingSessions = 0;
        int completedSessions = 0;
        int upcomingSessions = 0;
        int totalCourses = mentor.Courses!.Count(c => c.Status == CourseStatus.Published);
        List<UpcomingSessionResponse> upcomingSessionsList = new();
        HashSet<Guid> uniqueLearners = new();
        var now = DateTime.Now;

        if (upcomingSchedule != null)
        {
            foreach (var timeSlot in upcomingSchedule.AvailableTimeSlots!)
            {
                foreach (var session in timeSlot.Sessions)
                {
                    if (session.Status == SessionStatus.Pending || session.Status == SessionStatus.Rescheduled)
                    {
                        pendingSessions++;
                        continue;
                    }
                    else if (session.Status == SessionStatus.Completed)
                    {
                        completedSessions++;
                        uniqueLearners.Add(session.LearnerId);
                    }
                    else if (session.Status == SessionStatus.Approved)
                    {
                        upcomingSessions++;
                        upcomingSessionsList.Add(new UpcomingSessionResponse
                        {
                            LearnerProfilePhotoUrl = session.Learner!.ProfilePhotoUrl ?? string.Empty,
                            SessionId = session.Id,
                            LearnerName = session.Learner?.FullName ?? "Unknown Learner",
                            ScheduledDate = timeSlot.Date,
                            TimeRange = $"{timeSlot.StartTime:HH:mm} - {timeSlot.EndTime:HH:mm}",
                            Type = session.Type.ToString()
                        });
                }
            }
        }

        var result = new GetMentorDashboardResponse
        {
            TotalPendingSessions = pendingSessions,
            TotalLearners = uniqueLearners.Count,
            TotalCourses = totalCourses,
            UpcomingSessions = upcomingSessions,
            CompletedSessions = completedSessions,
            UpcomingSessionsList = upcomingSessionsList.OrderByDescending(s => s.ScheduledDate).ThenBy(s => s.TimeRange)
        };

        return Result.Success(result, HttpStatusCode.OK);

    }

