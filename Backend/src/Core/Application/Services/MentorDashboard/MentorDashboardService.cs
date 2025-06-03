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
        var user = await userRepository.GetByIdAsync(mentorId);

        if (user == null)
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
        List<UpcomingSessionResponse> upcomingSessionsList = new();
        Console.WriteLine(upcomingSchedule.Id);
        if (upcomingSchedule != null)
        {
            foreach (var timeSlot in upcomingSchedule.AvailableTimeSlots)
            {
                foreach (var session in timeSlot.Sessions)
                {
                    if (session.Status == SessionStatus.Completed)
                    {
                        completedSessions++;
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
            UpcomingSessions = upcomingSessions,
            CompletedSessions = completedSessions,
            UpcomingSessionsList = upcomingSessionsList
        };

        return Result.Success(result, HttpStatusCode.OK);

    }
}

