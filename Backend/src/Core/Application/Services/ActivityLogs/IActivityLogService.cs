using Contract.Dtos.ActivityLogs.Requests;
using Contract.Dtos.ActivityLogs.Responses;
using Contract.Shared;

namespace Application.Services.ActivityLogs;

public interface IActivityLogService
{
    Task<Result<PaginatedList<GetActivityLogResponse>>> GetPaginatedActivityLogs(
        GetActivityLogRequest request);
}