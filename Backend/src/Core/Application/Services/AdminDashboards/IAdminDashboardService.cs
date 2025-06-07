using Contract.Dtos.AdminDashboard.Responses;
using Contract.Shared;

namespace Application.Services.AdminDashboards;

public interface IAdminDashboardService
{
    Task<Result<AdminDashboardResponse>> GetAdminDashboardAsync();
    Task<byte[]> GetMentorActivityReportAsync();
    Task<byte[]> GetMentorApplicationReportCurrentYearAsync();
}