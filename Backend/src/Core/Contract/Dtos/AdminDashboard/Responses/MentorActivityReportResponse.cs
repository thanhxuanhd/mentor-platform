namespace Contract.Dtos.AdminDashboard.Responses;

public class MentorActivityReportResponse
{
    public string MentorName { get; set; } = string.Empty;
    public string Status { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public int TotalResources { get; set; }
    public int TotalSessions { get; set; }
    public int TotalCourses { get; set; }
}