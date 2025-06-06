namespace Contract.Dtos.AdminDashboard.Responses;

public class MonthlyApplicationReportResponse
{
    public string Month { get; set; } = null!;
    public int ApprovedApplications { get; set; }
    public int RejectedApplications { get; set; }
    public int TotalApplications { get; set; }
}