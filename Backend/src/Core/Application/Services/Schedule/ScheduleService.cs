using Contract.Dtos.Schedule.Requests;
using Contract.Dtos.Schedule.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using System.Net;

namespace Application.Services.Schedule;

public class ScheduleService(IScheduleRepository scheduleRepository, IUserRepository userRepository) : IScheduleService
{
    // TODO: possibly delete these methods
    // public async Task<Result<GetScheduleSettingsResponse>> GetScheduleByIdAsync(Guid id)
    // {
    //     var schedule = await scheduleRepository.GetByIdAsync(id);
    //     if (schedule == null)
    //     {
    //         return Result.Failure<GetScheduleSettingsResponse>("Schedule not found", HttpStatusCode.NotFound);
    //     }

    //     var response = new GetScheduleSettingsResponse
    //     {
    //         WeekStartDate = schedule.WeekStartDate,
    //         WeekEndDate = schedule.WeekEndDate,
    //         StartTime = schedule.StartTime.ToString("HH:mm"),
    //         EndTime = schedule.EndTime.ToString("HH:mm"),
    //         SessionDuration = schedule.SessionDuration,
    //         BufferTime = schedule.BufferTime,
    //         IsLocked = schedule.IsLocked
    //     };

    //     return Result.Success(response, HttpStatusCode.OK);
    // }

    // public async Task<Result<List<GetScheduleSettingsResponse>>> GetAllAsync()
    // {
    //     var schedules = await scheduleRepository.ToListAsync(
    //         scheduleRepository.GetAll().Select(x => new GetScheduleSettingsResponse
    //         {
    //             WeekStartDate = x.WeekStartDate,
    //             WeekEndDate = x.WeekEndDate,
    //             StartTime = x.StartTime.ToString("HH:mm"),
    //             EndTime = x.EndTime.ToString("HH:mm"),
    //             SessionDuration = x.SessionDuration,
    //             BufferTime = x.BufferTime,
    //             IsLocked = x.IsLocked
    //         }));

    //     return Result.Success(schedules, HttpStatusCode.OK);
    // }

    // public async Task<Result<GetScheduleSettingsResponse>> CreateAsync(CreateScheduleSettingsRequest request)
    // {
    //     var existingScheduleSettings = await scheduleRepository.GetScheduleSettingsAsync(request.MentorId, request.WeekStartDate, request.WeekEndDate);

    //     Schedules schedule;

    //     if (existingScheduleSettings != null)
    //     {

    //     }

    //     schedule = new Schedules
    //     {
    //         Id = Guid.NewGuid(),
    //         WeekStartDate = request.WeekStartDate,
    //         WeekEndDate = request.WeekEndDate,
    //         StartTime = request.StartTime,
    //         EndTime = request.EndTime,
    //         SessionDuration = request.SessionDuration,
    //         BufferTime = request.BufferTime,
    //     };

    //     await scheduleRepository.AddAsync(schedule);
    //     await scheduleRepository.SaveChangesAsync();

    //     var response = new GetScheduleSettingsResponse
    //     {
    //         WeekStartDate = schedule.WeekStartDate,
    //         WeekEndDate = schedule.WeekEndDate,
    //         StartTime = schedule.StartTime.ToString("HH:mm"),
    //         EndTime = schedule.EndTime.ToString("HH:mm"),
    //         SessionDuration = schedule.SessionDuration,
    //         BufferTime = schedule.BufferTime,
    //         IsLocked = schedule.IsLocked
    //     };

    //     return Result.Success(response, HttpStatusCode.Created);
    // }

    public async Task<Result<bool>> UpdateAsync(Guid id, CreateScheduleSettingsRequest request)
    {
        var schedule = await scheduleRepository.GetByIdAsync(id);
        if (schedule == null)
        {
            return Result.Failure<bool>("Schedule not found", HttpStatusCode.NotFound);
        }

        schedule.WeekStartDate = request.WeekStartDate;
        schedule.WeekEndDate = request.WeekEndDate;
        schedule.StartTime = request.StartTime;
        schedule.EndTime = request.EndTime;
        schedule.SessionDuration = request.SessionDuration;
        schedule.BufferTime = request.BufferTime;

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

    public async Task<Result<GetScheduleSettingsResponse>> GetScheduleSettingsAsync(GetScheduleSettingsRequest request)
    {
        var mentor = await userRepository.GetByIdAsync(request.MentorId);
        if (mentor == null)
        {
            return Result.Failure<GetScheduleSettingsResponse>("Mentor not found", HttpStatusCode.NotFound);
        }

        DateOnly weekStartDate;

        if (request.WeekStartDate.HasValue)
        {
            weekStartDate = request.WeekStartDate.Value;
        }
        else
        {
            var today = DateTime.Now;
            int daysToSubtract = (int)today.DayOfWeek;
            weekStartDate = DateOnly.FromDateTime(today.AddDays(-daysToSubtract));
        }

        DateOnly weekEndDate = request.WeekEndDate.HasValue ? request.WeekEndDate.Value : weekStartDate.AddDays(6);

        Schedules? scheduleSettings = await scheduleRepository.GetScheduleSettingsAsync(request.MentorId, weekStartDate, weekEndDate);
        if (scheduleSettings == null)
        {
            scheduleSettings = new Schedules
            {
                WeekStartDate = weekStartDate,
                WeekEndDate = weekEndDate,
                StartTime = new TimeOnly(09, 00),
                EndTime = new TimeOnly(17, 00),
                SessionDuration = 60,
                BufferTime = 15,
                IsLocked = false
            };
        }

        var response = new GetScheduleSettingsResponse
        {
            WeekStartDate = scheduleSettings.WeekStartDate,
            WeekEndDate = scheduleSettings.WeekEndDate,
            StartTime = scheduleSettings.StartTime.ToString("HH:mm"),
            EndTime = scheduleSettings.EndTime.ToString("HH:mm"),
            SessionDuration = scheduleSettings.SessionDuration,
            BufferTime = scheduleSettings.BufferTime,
            IsLocked = scheduleSettings.IsLocked
        };
        
        return Result.Success(response, HttpStatusCode.OK);
    }
}
