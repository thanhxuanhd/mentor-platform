namespace Contract.Dtos.LearnerDashboard.Responses;

public class GetLearnerDashboardResponse
{
    public IEnumerable<LearnerUpcomingSessionResponse> UpcomingSessions { get; set; } = new List<LearnerUpcomingSessionResponse>();
}

public class LearnerUpcomingSessionResponse
{
    public Guid SessionId { get; set; }
    public string MentorName { get; set; } = string.Empty;
    public string? MentorProfilePictureUrl { get; set; }
    public DateOnly ScheduledDate { get; set; }
    public string TimeRange { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
