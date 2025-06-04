using Contract.Dtos.Schedule.Requests;
using Contract.Dtos.Schedule.Responses;
using Contract.Shared;

namespace Application.Services.Schedule;

public interface IScheduleService
{
    Task<Result<ScheduleSettingsResponse>> GetScheduleSettingsAsync(Guid mentorId, GetScheduleSettingsRequest request);
    Task<Result<SaveScheduleSettingsResponse>> SaveScheduleSettingsAsync(Guid id, SaveScheduleSettingsRequest request);
}

