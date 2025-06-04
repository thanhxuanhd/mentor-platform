namespace Contract.Dtos.AdminDashboard.Responses;

public class AdminDashboardResponse
{
    public int TotalUsers { get; set; }
    public int TotalMentors { get; set; }
    public int TotalLearners { get; set; }
    public int TotalResources { get; set; }
    public int SessionsThisWeek { get; set; }
    public int PendingApplications { get; set; }
}