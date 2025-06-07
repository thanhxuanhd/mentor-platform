using Contract.Dtos.MentorDashboard.Responses;
using Contract.Shared;

namespace Application.Services.MentorDashboard;

public interface IMentorDashboardService
{
    Task<Result<GetMentorDashboardResponse>> GetMentorDashboardAsync(Guid mentorId);
}
