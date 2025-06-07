using System.Net;
using Contract.Dtos.ActivityLogs.Requests;
using Contract.Dtos.ActivityLogs.Responses;
using Contract.Repositories;
using Contract.Shared;

namespace Application.Services.ActivityLogs;

public class ActivityLogService(IActivityLogRepository activityLogRepository) : IActivityLogService
{
    public async Task<Result<PaginatedList<GetActivityLogResponse>>> GetPaginatedActivityLogs(
        GetActivityLogRequest request)
    {
        var end = request.EndDateTime ?? DateTime.Now;
        var start = request.StartDateTime ?? DateTime.Now.Subtract(TimeSpan.FromDays(7));
        var keyword = request.Keyword ?? "";

        var activityLogs = activityLogRepository
            .GetAll()
            .Where(a => a.Action.Contains(keyword))
            .Where(a => a.Timestamp >= start && a.Timestamp <= end)
            .OrderByDescending(a => a.Timestamp)
            .Select(a => new GetActivityLogResponse(a.Id, a.Action, a.Timestamp));

        var paginatedList = await activityLogRepository.ToPaginatedListAsync(activityLogs, request.PageSize, request.PageIndex);

        return Result.Success(paginatedList, HttpStatusCode.OK);
    }
}