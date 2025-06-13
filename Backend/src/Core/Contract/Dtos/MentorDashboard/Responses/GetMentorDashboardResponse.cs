namespace Contract.Dtos.MentorDashboard.Responses;

public class GetMentorDashboardResponse
{
    public int TotalPendingSessions { get; set; }
    public int TotalLearners { get; set; }
    public int TotalCourses { get; set; }
    public int UpcomingSessions { get; set; }
    public int CompletedSessions { get; set; }
    public IEnumerable<UpcomingSessionResponse> UpcomingSessionsList { get; set; } = new List<UpcomingSessionResponse>();
}

public class UpcomingSessionResponse
{
    public string? LearnerProfilePhotoUrl { get; set; }
    public Guid SessionId { get; set; }
    public string LearnerName { get; set; } = string.Empty;
    public DateOnly ScheduledDate { get; set; }
    public string TimeRange { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}