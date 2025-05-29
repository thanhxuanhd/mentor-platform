using Contract.Dtos.Schedule.Requests;
using Contract.Dtos.Schedule.Responses;
using Contract.Shared;

namespace Application.Services.Schedule;

public interface IScheduleService
{
    Task<Result<GetScheduleResponse>> GetScheduleByIdAsync(Guid id);
    Task<Result<List<GetScheduleResponse>>> GetAllAsync();
    Task<Result<GetScheduleResponse>> CreateAsync(ScheduleRequest request);
    Task<Result<bool>> UpdateAsync(Guid id, ScheduleRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
}

