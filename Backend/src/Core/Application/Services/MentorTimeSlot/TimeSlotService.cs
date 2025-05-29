using Application.Services.Schedule;
using Contract.Dtos.Schedule.Responses;
using Contract.Dtos.Timeslot.Request;
using Contract.Dtos.Timeslot.Response;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using System.Net;

namespace Application.Services.MentorTimeSlot;

public class TimeSlotService(ITimeSlotRepository timeslotRepository) : ITimeSlotService
{

    public async Task AddAsync(MentorAvailableTimeSlot timeSlot)
    {
        await timeslotRepository.AddAsync(timeSlot);
    }

    public async Task<ApiResponse> CheckLockedStatusAsync(Guid mentorId)
    {
        try
        {
            var hasBookedSessions = await HasBookedSessionsAsync(mentorId);

            var result = new
            {
                IsLocked = hasBookedSessions,
                Message = hasBookedSessions
                    ? "Schedule is locked due to existing bookings. Contact admin to make changes."
                    : "Schedule is available for editing."
            };

            return ApiResponse.Ok(result, "Lock status retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.Error($"Error checking lock status: {ex.Message}");
        }
    }
   

    public async Task<MentorAvailableTimeSlot?> GetTimeSlotByIdAsync(Guid id)
    {
        return await timeslotRepository.GetByIdAsync(id);
    }

    //public async Task<ApiResponse> GetWeeklyCalendarAsync(DateTime weekStartDate, Guid mentorId)
    //{
    //    try
    //    {
    //        var startOfWeek = StartOfWeek(weekStartDate, DayOfWeek.Sunday);
    //        var endOfWeek = startOfWeek.AddDays(6);

    //        var timeSlots = await timeslotRepository.GetTimeSlotsByMentorAndDateRangeAsync(mentorId, startOfWeek, endOfWeek);

    //        var weekDays = Enumerable.Range(0, 7)
    //            .Select(offset =>
    //            {
    //                var date = startOfWeek.AddDays(offset);
    //                var slotsForDay = timeSlots
    //                    .Where(ts => ts.Date.Date == date.Date)
    //                    .Select(ts => new
    //                    {
    //                        Id = ts.Id,
    //                        StartTime = ts.StartTime.ToString("HH:mm"),
    //                        EndTime = ts.EndTime.ToString("HH:mm"),
    //                        IsBooked = ts.Bookings?.Any() == true,
    //                        IsSelected = false
    //                    }).ToList();

    //                return new
    //                {
    //                    FullDayName = date.ToString("dddd"),
    //                    ShortDayName = date.ToString("ddd"),
    //                    MonthDate = date.ToString("MMM dd"),
    //                    IsoDate = date.ToString("yyyy-MM-dd"),
    //                    IsToday = date.Date == DateTime.Today,
    //                    TimeBlocks = slotsForDay
    //                };
    //            }).ToList();

    //        var weekRangeDisplay = $"{startOfWeek.ToString("MMM dd")} - {endOfWeek.ToString("MMM dd")}";

    //        var result = new
    //        {
    //            WeekRange = weekRangeDisplay,
    //            Days = weekDays
    //        };

    //        return ApiResponse.Ok(result, "Weekly calendar retrieved successfully");
    //    }
    //    catch (Exception ex)
    //    {
    //        return ApiResponse.Error($"Error retrieving weekly calendar: {ex.Message}");
    //    }
    //}

    public async Task<ApiResponse> SaveWeeklyAvailabilityAsync(SaveWeeklyAvailabilityRequest request)
    {
        try
        {
            if (request == null || request.MentorId == Guid.Empty)
            {
                return ApiResponse.Error("Invalid request data");
            }

            var hasBookings = await HasBookedSessionsAsync(request.MentorId);
            if (hasBookings)
            {
                return ApiResponse.Forbidden("Cannot modify availability due to existing bookings. Contact admin for assistance.");
            }

            await timeslotRepository.SaveWeeklyAvailabilityAsync(request);

            return ApiResponse.Ok(null, "Weekly availability saved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.Error($"Error saving availability: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, TimeSlotRequest request)
    {
        var timeSlot = await timeslotRepository.GetByIdAsync(id);
        if (timeSlot == null)
        {
            return Result.Failure<bool>("TimeSlot not found", HttpStatusCode.NotFound);
        }

        timeSlot.MentorId = request.MentorId;
        timeSlot.StartTime = request.StartTime;
        timeSlot.EndTime = request.EndTime;

        timeslotRepository.Update(timeSlot);
        await timeslotRepository.SaveChangesAsync();

        return Result.Success(true, HttpStatusCode.OK);
    }

    public async Task<ApiResponse> UpdateSessionParametersAsync(UpdateSessionParametersRequest request)
    {
        try
        {
            if (request == null || request.MentorId == Guid.Empty)
            {
                return ApiResponse.Error("Invalid request data");
            }

            if (request.SessionDurationMinutes < 15)
            {
                return ApiResponse.Error("Session duration must be at least 15 minutes");
            }

            if (request.BufferTimeMinutes < 0)
            {
                return ApiResponse.Error("Buffer time must be at least 0 minutes");
            }

            var hasBookings = await HasBookedSessionsAsync(request.MentorId);
            if (hasBookings)
            {
                return ApiResponse.Forbidden("Cannot modify session parameters due to existing bookings. Contact admin for assistance.");
            }

            await timeslotRepository.UpdateSessionParametersAsync(request);

            return ApiResponse.Ok(null, "Session parameters updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.Error($"Error updating session parameters: {ex.Message}");
        }
    }

    public async Task<ApiResponse> UpdateWorkHoursAsync(UpdateWorkHoursRequest request)
    {
        try
        {
            if (request == null || request.MentorId == Guid.Empty)
            {
                return ApiResponse.Error("Invalid request data");
            }

            if (request.StartTime >= request.EndTime)
            {
                return ApiResponse.Error("End time must be after start time");
            }

            var hasBookings = await HasBookedSessionsAsync(request.MentorId);
            if (hasBookings)
            {
                return ApiResponse.Forbidden("Cannot modify work hours due to existing bookings. Contact admin for assistance.");
            }

            await timeslotRepository.UpdateWorkHoursAsync(request);

            return ApiResponse.Ok(null, "Work hours updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.Error($"Error updating work hours: {ex.Message}");
        }
    }

    public async Task<ApiResponse> GetWorkHoursAsync(Guid mentorId)
    {
        try
        {
            var workHours = await timeslotRepository.GetWorkHoursAsync(mentorId);
            return ApiResponse.Ok(workHours, "Work hours retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.Error($"Error retrieving work hours: {ex.Message}");
        }
    }

    public async Task<ApiResponse> GetSessionParametersAsync(Guid mentorId)
    {
        try
        {
            var sessionParams = await timeslotRepository.GetSessionParametersAsync(mentorId);
            return ApiResponse.Ok(sessionParams, "Session parameters retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.Error($"Error retrieving session parameters: {ex.Message}");
        }
    }

    public async Task<bool> HasBookedSessionsAsync(Guid mentorId)
    {
        return await timeslotRepository.HasBookedSessionsAsync(mentorId);
    }

    private static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    public Task UpdateAsync(Guid id, MentorAvailableTimeSlot timeSlot)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<GetTimeSlotResponse>>> GetAllAsync()
    {
        var timeslot = await timeslotRepository.ToListAsync(
           timeslotRepository.GetAll().Select(ts => new GetTimeSlotResponse
           {
               MentorId = ts.MentorId,
               ScheduleId = ts.ScheduleId,
               SessionId = ts.SessionId,
               StartTime = ts.StartTime, 
               EndTime = ts.EndTime,  
               Mentor = ts.Mentor,
               Bookings = ts.Bookings
           }));

        return Result.Success(timeslot, HttpStatusCode.OK);
    }

    public async Task<Result<GetTimeSlotResponse>> GetTimeslotByIdAsync(Guid id)
    {
        var timeslot = await timeslotRepository.GetByIdAsync(id);
        if (timeslot == null)
        {
            return Result.Failure<GetTimeSlotResponse>("Schedule not found", HttpStatusCode.NotFound);
        }

        var response = new GetTimeSlotResponse
        {
            MentorId = timeslot.MentorId,
            ScheduleId = timeslot.ScheduleId,
            SessionId = timeslot.SessionId,
            StartTime = timeslot.StartTime,
            EndTime = timeslot.EndTime,
            Mentor = timeslot.Mentor,
            Bookings = timeslot.Bookings
        };

        return Result.Success(response, HttpStatusCode.OK);
    }

}