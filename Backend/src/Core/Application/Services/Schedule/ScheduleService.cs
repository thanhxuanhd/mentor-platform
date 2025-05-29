using Contract.Dtos.Schedule.Requests;
using Contract.Dtos.Schedule.Responses;
using Contract.Repositories;
using Contract.Shared;
using System.Net;

namespace Application.Services.Schedule;

public class ScheduleService(IScheduleRepository scheduleRepository) : IScheduleService
{
    public async Task<Result<GetScheduleResponse>> GetScheduleByIdAsync(Guid id)
    {
        var schedule = await scheduleRepository.GetByIdAsync(id);
        if (schedule == null)
        {
            return Result.Failure<GetScheduleResponse>("Schedule not found", HttpStatusCode.NotFound);
        }

        var response = new GetScheduleResponse
        {
            Id = schedule.Id,
            DayOfWeek = schedule.DayOfWeek,
            StartTime = schedule.StartTime.ToString("HH:mm"),
            EndTime = schedule.EndTime.ToString("HH:mm"),
            SessionDuration = schedule.SessionDuration,
            BufferTime = schedule.BufferTime,
            IsLocked = schedule.IsLocked
        };

        return Result.Success(response, HttpStatusCode.OK);
    }

    public async Task<Result<List<GetScheduleResponse>>> GetAllAsync()
    {
        var schedules = await scheduleRepository.ToListAsync(
            scheduleRepository.GetAll().Select(x => new GetScheduleResponse
            {
                Id = x.Id,
                DayOfWeek = x.DayOfWeek,
                StartTime = x.StartTime.ToString("HH:mm"),
                EndTime = x.EndTime.ToString("HH:mm"),
                SessionDuration = x.SessionDuration,
                BufferTime = x.BufferTime,
                IsLocked = x.IsLocked
            }));

        return Result.Success(schedules, HttpStatusCode.OK);
    }

    public async Task<Result<GetScheduleResponse>> CreateAsync(ScheduleRequest request)
    {
        var schedule = new Domain.Entities.Schedule
        {
            Id = Guid.NewGuid(),
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SessionDuration = request.SessionDuration,
            BufferTime = request.BufferTime,
            IsLocked = request.IsLocked
        };

        await scheduleRepository.AddAsync(schedule);
        await scheduleRepository.SaveChangesAsync();

        var response = new GetScheduleResponse
        {
            Id = schedule.Id,
            DayOfWeek = schedule.DayOfWeek,
            StartTime = schedule.StartTime.ToString("HH:mm"),
            EndTime = schedule.EndTime.ToString("HH:mm"),
            SessionDuration = schedule.SessionDuration,
            BufferTime = schedule.BufferTime,
            IsLocked = schedule.IsLocked
        };

        return Result.Success(response, HttpStatusCode.Created);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, ScheduleRequest request)
    {
        var schedule = await scheduleRepository.GetByIdAsync(id);
        if (schedule == null)
        {
            return Result.Failure<bool>("Schedule not found", HttpStatusCode.NotFound);
        }

        schedule.DayOfWeek = request.DayOfWeek;
        schedule.StartTime = request.StartTime;
        schedule.EndTime = request.EndTime;
        schedule.SessionDuration = request.SessionDuration;
        schedule.BufferTime = request.BufferTime;
        schedule.IsLocked = request.IsLocked;

        scheduleRepository.Update(schedule);
        await scheduleRepository.SaveChangesAsync();

        return Result.Success(true, HttpStatusCode.OK);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var schedule = await scheduleRepository.GetByIdAsync(id);
        if (schedule == null)
        {
            return Result.Failure<bool>("Schedule not found", HttpStatusCode.NotFound);
        }

        schedule.IsLocked = true;
        scheduleRepository.Update(schedule);
        await scheduleRepository.SaveChangesAsync();

        return Result.Success(true, HttpStatusCode.OK);
    }
}
