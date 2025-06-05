using System.Net;
using Contract.Dtos.LearnerDashboard.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;

namespace Application.Services.LearnerDashboard;

public class LearnerDashboardService(IUserRepository userRepository, ISessionsRepository sessionsRepository) : ILearnerDashboardService
{
	public async Task<Result<GetLearnerDashboardResponse>> GetLearnerDashboardAsync(Guid userId)
	{
		User? learner = await userRepository.GetByIdAsync(userId, c => c.Sessions!);

		if (learner == null)
		{
			return Result.Failure<GetLearnerDashboardResponse>("User not found", HttpStatusCode.NotFound);
		}

		var upcomingSessions = await sessionsRepository.GetLearnerUpcomingSessionsAsync(userId);

		var upcomingSession = new List<LearnerUpcomingSessionResponse>();
		foreach (var session in upcomingSessions)
		{
			upcomingSession.Add(new LearnerUpcomingSessionResponse
			{
				SessionId = session.Id,
				MentorName = session.TimeSlot.Schedules?.Mentor?.FullName ?? "Unknown Mentor",
				MentorProfilePictureUrl = session.TimeSlot.Schedules?.Mentor?.ProfilePhotoUrl,
				ScheduledDate = session.TimeSlot.Date,
				TimeRange = $"{session.TimeSlot.StartTime:HH:mm} - {session.TimeSlot.EndTime:HH:mm}",
				Type = session.Type.ToString()
			});
		}

		var result = new GetLearnerDashboardResponse
		{
			UpcomingSessions = upcomingSession
		};

		return Result.Success(result, HttpStatusCode.OK);
	}
}