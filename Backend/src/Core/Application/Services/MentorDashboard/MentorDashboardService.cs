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
        User mentor = await userRepository.GetByIdAsync(mentorId, c => c.);

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

        int completedSessions = 0;
        int upcomingSessions = 0;
        int totalCourses = mentor.Courses!.Count(c => c.Status == CourseStatus.Published);
        List<UpcomingSessionResponse> upcomingSessionsList = new();
        HashSet<Guid> uniqueLearners = new();
        if (upcomingSchedule != null)
        {
            foreach (var timeSlot in upcomingSchedule.AvailableTimeSlots)
            {
                foreach (var session in timeSlot.Sessions)
                {
                    if (session.Status == SessionStatus.Completed)
                    {
                        completedSessions++;
                        uniqueLearners.Add(session.LearnerId);
                    }
                    else if (session.Status == SessionStatus.Approved)
                    {
                        upcomingSessions++;
                        upcomingSessionsList.Add(new UpcomingSessionResponse
                        {
                            SessionId = session.Id,
                            LearnerName = session.Learner?.FullName ?? "Unknown Learner",
                            ScheduledDate = timeSlot.Date,
                            TimeRange = $"{timeSlot.StartTime:HH:mm} - {timeSlot.EndTime:HH:mm}",
                            Type = session.Type.ToString()
                        });
                    }
                }
            }
        }

        var result = new GetMentorDashboardResponse
        {
            TotalLearners = uniqueLearners.Count,
            TotalCourses = totalCourses,
            UpcomingSessions = upcomingSessions,
            CompletedSessions = completedSessions,
            UpcomingSessionsList = upcomingSessionsList
        };

        return Result.Success(result, HttpStatusCode.OK);

    }
}

