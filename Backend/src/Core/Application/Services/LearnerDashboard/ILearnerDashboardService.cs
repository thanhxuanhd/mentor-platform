using System;
using Contract.Dtos.LearnerDashboard.Responses;
using Contract.Shared;

namespace Application.Services.LearnerDashboard;

public interface ILearnerDashboardService
{
    Task<Result<GetLearnerDashboardResponse>> GetLearnerDashboardAsync(Guid userId);
}
