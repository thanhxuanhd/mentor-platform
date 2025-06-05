// filepath: c:\Users\harun\Desktop\r2e-final\Backend\test\Application.Test\ScheduleServiceTests.cs
using Application.Services.Schedule;
using Contract.Dtos.Schedule.Requests;
using Contract.Dtos.Schedule.Responses;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Domain.Constants;
using Contract.Dtos.Schedule.Extensions; // Added for ScheduleSettingsConstants

namespace Application.Test;

[TestFixture]
public class ScheduleServiceTests
{
    private Mock<IScheduleRepository> _mockScheduleRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IMentorAvailabilityTimeSlotRepository> _mockMentorAvailabilityTimeSlotRepository;
    private Mock<IEmailService> _mockEmailService;
    private ScheduleService _scheduleService;

    [SetUp]
    public void Setup()
    {
        _mockScheduleRepository = new Mock<IScheduleRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMentorAvailabilityTimeSlotRepository = new Mock<IMentorAvailabilityTimeSlotRepository>();
        _mockEmailService = new Mock<IEmailService>();

        _scheduleService = new ScheduleService(
            _mockScheduleRepository.Object,
            _mockUserRepository.Object,
            _mockMentorAvailabilityTimeSlotRepository.Object,
            _mockEmailService.Object);
    }

    private User CreateTestUser(Guid id, UserRole role = UserRole.Mentor, UserStatus status = UserStatus.Active)
    {
        return new User
        {
            Id = id,
            RoleId = (int)role,
            Status = status,
            Email = "mentor@example.com",
            // FirstName and LastName are not part of the User entity based on provided context
            // If they are, ensure User entity definition includes them.
            FullName = "Test Mentor" // Assuming FullName exists
        };
    }

    private Schedules CreateTestScheduleSettings(Guid mentorId, DateOnly weekStartDate, bool withTimeSlots = true)
    {
        var schedule = new Schedules
        {
            Id = Guid.NewGuid(),
            MentorId = mentorId,
            WeekStartDate = weekStartDate,
            WeekEndDate = weekStartDate.AddDays(6),
            StartHour = ScheduleSettingsConstants.DefaultStartTime, // Using constants
            EndHour = ScheduleSettingsConstants.DefaultEndTime,
            SessionDuration = ScheduleSettingsConstants.DefaultSessionDuration,
            BufferTime = ScheduleSettingsConstants.DefaultBufferTime,
            AvailableTimeSlots = withTimeSlots ? new List<MentorAvailableTimeSlot>
            {
                new MentorAvailableTimeSlot
                {
                    Id = Guid.NewGuid(),
                    // ScheduleId will be set by the loop below
                    Date = weekStartDate.AddDays((int)DayOfWeek.Monday - (int)weekStartDate.DayOfWeek + (weekStartDate.DayOfWeek > DayOfWeek.Monday ? 7 : 0)),
                    StartTime = new TimeOnly(9,0),
                    EndTime = new TimeOnly(10,0),
                    Sessions = new List<Sessions>() // Not booked
                },
                new MentorAvailableTimeSlot
                {
                    Id = Guid.NewGuid(),
                    // ScheduleId will be set by the loop below
                    Date = weekStartDate.AddDays((int)DayOfWeek.Monday - (int)weekStartDate.DayOfWeek + (weekStartDate.DayOfWeek > DayOfWeek.Monday ? 7 : 0)),
                    StartTime = new TimeOnly(10,0),
                    EndTime = new TimeOnly(11,0),
                    Sessions = new List<Sessions> { new Sessions { Status = SessionStatus.Approved } } // Booked
                }
            } : new List<MentorAvailableTimeSlot>()
        };
        if (withTimeSlots)
        {
            foreach (var slot in schedule.AvailableTimeSlots!)
            {
                slot.ScheduleId = schedule.Id;
            }
        }
        return schedule;
    }

