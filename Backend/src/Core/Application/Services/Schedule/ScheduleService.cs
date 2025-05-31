using Contract.Dtos.Schedule.Extensions;
using Contract.Dtos.Schedule.Requests;
using Contract.Dtos.Schedule.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using System.Net;

namespace Application.Services.Schedule;

public class ScheduleService(IScheduleRepository scheduleRepository, IUserRepository userRepository) : IScheduleService
{
    public async Task<Result<ScheduleSettingsResponse>> GetScheduleSettingsAsync(Guid mentorId, GetScheduleSettingsRequest request)
    {
        var mentor = await userRepository.GetByIdAsync(mentorId);
        if (mentor == null)
        {
            return Result.Failure<ScheduleSettingsResponse>("Mentor not found", HttpStatusCode.NotFound);
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

        Schedules? scheduleSettings = await scheduleRepository.GetScheduleSettingsAsync(mentorId, weekStartDate, weekEndDate);
        if (scheduleSettings == null)
        {
            scheduleSettings = new Schedules
            {
                WeekStartDate = weekStartDate,
                WeekEndDate = weekEndDate,
                StartHour = ScheduleSettingsConstants.DefaultStartTime,
                EndHour = ScheduleSettingsConstants.DefaultEndTime,
                SessionDuration = ScheduleSettingsConstants.DefaultSessionDuration,
                BufferTime = ScheduleSettingsConstants.DefaultBufferTime,
            };
        }

        var response = new ScheduleSettingsResponse
        {
            WeekStartDate = scheduleSettings.WeekStartDate,
            WeekEndDate = scheduleSettings.WeekEndDate,
            StartTime = scheduleSettings.StartHour.ToString("HH:mm"),
            EndTime = scheduleSettings.EndHour.ToString("HH:mm"),
            SessionDuration = scheduleSettings.SessionDuration,
            BufferTime = scheduleSettings.BufferTime,
        };

        var defaultTimeSlot = GetAllDefaultTimeSlots(scheduleSettings);
        var existingTimeSlots = ConvertToDictionary(scheduleSettings);
        var allTimeSlots = ReplaceDefaultWithExistingTimeSlots(defaultTimeSlot, existingTimeSlots);

        response.AvailableTimeSlots = allTimeSlots;

        response.IsLocked = IsLocked(allTimeSlots);

        return Result.Success(response, HttpStatusCode.OK);
    }

    public async Task<Result<SaveScheduleSettingsResponse>> SaveScheduleSettingsAsync(Guid mentorId, SaveScheduleSettingsRequest request)
    {
        var mentor = await userRepository.GetByIdAsync(mentorId);
        if (mentor == null)
        {
            return Result.Failure<SaveScheduleSettingsResponse>("Mentor not found", HttpStatusCode.NotFound);
        }

        var scheduleSettings = await scheduleRepository.GetScheduleSettingsAsync(mentorId, request.WeekStartDate, request.WeekEndDate);

        Dictionary<Guid, TimeSlotData> bookedTimeSlots = new();
        if (scheduleSettings != null)
        {
            bookedTimeSlots = GetBookedTimeSlotsData(scheduleSettings);
            scheduleRepository.Delete(scheduleSettings);
        }

        scheduleSettings = new Schedules
        {
            MentorId = mentorId,
            WeekStartDate = request.WeekStartDate,
            WeekEndDate = request.WeekEndDate,
            StartHour = request.StartTime,
            EndHour = request.EndTime,
            SessionDuration = request.SessionDuration,
            BufferTime = request.BufferTime,
        };

        foreach (var timeSlot in request.AvailableTimeSlots)
        {
            DateOnly date = timeSlot.Key;
            List<TimeSlotRequest> slots = timeSlot.Value;

            foreach (var slot in slots)
            {
                var mentorAvailableTimeSlot = new MentorAvailableTimeSlot
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = scheduleSettings.Id,
                    Date = date,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                };

                scheduleSettings.AvailableTimeSlots ??= new List<MentorAvailableTimeSlot>();
                scheduleSettings.AvailableTimeSlots.Add(mentorAvailableTimeSlot);
            }
        }
        foreach (var bookedSlot in bookedTimeSlots)
        {
            var timeSlotId = Guid.NewGuid();
            var mentorAvailableTimeSlot = new MentorAvailableTimeSlot
            {
                Id = timeSlotId,
                ScheduleId = scheduleSettings.Id,
                Date = bookedSlot.Value.Date,
                StartTime = bookedSlot.Value.StartTime,
                EndTime = bookedSlot.Value.EndTime,
                Sessions = new List<Sessions>
                {
                    new Sessions
                    {
                        Id = Guid.NewGuid(),
                        LearnerId = bookedSlot.Key,
                        Status = bookedSlot.Value.Status,
                        TimeSlotId = timeSlotId
                    }
                }   
            };

            scheduleSettings.AvailableTimeSlots ??= new List<MentorAvailableTimeSlot>();
            scheduleSettings.AvailableTimeSlots.Add(mentorAvailableTimeSlot);
        }

        await scheduleRepository.AddAsync(scheduleSettings);
        await scheduleRepository.SaveChangesAsync();

        var result = new SaveScheduleSettingsResponse
        {
            Message = "Schedule settings saved successfully."
        };

        return Result.Success(result, HttpStatusCode.OK);
    }

