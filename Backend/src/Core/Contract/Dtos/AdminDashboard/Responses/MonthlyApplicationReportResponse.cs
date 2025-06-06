namespace Contract.Dtos.AdminDashboard.Responses;

public class MonthlyApplicationReportResponse
{
    public int Month { get; set; }
    public int ApprovedApplications { get; set; }
    public int RejectedApplications { get; set; }
    public int TotalApplications { get; set; }
}