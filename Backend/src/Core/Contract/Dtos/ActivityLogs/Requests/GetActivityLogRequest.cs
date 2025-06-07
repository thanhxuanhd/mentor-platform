namespace Contract.Dtos.ActivityLogs.Requests;

public record GetActivityLogRequest(DateTime? StartDateTime, DateTime? EndDateTime, string? Keyword, int PageSize, int PageIndex);