    /// <summary>
    /// Generates a dictionary of all possible time slots for a given schedule settings.
    /// These time slots are initially marked as not available and not booked.
    /// </summary>
    /// <param name="scheduleSettings">The schedule settings containing start/end dates, times, session duration, and buffer time.</param>
    /// <returns>A dictionary where the key is the date and the value is a list of default <see cref="TimeSlotResponse"/> objects for that date.</returns>
    public Dictionary<DateOnly, List<TimeSlotResponse>> GetAllDefaultTimeSlots(Schedules scheduleSettings)
    {
        Dictionary<DateOnly, List<TimeSlotResponse>> allTimeSlots = new();

        int dayCount = (scheduleSettings.WeekEndDate.DayNumber - scheduleSettings.WeekStartDate.DayNumber) + 1;

        for (int dayIndex = 0; dayIndex < dayCount; dayIndex++)
        {
            List<TimeSlotResponse> dailyTimeSlots = new();
            DateOnly currentDate = scheduleSettings.WeekStartDate.AddDays(dayIndex);
            DateTime currentDateTime = currentDate.ToDateTime(scheduleSettings.StartHour);
            DateTime endDateTime;

            endDateTime = scheduleSettings.EndHour <= scheduleSettings.StartHour
                ? currentDate.AddDays(1).ToDateTime(scheduleSettings.EndHour)
                : currentDate.ToDateTime(scheduleSettings.EndHour);

            while (currentDateTime.AddMinutes(scheduleSettings.SessionDuration) <= endDateTime)
            {
                var sessionEndDateTime = currentDateTime.AddMinutes(scheduleSettings.SessionDuration);

                var timeSlot = new TimeSlotResponse
                {
                    Id = Guid.NewGuid(),
                    StartTime = TimeOnly.FromDateTime(currentDateTime).ToString("HH:mm"),
                    EndTime = TimeOnly.FromDateTime(sessionEndDateTime).ToString("HH:mm"),
                    IsAvailable = false,
                    IsBooked = false
                };

                dailyTimeSlots.Add(timeSlot);

                currentDateTime = currentDateTime.AddMinutes(scheduleSettings.SessionDuration + scheduleSettings.BufferTime);
            }

            allTimeSlots.Add(currentDate, dailyTimeSlots);
        }

        return allTimeSlots;
    }

    /// <summary>
    /// Converts the available time slots from the schedule settings into a dictionary format.
    /// Each date maps to a list of time slots, where each time slot contains its start and end times, availability status, and booking status.
    /// </summary>
    /// <param name="scheduleSettings">The schedule settings containing available time slots.</param>
    /// <returns>A dictionary where the key is the date and the value is a list of <see cref="TimeSlotResponse"/> objects for that date.</returns>
    public Dictionary<DateOnly, List<TimeSlotResponse>> ConvertToDictionary(Schedules scheduleSettings)
    {
        Dictionary<DateOnly, List<TimeSlotResponse>> result = new();

        if (scheduleSettings.AvailableTimeSlots == null || !scheduleSettings.AvailableTimeSlots.Any())
        {
            return result;
        }

        // Group the available time slots by date
        var groupedByDate = scheduleSettings.AvailableTimeSlots.GroupBy(ts => ts.Date);

        foreach (var group in groupedByDate)
        {
            DateOnly date = group.Key;
            List<TimeSlotResponse> timeSlots = new();

            foreach (var availableSlot in group)
            {
                bool isBooked = availableSlot.Sessions != null && availableSlot.Sessions.Any();

                TimeSlotResponse timeSlot = new TimeSlotResponse
                {
                    Id = availableSlot.Id,
                    StartTime = availableSlot.StartTime.ToString("HH:mm"),
                    EndTime = availableSlot.EndTime.ToString("HH:mm"),
                    IsAvailable = true,
                    IsBooked = availableSlot.Sessions?.Any(s => s.Status == SessionStatus.Confirmed) ?? false
                };

                timeSlots.Add(timeSlot);
            }

            timeSlots = timeSlots.OrderBy(ts => TimeOnly.Parse(ts.StartTime)).ToList();
            result.Add(date, timeSlots);
        }

        return result;
    }

