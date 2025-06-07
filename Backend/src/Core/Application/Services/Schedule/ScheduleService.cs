using Contract.Dtos.Schedule.Extensions;
using Contract.Dtos.Schedule.Requests;
using Contract.Dtos.Schedule.Responses;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using System.Net;
using System.Text;

namespace Application.Services.Schedule;

public class ScheduleService(IScheduleRepository scheduleRepository, IUserRepository userRepository, IMentorAvailabilityTimeSlotRepository mentorAvailableTimeSlotRepository, IEmailService emailService) : IScheduleService
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
            var today = DateTime.UtcNow;
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

        var defaultTimeSlot = GetAllDefaultTimeSlots(scheduleSettings, mentor.Timezone);
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

        Schedules? scheduleSettings = await scheduleRepository.GetScheduleSettingsAsync(mentorId, request.WeekStartDate, request.WeekEndDate);

        if (scheduleSettings is not null)
        {
            scheduleSettings.StartHour = request.StartTime;
            scheduleSettings.EndHour = request.EndTime;
            scheduleSettings.SessionDuration = request.SessionDuration;
            scheduleSettings.BufferTime = request.BufferTime;
            scheduleRepository.Update(scheduleSettings);
        }
        else
        {
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
            await scheduleRepository.AddAsync(scheduleSettings);
        }
        await scheduleRepository.SaveChangesAsync();

        List<MentorAvailableTimeSlot> deletingTimeSlots = mentorAvailableTimeSlotRepository.DeletePendingAndCancelledTimeSlots(scheduleSettings.Id);

        Dictionary<string, string> uniqueLearnerInfos = new Dictionary<string, string>();

        if (deletingTimeSlots is not null)
        {
            foreach (var timeSlot in deletingTimeSlots)
            {
                if (timeSlot.Sessions == null || !timeSlot.Sessions.Any())
                {
                    continue;
                }

                foreach (var session in timeSlot.Sessions)
                {
                    if (session.Status == SessionStatus.Pending)
                    {
                        uniqueLearnerInfos[session.Learner!.Email] = session.Learner.FullName;
                    }
                }
            }
        }

        if (uniqueLearnerInfos.Any())
        {
            foreach (var learnerInfo in uniqueLearnerInfos)
            {
                var subject = EmailConstants.SUBJECT_MENTOR_UPDATED_SCHEDULE;
                var body = EmailConstants.BodyMentorUpdatedScheduleEmail(learnerInfo.Value, mentor.FullName);
                await emailService.SendEmailAsync(learnerInfo.Key, subject, body);
            }
        }

        var existingActiveSessions = mentorAvailableTimeSlotRepository.GetConfirmedTimeSlots(scheduleSettings.Id);
        StringBuilder stringBuilder = new();
        
        foreach (var timeSlot in request.AvailableTimeSlots)
        {
            DateOnly date = timeSlot.Key;
            List<TimeSlotRequest> slots = timeSlot.Value;

            foreach (var slot in slots)
            {
                var slotStartDateTime = date.ToDateTime(slot.StartTime);
                var slotEndDateTime = date.ToDateTime(slot.EndTime);

                if (existingActiveSessions is not null && existingActiveSessions.Any())
                {
                    bool isOverlap = existingActiveSessions.Any(s => s.Date == date && (
                        (s.StartTime <= slot.StartTime && s.EndTime > slot.StartTime) ||
                        (s.StartTime < slot.EndTime && s.EndTime >= slot.EndTime) ||
                        (s.StartTime >= slot.StartTime && s.EndTime <= slot.EndTime)
                    ));

                    if (isOverlap)
                    {
                        var msg = $"Time slot {slot.StartTime} - {slot.EndTime} on {date} overlaps with an existing session. ";
                        stringBuilder.AppendLine(msg);
                        continue;
                    }
                }

                var mentorAvailableTimeSlot = new MentorAvailableTimeSlot
                {
                    ScheduleId = scheduleSettings.Id,
                    Date = date,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                };

                await mentorAvailableTimeSlotRepository.AddAsync(mentorAvailableTimeSlot);
            }
        }

        await mentorAvailableTimeSlotRepository.SaveChangesAsync();

        var result = new SaveScheduleSettingsResponse
        {
            Message = stringBuilder.Length > 0
                ? stringBuilder.ToString()
                : "Schedule saved successfully!"
        };

        return Result.Success(result, HttpStatusCode.OK);
    }

    /// <summary>
    /// Generates a dictionary of all possible time slots for a given schedule settings.
    /// These time slots are initially marked as not available and not booked.
    /// Changes: Only generate time slots in the future (current not included).
    /// </summary>
    /// <param name="scheduleSettings">The schedule settings containing start/end dates, times, session duration, and buffer time.</param>
    /// <returns>A dictionary where the key is the date and the value is a list of default <see cref="TimeSlotResponse"/> objects for that date.</returns>
    public Dictionary<DateOnly, List<TimeSlotResponse>> GetAllDefaultTimeSlots(Schedules scheduleSettings, string userTimezone)
    {
        Dictionary<DateOnly, List<TimeSlotResponse>> allTimeSlots = new();
        
        // Create TimeZoneInfo from user's timezone
        TimeZoneInfo userTimeZoneInfo;
        try 
        {
            userTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userTimezone);
        }
        catch 
        {
            // Fallback to UTC if timezone is invalid
            userTimeZoneInfo = TimeZoneInfo.Utc;
        }

        // Get current date and time in UTC
        DateTime utcNow = DateTime.UtcNow;

        int dayCount = scheduleSettings.WeekEndDate.DayNumber - scheduleSettings.WeekStartDate.DayNumber + 1;

        for (int dayIndex = 0; dayIndex < dayCount; dayIndex++)
        {
            var currentLocalDate = scheduleSettings.WeekStartDate.AddDays(dayIndex);
            
            var localStartDateTime = currentLocalDate.ToDateTime(scheduleSettings.StartHour);
            DateTime localEndDateTime;

            localEndDateTime = scheduleSettings.EndHour <= scheduleSettings.StartHour
                ? currentLocalDate.AddDays(1).ToDateTime(scheduleSettings.EndHour)
                : currentLocalDate.ToDateTime(scheduleSettings.EndHour);
            
            var localSlotDateTime = localStartDateTime;
            
            while (localSlotDateTime.AddMinutes(scheduleSettings.SessionDuration) <= localEndDateTime)
            {
                var localEndSlotTime = localSlotDateTime.AddMinutes(scheduleSettings.SessionDuration);
                
                var utcSlotDateTime = TimeZoneInfo.ConvertTimeToUtc(localSlotDateTime, userTimeZoneInfo);
                var utcEndSlotTime = TimeZoneInfo.ConvertTimeToUtc(localEndSlotTime, userTimeZoneInfo);
                
                if (utcSlotDateTime > utcNow)
                {
                    var timeSlot = new TimeSlotResponse
                    {
                        Id = Guid.NewGuid(),
                        StartTime = TimeOnly.FromDateTime(utcSlotDateTime).ToString("HH:mm"),
                        EndTime = TimeOnly.FromDateTime(utcEndSlotTime).ToString("HH:mm"),
                        IsAvailable = false,
                        IsBooked = false
                    };

                    var utcDate = DateOnly.FromDateTime(utcSlotDateTime);
                    
                    if (!allTimeSlots.ContainsKey(utcDate))
                    {
                        allTimeSlots[utcDate] = new List<TimeSlotResponse>();
                    }
                    
                    allTimeSlots[utcDate].Add(timeSlot);
                }

                localSlotDateTime = localSlotDateTime.AddMinutes(scheduleSettings.SessionDuration + scheduleSettings.BufferTime);
            }
        }

        return allTimeSlots;
    }

    /// <summary>
    /// Converts the available time slots from the schedule settings into a dictionary format.
    /// Each date maps to a list of time slots, where each time slot contains its start and end times, availability status, and booking status.
    /// Changes: Only generate time slots in the future (current not included).
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
        var now = DateTime.UtcNow;

        foreach (var group in groupedByDate)
        {
            DateOnly date = group.Key;
            List<TimeSlotResponse> timeSlots = new();

            foreach (var availableSlot in group)
            {
                DateTime slotDateTime = date.ToDateTime(availableSlot.StartTime);
                if (slotDateTime <= now)
                {
                    continue;
                }

                TimeSlotResponse timeSlot = new TimeSlotResponse
                {
                    Id = availableSlot.Id,
                    StartTime = availableSlot.StartTime.ToString("HH:mm"),
                    EndTime = availableSlot.EndTime.ToString("HH:mm"),
                    IsAvailable = true,
                    IsBooked = availableSlot.Sessions?.Any(
                        s => s.Status == SessionStatus.Approved ||
                        s.Status == SessionStatus.Completed ||
                        s.Status == SessionStatus.Rescheduled
                    ) ?? false
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
        DateTime now = DateTime.UtcNow;
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
}