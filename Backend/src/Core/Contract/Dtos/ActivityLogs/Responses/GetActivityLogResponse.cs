namespace Contract.Dtos.ActivityLogs.Responses;

public record GetActivityLogResponse(Guid Id, string Action, DateTime Timestamp);