    /// <summary>
    /// Replaces the default time slots with existing time slots for each date.
    /// Expectation: both dictionaries must be created from two functions: GetAllDefaultTimeSlots and ConvertToDictionary with the same Schedules object.
    /// If an existing time slot matches a default time slot by start and end times, it replaces the default slot.
    /// </summary>
    /// <param name="allDefaultTimeSlots">The dictionary of all default time slots.</param>
    /// <param name="existingTimeSlots">The dictionary of existing time slots.</param>
    /// <returns>A merged dictionary of time slots.</returns>
    public Dictionary<DateOnly, List<TimeSlotResponse>> ReplaceDefaultWithExistingTimeSlots(
        Dictionary<DateOnly, List<TimeSlotResponse>> allDefaultTimeSlots,
        Dictionary<DateOnly, List<TimeSlotResponse>> existingTimeSlots)
    {
        var mergedTimeSlots = new Dictionary<DateOnly, List<TimeSlotResponse>>();

        foreach (var defaultEntry in allDefaultTimeSlots)
        {
            DateOnly date = defaultEntry.Key;
            List<TimeSlotResponse> defaultSlotsForDate = defaultEntry.Value;
            List<TimeSlotResponse> finalSlotsForDate = new List<TimeSlotResponse>();

            existingTimeSlots.TryGetValue(date, out List<TimeSlotResponse>? existingSlotsForDateFromInput);

            var existingSlotsLookup = existingSlotsForDateFromInput?
                .GroupBy(ts => (ts.StartTime, ts.EndTime))
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var defaultSlot in defaultSlotsForDate)
            {
                if (existingSlotsLookup != null &&
                    existingSlotsLookup.TryGetValue((defaultSlot.StartTime, defaultSlot.EndTime), out TimeSlotResponse? existingMatch))
                {
                    finalSlotsForDate.Add(existingMatch);
                }
                else
                {
                    finalSlotsForDate.Add(defaultSlot);
                }
            }
            mergedTimeSlots.Add(date, finalSlotsForDate);
        }
        return mergedTimeSlots;
    }

    // Just a simple check to see if any of the future time slots are booked.
    // kvp stands for KeyValuePair<DateOnly, List<TimeSlotResponse>> if anyone is wondering.
    public bool IsLocked(Dictionary<DateOnly, List<TimeSlotResponse>> availableTimeSlots)
    {
        DateTime now = DateTime.Now;
        DateOnly today = DateOnly.FromDateTime(now);
        TimeOnly currentTime = TimeOnly.FromDateTime(now);

        return availableTimeSlots
            .Where(kvp => kvp.Key >= today && kvp.Value != null)
            .SelectMany(kvp => kvp.Value.Select(ts => new { Date = kvp.Key, TimeSlot = ts }))
            .Where(item => item.TimeSlot != null && item.TimeSlot.IsBooked)
            .Any(item =>
            {
                if (item.Date > today)
                    return true;

                if (item.Date == today && TimeOnly.TryParse(item.TimeSlot.StartTime, out TimeOnly slotStartTime))
                    return slotStartTime > currentTime;

                return false;
            });
    }

    public Dictionary<Guid, TimeSlotData> GetBookedTimeSlotsData(Schedules scheduleSettings)
    {
        Dictionary<Guid, TimeSlotData> bookedTimeSlots = new();

        if (scheduleSettings.AvailableTimeSlots == null || !scheduleSettings.AvailableTimeSlots.Any())
        {
            return bookedTimeSlots;
        }

        foreach (var timeSlot in scheduleSettings.AvailableTimeSlots)
        {
            if (timeSlot.Sessions != null && timeSlot.Sessions.Any())
            {
                foreach (var session in timeSlot.Sessions)
                {
                    if (session.Status == SessionStatus.Confirmed || 
                        session.Status == SessionStatus.Completed)
                    {
                        // Only add confirmed or completed sessions to the booked time slots
                        bookedTimeSlots[session.LearnerId] = new TimeSlotData
                        {
                            StartTime = timeSlot.StartTime,
                            EndTime = timeSlot.EndTime,
                            Date = timeSlot.Date,
                            Status = session.Status
                        };
                    }
                }
            }
        }

        return bookedTimeSlots;
    }
}