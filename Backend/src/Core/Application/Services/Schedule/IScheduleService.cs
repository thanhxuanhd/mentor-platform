using Contract.Dtos.Schedule.Requests;
using Contract.Dtos.Schedule.Responses;
using Contract.Shared;

namespace Application.Services.Schedule;

public interface IScheduleService
{
    // Task<Result<GetScheduleSettingsResponse>> GetScheduleByIdAsync(Guid id);
    // Task<Result<List<GetScheduleSettingsResponse>>> GetAllAsync();
    // Task<Result<ScheduleSettingsResponse>> CreateAsync(UpdateScheduleSettingsRequest request);
    Task<Result<ScheduleSettingsResponse>> GetScheduleSettingsAsync(Guid mentorId, GetScheduleSettingsRequest request);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateScheduleSettingsRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
}

