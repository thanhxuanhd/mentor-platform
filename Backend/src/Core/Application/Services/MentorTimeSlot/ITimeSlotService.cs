using Contract.Dtos.Schedule.Responses;
using Contract.Dtos.Timeslot.Request;
using Contract.Dtos.Timeslot.Response;
using Contract.Shared;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Application.Services.MentorTimeSlot;

public interface ITimeSlotService
{
    //Task<List<MentorAvailableTimeSlot>> GenerateTimeSlotsFromScheduleAsync(Guid mentorId, DateOnly fromDate, DateOnly toDate);
    //Task<List<MentorAvailableTimeSlot>> GetAvailableSlotsAsync(Guid mentorId, DateOnly date);
    Task<Result<GetTimeSlotResponse>> GetTimeslotByIdAsync(Guid id);
    Task<Result<List<GetTimeSlotResponse>>> GetAllAsync();
    Task AddAsync(MentorAvailableTimeSlot timeSlot);
    Task UpdateAsync(Guid id, MentorAvailableTimeSlot timeSlot);
    //Task DeleteAsync(Guid id);

    //Task<ApiResponse> GetWeeklyCalendarAsync(DateTime weekStartDate, Guid mentorId);
    //Task<ApiResponse> GetTimeSlotsByDateAsync(DateTime date, Guid mentorId);
    Task<ApiResponse> SaveWeeklyAvailabilityAsync(SaveWeeklyAvailabilityRequest request);
    Task<ApiResponse> UpdateWorkHoursAsync(UpdateWorkHoursRequest request);
    Task<ApiResponse> UpdateSessionParametersAsync(UpdateSessionParametersRequest request);
    Task<ApiResponse> CheckLockedStatusAsync(Guid mentorId);

    Task<ApiResponse> GetWorkHoursAsync(Guid mentorId);
    Task<ApiResponse> GetSessionParametersAsync(Guid mentorId);
    Task<bool> HasBookedSessionsAsync(Guid mentorId);
}

public class ApiResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }

    public static ApiResponse Ok(object? data = null, string? message = null) =>
        new ApiResponse { StatusCode = HttpStatusCode.OK, Success = true, Message = message, Data = data };

    public static ApiResponse Error(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) =>
        new ApiResponse { StatusCode = statusCode, Success = false, Message = message };

    public static ApiResponse NotFound(string message = "Resource not found") =>
        new ApiResponse { StatusCode = HttpStatusCode.NotFound, Success = false, Message = message };

    public static ApiResponse Forbidden(string message = "Access forbidden") =>
        new ApiResponse { StatusCode = HttpStatusCode.Forbidden, Success = false, Message = message };
}