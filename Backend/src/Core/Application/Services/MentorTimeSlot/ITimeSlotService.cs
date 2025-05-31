using Contract.Dtos.Timeslot.Request;
using Contract.Dtos.Timeslot.Response;
using Contract.Shared;
using Domain.Entities;
using System.Net;

namespace Application.Services.MentorTimeSlot;

public interface ITimeSlotService
{
    //Task<List<MentorAvailableTimeSlot>> GenerateTimeSlotsFromScheduleAsync(Guid mentorId, DateOnly fromDate, DateOnly toDate);
    //Task<List<MentorAvailableTimeSlot>> GetAvailableSlotsAsync(Guid mentorId, DateOnly date);
    // Task<Result<GetTimeSlotResponse>> GetTimeslotByIdAsync(Guid id);
    // Task<Result<List<GetTimeSlotResponse>>> GetAllAsync();
    // Task AddAsync(MentorAvailableTimeSlot timeSlot);
    // Task UpdateAsync(Guid id, MentorAvailableTimeSlot timeSlot);
    // //Task DeleteAsync(Guid id);

    // //Task<ApiResponse> GetWeeklyCalendarAsync(DateTime weekStartDate, Guid mentorId);
    // //Task<ApiResponse> GetTimeSlotsByDateAsync(DateTime date, Guid mentorId);
    // Task<ApiResponse> SaveWeeklyAvailabilityAsync(SaveWeeklyAvailabilityRequest request);
    // Task<ApiResponse> UpdateWorkHoursAsync(UpdateWorkHoursRequest request);
    // Task<ApiResponse> UpdateSessionParametersAsync(UpdateSessionParametersRequest request);
    // Task<ApiResponse> CheckLockedStatusAsync(Guid mentorId);

    // Task<ApiResponse> GetWorkHoursAsync(Guid mentorId);
    // Task<ApiResponse> GetSessionParametersAsync(Guid mentorId);
    // Task<bool> HasBookedSessionsAsync(Guid mentorId);
}