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

        var today = DateTime.UtcNow;
        int daysToSubtract = (int)today.DayOfWeek;
        var weekStartDate = DateOnly.FromDateTime(today.AddDays(-daysToSubtract));
        DateOnly weekEndDate = weekStartDate.AddDays(6);

        Schedules? upcomingSchedule = await scheduleRepository.GetScheduleSettingsAsync(mentorId, weekStartDate, weekEndDate);

        int pendingSessions = 0;
        int completedSessions = 0;
        int upcomingSessions = 0;
        int totalCourses = mentor.Courses!.Count();
        List<UpcomingSessionResponse> upcomingSessionsList = new();
        HashSet<Guid> uniqueLearners = new();
        var now = DateTime.UtcNow;
        DateOnly currentDate = DateOnly.FromDateTime(now);
        TimeOnly currentTime = TimeOnly.FromDateTime(now);

        if (upcomingSchedule != null)
        {
            foreach (var timeSlot in upcomingSchedule.AvailableTimeSlots!)
            {
                if (timeSlot.Date < currentDate || (timeSlot.Date == currentDate && timeSlot.EndTime < currentTime))
                {
                    continue;
                }
                foreach (var session in timeSlot.Sessions!)
                {
                    if (session.Status == SessionStatus.Approved)
                    {
                        upcomingSessions++;
                        upcomingSessionsList.Add(new UpcomingSessionResponse
                        {
                            LearnerProfilePhotoUrl = session.Learner!.ProfilePhotoUrl,
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

        IEnumerable<Schedules> allSchedules = await scheduleRepository.GetAllSchedulesAsync(mentorId);

        foreach (var schedule in allSchedules)
        {
            foreach (var timeSlot in schedule.AvailableTimeSlots!)
            {
                foreach (var session in timeSlot.Sessions!)
                {
                    if (currentDate > timeSlot.Date || (currentDate == timeSlot.Date && currentTime > timeSlot.EndTime))
                    {
                        if (session.Status == SessionStatus.Completed)
                        {
                            completedSessions++;
                            uniqueLearners.Add(session.Learner!.Id);
                        }
                        break;
                    }

                    if (session.Status is SessionStatus.Pending or SessionStatus.Rescheduled)
                    {
                        pendingSessions++;
                    }
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
            UpcomingSessionsList = upcomingSessionsList.OrderBy(s => s.ScheduledDate).ThenBy(s => s.TimeRange)
        };

        return Result.Success(result, HttpStatusCode.OK);
    }
}