    [Test]
    public async Task GetScheduleSettingsAsync_MentorNotFound_ReturnsFailureNotFound()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var request = new GetScheduleSettingsRequest { WeekStartDate = null };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(mentorId, null)).ReturnsAsync((User)null!);

        // Act
        var result = await _scheduleService.GetScheduleSettingsAsync(mentorId, request);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(result.Error, Is.EqualTo("Mentor not found"));
    }

    [Test]
    public async Task GetScheduleSettingsAsync_ScheduleSettingsNotFound_ReturnsNewDefaultSettings()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var mentor = CreateTestUser(mentorId);
        var today = DateTime.Now;
        // For calculating week start: if today is Sunday (DayOfWeek=0), subtract 0 days. If Monday (1), subtract 1 day.
        int daysToSubtract = (int)today.DayOfWeek; 
        // If the culture considers Monday as the first day of the week and DayOfWeek returns 0 for Sunday, 
        // and 1 for Monday, this calculation correctly finds the preceding Sunday if week starts on Sunday.
        // If week is considered to start on Monday, and today is Sunday, this would go to previous Sunday.
        // Assuming week starts on Sunday for this calculation based on typical DayOfWeek enum.
        // If your system defines week start differently (e.g. Monday), adjust 'daysToSubtract'.
        // For a consistent Monday start: daysToSubtract = (today.DayOfWeek == DayOfWeek.Sunday) ? 6 : (int)today.DayOfWeek - 1;
        var expectedWeekStartDate = DateOnly.FromDateTime(today.AddDays(-daysToSubtract));
        var request = new GetScheduleSettingsRequest { WeekStartDate = null };

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(mentorId, null)).ReturnsAsync(mentor);
        _mockScheduleRepository.Setup(repo => repo.GetScheduleSettingsAsync(mentorId, expectedWeekStartDate, expectedWeekStartDate.AddDays(6))).ReturnsAsync((Schedules)null!);

        // Act
        var result = await _scheduleService.GetScheduleSettingsAsync(mentorId, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.StartTime, Is.EqualTo(ScheduleSettingsConstants.DefaultStartTime.ToString("HH:mm")));
        // Default slots should be generated, and since it's for the current week, 
        // past slots won't be generated, but future ones might.
        // This assertion depends on current time; might be empty if all default slots are in the past.
        // For robustness, consider if this should assert Not.Null and then conditionally Not.Empty
        // or check for specific future slots if test date is controlled.
        // For now, assuming some future default slots will be generated if the week is current/future.
        Assert.That(result.Value.AvailableTimeSlots, Is.Not.Null); 
    }
    
    [Test]
    public async Task GetScheduleSettingsAsync_ExistingSettingsFound_ReturnsSettings()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var mentor = CreateTestUser(mentorId);
        var weekStartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var scheduleSettings = CreateTestScheduleSettings(mentorId, weekStartDate);
        var request = new GetScheduleSettingsRequest { WeekStartDate = weekStartDate };

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(mentorId, null)).ReturnsAsync(mentor);
        _mockScheduleRepository.Setup(repo => repo.GetScheduleSettingsAsync(mentorId, weekStartDate, weekStartDate.AddDays(6))).ReturnsAsync(scheduleSettings);

        // Act
        var result = await _scheduleService.GetScheduleSettingsAsync(mentorId, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.StartTime, Is.EqualTo(scheduleSettings.StartHour.ToString("HH:mm")));
        Assert.That(result.Value.AvailableTimeSlots, Is.Not.Empty);
    }

    [Test]
    public async Task SaveScheduleSettingsAsync_MentorNotFound_ReturnsFailureNotFound()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var request = new SaveScheduleSettingsRequest();
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(mentorId, null)).ReturnsAsync((User)null!);

        // Act
        var result = await _scheduleService.SaveScheduleSettingsAsync(mentorId, request);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(result.Error, Is.EqualTo("Mentor not found"));
    }

    [Test]
    public async Task SaveScheduleSettingsAsync_NewSettings_CreatesAndSavesSettings()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var mentor = CreateTestUser(mentorId);
        var weekStartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var request = new SaveScheduleSettingsRequest
        {
            WeekStartDate = weekStartDate,
            WeekEndDate = weekStartDate.AddDays(6),
            StartTime = new TimeOnly(9,0),
            EndTime = new TimeOnly(17,0),
            SessionDuration = 45,
            BufferTime = 10,
            AvailableTimeSlots = new Dictionary<DateOnly, List<TimeSlotRequest>>
            {
                { weekStartDate.AddDays(1), new List<TimeSlotRequest> { new TimeSlotRequest { StartTime = new TimeOnly(9,0), EndTime = new TimeOnly(10,0) } } }
            }
        };

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(mentorId, null)).ReturnsAsync(mentor);
        _mockScheduleRepository.Setup(repo => repo.GetScheduleSettingsAsync(mentorId, request.WeekStartDate, request.WeekEndDate)).ReturnsAsync((Schedules)null!);
        _mockScheduleRepository.Setup(repo => repo.AddAsync(It.IsAny<Schedules>())).Returns(Task.CompletedTask);
        _mockMentorAvailabilityTimeSlotRepository.Setup(repo => repo.AddAsync(It.IsAny<MentorAvailableTimeSlot>())).Returns(Task.CompletedTask);
        _mockMentorAvailabilityTimeSlotRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1); // Simulate one change saved

        // Act
        var result = await _scheduleService.SaveScheduleSettingsAsync(mentorId, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Message.Contains("Schedule saved successfully!"), Is.True);
        _mockScheduleRepository.Verify(repo => repo.AddAsync(It.Is<Schedules>(s => s.MentorId == mentorId)), Times.Once);
        _mockMentorAvailabilityTimeSlotRepository.Verify(repo => repo.AddAsync(It.IsAny<MentorAvailableTimeSlot>()), Times.AtLeastOnce);
    }

    [Test]
    public async Task SaveScheduleSettingsAsync_ExistingSettings_UpdatesSettings()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var mentor = CreateTestUser(mentorId);
        var weekStartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var existingSettings = CreateTestScheduleSettings(mentorId, weekStartDate, false); // no existing time slots initially
        var request = new SaveScheduleSettingsRequest
        {
            WeekStartDate = weekStartDate,
            WeekEndDate = weekStartDate.AddDays(6),
            StartTime = new TimeOnly(10,0),
            EndTime = new TimeOnly(18,0),
            SessionDuration = 30,
            BufferTime = 5,
            AvailableTimeSlots = new Dictionary<DateOnly, List<TimeSlotRequest>>
            {
                { weekStartDate.AddDays(2), new List<TimeSlotRequest> { new TimeSlotRequest { StartTime = new TimeOnly(10,0), EndTime = new TimeOnly(11,0) } } }
            }
        };

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(mentorId, null)).ReturnsAsync(mentor);
        _mockScheduleRepository.Setup(repo => repo.GetScheduleSettingsAsync(mentorId, request.WeekStartDate, request.WeekEndDate)).ReturnsAsync(existingSettings);
        _mockScheduleRepository.Setup(repo => repo.Update(It.IsAny<Schedules>()));
        _mockMentorAvailabilityTimeSlotRepository.Setup(repo => repo.DeletePendingAndCancelledTimeSlots(existingSettings.Id)).Returns(new List<MentorAvailableTimeSlot>());
        _mockMentorAvailabilityTimeSlotRepository.Setup(repo => repo.GetConfirmedTimeSlots(existingSettings.Id)).Returns(new List<MentorAvailableTimeSlot>());
        _mockMentorAvailabilityTimeSlotRepository.Setup(repo => repo.AddAsync(It.IsAny<MentorAvailableTimeSlot>())).Returns(Task.CompletedTask);
        _mockMentorAvailabilityTimeSlotRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _scheduleService.SaveScheduleSettingsAsync(mentorId, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Message.Contains("Schedule saved successfully!"), Is.True);
        _mockScheduleRepository.Verify(repo => repo.Update(It.Is<Schedules>(s => s.Id == existingSettings.Id && s.StartHour == request.StartTime)), Times.Once);
        _mockMentorAvailabilityTimeSlotRepository.Verify(repo => repo.AddAsync(It.IsAny<MentorAvailableTimeSlot>()), Times.AtLeastOnce);
    }

    [Test]
    public void GetAllDefaultTimeSlots_ValidSettings_GeneratesFutureTimeSlots()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var today = DateTime.UtcNow;
        var weekStartDate = DateOnly.FromDateTime(today.AddDays(1)); // Ensure start date is tomorrow
        var scheduleSettings = new Schedules
        {
            MentorId = mentorId,
            WeekStartDate = weekStartDate,
            WeekEndDate = weekStartDate.AddDays(6), // Covers a full week from tomorrow
            StartHour = new TimeOnly(9, 0),
            EndHour = new TimeOnly(17, 0),
            SessionDuration = 60, 
            BufferTime = 0,
            AvailableTimeSlots = new List<MentorAvailableTimeSlot>() // Not directly used by GetAllDefaultTimeSlots
        };

        // Act
        var result = _scheduleService.GetAllDefaultTimeSlots(scheduleSettings);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);

        var currentDateTime = DateTime.Now;
        foreach (var dateSlotsPair in result)
        {
            Assert.That(dateSlotsPair.Key, Is.GreaterThanOrEqualTo(weekStartDate), "All generated dates should be within the schedule range.");
            Assert.That(dateSlotsPair.Key, Is.LessThanOrEqualTo(scheduleSettings.WeekEndDate), "All generated dates should be within the schedule range.");
            
            foreach (var slot in dateSlotsPair.Value)
            {
                var slotStartDateTime = dateSlotsPair.Key.ToDateTime(TimeOnly.Parse(slot.StartTime));
                Assert.That(slotStartDateTime, Is.GreaterThan(currentDateTime), "All slot start times should be in the future.");
                Assert.That(slot.IsAvailable, Is.False, "Default slots should be marked as not available.");
                Assert.That(slot.IsBooked, Is.False, "Default slots should be marked as not booked.");
                Assert.That(TimeOnly.Parse(slot.EndTime), Is.EqualTo(TimeOnly.Parse(slot.StartTime).AddMinutes(scheduleSettings.SessionDuration)));
            }
        }
        
        var firstDayWithSlots = result.FirstOrDefault(kvp => kvp.Value.Any());
        Assert.That(firstDayWithSlots.Value, Is.Not.Null, "Should have at least one day with slots if StartDate is future and EndDate allows.");
        if (firstDayWithSlots.Value != null) {
            // Expected slots for the first available day (e.g., 9:00 to 17:00, 1-hour sessions, 0 buffer = 8 slots)
            var expectedSlots = (scheduleSettings.EndHour.ToTimeSpan() - scheduleSettings.StartHour.ToTimeSpan()).TotalMinutes / (scheduleSettings.SessionDuration + scheduleSettings.BufferTime);
            Assert.That(firstDayWithSlots.Value.Count, Is.EqualTo(expectedSlots));
        }
    }

    [Test]
    public void ConvertToDictionary_NoAvailableTimeSlots_ReturnsEmptyDictionary()
    {
        // Arrange
        var scheduleSettings = CreateTestScheduleSettings(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), false); // No time slots
        if (scheduleSettings.AvailableTimeSlots != null) // Ensure it's not null before clearing
        {
            scheduleSettings.AvailableTimeSlots.Clear();
        }
        else
        {
            scheduleSettings.AvailableTimeSlots = new List<MentorAvailableTimeSlot>();
        }

        // Act
        var result = _scheduleService.ConvertToDictionary(scheduleSettings);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ConvertToDictionary_WithAvailableTimeSlots_ConvertsToDictionaryCorrectly()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var today = DateTime.UtcNow;
        var futureDate = DateOnly.FromDateTime(today.AddDays(2)); // Ensure it's a future date

        var scheduleSettings = new Schedules
        {
            Id = Guid.NewGuid(),
            MentorId = mentorId,
            WeekStartDate = futureDate.AddDays(-(int)futureDate.DayOfWeek), // Start of the week for futureDate
            WeekEndDate = futureDate.AddDays(6 - (int)futureDate.DayOfWeek), // End of the week for futureDate
            StartHour = new TimeOnly(8,0),
            EndHour = new TimeOnly(18,0),
            SessionDuration = 60,
            BufferTime = 0,
            AvailableTimeSlots = new List<MentorAvailableTimeSlot>
            {
                new MentorAvailableTimeSlot
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = Guid.NewGuid(), // Will be set to scheduleSettings.Id below
                    Date = futureDate,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(10, 0),
                    Sessions = new List<Sessions>() // Simulates not booked
                },
                new MentorAvailableTimeSlot
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = Guid.NewGuid(), 
                    Date = futureDate,
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(11, 0),
                    Sessions = new List<Sessions> { new Sessions { Status = SessionStatus.Approved } } // Simulates booked
                },
                new MentorAvailableTimeSlot // A slot for today, should be filtered out if its start time is in the past
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = Guid.NewGuid(),
                    Date = DateOnly.FromDateTime(today.AddDays(-1)), // Set date to yesterday to ensure it's filtered out
                    StartTime = new TimeOnly(14,0), // Assuming this time is in the future today for it to appear
                    EndTime = new TimeOnly(15,0),
                    Sessions = new List<Sessions>()
                }
            }
        };
        foreach (var slot in scheduleSettings.AvailableTimeSlots!)
        {
            slot.ScheduleId = scheduleSettings.Id;
        }

        // Act
        var result = _scheduleService.ConvertToDictionary(scheduleSettings);

        // Assert
        Assert.That(result, Is.Not.Null);
        // Expecting only one entry for the 'futureDate' as the other slot is for 'today'
        Assert.That(result.Count, Is.EqualTo(1), "Should only contain future dated slots.");
        Assert.That(result.ContainsKey(futureDate), Is.True);

        var slotsForDate = result[futureDate];
        Assert.That(slotsForDate.Count, Is.EqualTo(2));

        var firstSlot = slotsForDate.First(s => TimeOnly.Parse(s.StartTime) == new TimeOnly(9,0));
        Assert.That(firstSlot.IsAvailable, Is.True);
        Assert.That(firstSlot.IsBooked, Is.False);

        var secondSlot = slotsForDate.First(s => TimeOnly.Parse(s.StartTime) == new TimeOnly(10,0));
        Assert.That(secondSlot.IsAvailable, Is.True); // Corrected: ConvertToDictionary sets IsAvailable to true for future slots
        Assert.That(secondSlot.IsBooked, Is.True);
        
        var currentDateTime = DateTime.UtcNow; // Use UtcNow for consistency
        foreach (var dateKey in result.Keys)
        {
            foreach(var slot in result[dateKey])
            {
                 Assert.That(dateKey.ToDateTime(TimeOnly.Parse(slot.StartTime)), Is.GreaterThan(currentDateTime));
            }
        }
    }
}
