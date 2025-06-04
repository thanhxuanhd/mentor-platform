namespace Contract.Dtos.MentorDashboard.Responses;

public class GetMentorDashboardResponse
{
    public int TotalLearners { get; set; }
    public int TotalCourses { get; set; }
    public int UpcomingSessions { get; set; }
    public int CompletedSessions { get; set; }
    public List<UpcomingSessionResponse> UpcomingSessionsList { get; set; } = new List<UpcomingSessionResponse>();
}

public class UpcomingSessionResponse
{
    public Guid SessionId { get; set; }
    public string LearnerName { get; set; } = string.Empty;
    public DateOnly ScheduledDate { get; set; }
    public string TimeRange { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Assuming Type is a string representation of SessionType
}