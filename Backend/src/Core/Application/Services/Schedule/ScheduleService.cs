using Contract.Dtos.Scheduling.Requests;
using Contract.Dtos.Scheduling.Responses;
using Contract.Repositories;
using Contract.Shared;

namespace Application.Services.Schedule;

public class ScheduleService(IScheduleRepository scheduleRepository) : IScheduleService
{
    public async Task<Result<GetScheduleResponse>> GetScheduleByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<GetScheduleResponse>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<GetScheduleResponse>> CreateAsync(ScheduleRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, ScheduleRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
