using System.Net;
using Application.Services.Schedule;
using Contract.Dtos.Schedule.Extensions;
using Contract.Dtos.Schedule.Requests;
using Contract.Dtos.Schedule.Responses;
using Contract.Shared;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace WebAPI.Test;

[TestFixture]
public class ScheduleControllerTests
{
    private Mock<IScheduleService> _scheduleServiceMock;
    private ScheduleController _scheduleController;

    [SetUp]
    public void SetUp()
    {
        _scheduleServiceMock = new Mock<IScheduleService>();
        _scheduleController = new ScheduleController(_scheduleServiceMock.Object);
    }

    [Test]
    public async Task GetScheduleSettings_ReturnsOkResult_WhenServiceReturnsSuccess()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var request = new GetScheduleSettingsRequest()
        {
            WeekStartDate = DateOnly.FromDateTime(DateTime.Now),
            WeekEndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(6))
        };

        var response = new ScheduleSettingsResponse()
        {
            WeekStartDate = request.WeekStartDate.Value,
            WeekEndDate = request.WeekEndDate.Value,
            StartTime = "09:00",
            EndTime = "17:00",
            SessionDuration = 60,
            BufferTime = 15,
            IsLocked = false,
            AvailableTimeSlots = new Dictionary<DateOnly, List<TimeSlotResponse>>()
        };

        var serviceResult = Result<ScheduleSettingsResponse>.Success(response, HttpStatusCode.OK);

        _scheduleServiceMock
            .Setup(s => s.GetScheduleSettingsAsync(mentorId, request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _scheduleController.GetScheduleSettings(mentorId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _scheduleServiceMock.Verify(s => s.GetScheduleSettingsAsync(mentorId, request), Times.Once);
        });
    }

    [Test]
    public async Task GetScheduleSettings_ReturnsBadRequest_WhenServiceReturnsError()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var request = new GetScheduleSettingsRequest()
        {
            WeekStartDate = DateOnly.FromDateTime(DateTime.Now),
            WeekEndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(6))
        };

        var serviceResult = Result.Failure<ScheduleSettingsResponse>("Error retrieving schedule settings", HttpStatusCode.BadRequest);

        _scheduleServiceMock
            .Setup(s => s.GetScheduleSettingsAsync(mentorId, request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _scheduleController.GetScheduleSettings(mentorId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _scheduleServiceMock.Verify(s => s.GetScheduleSettingsAsync(mentorId, request), Times.Once);
        });
    }

    [Test]
    public async Task UpdateScheduleSettings_ReturnsOkResult_WhenServiceReturnsSuccess()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var request = new SaveScheduleSettingsRequest()
        {
            WeekStartDate = DateOnly.FromDateTime(DateTime.Now),
            WeekEndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(6)),
            StartTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(9)),
            EndTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(17)),
            SessionDuration = 60,
            BufferTime = 15,
            AvailableTimeSlots = new Dictionary<DateOnly, List<TimeSlotRequest>>()
        };

        var serviceResult = Result<SaveScheduleSettingsResponse>.Success(new SaveScheduleSettingsResponse(), HttpStatusCode.OK);

        // Set up user context with matching mentorId
        var user = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, mentorId.ToString())
            })
        );
        _scheduleController.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
        };

        _scheduleServiceMock
            .Setup(s => s.SaveScheduleSettingsAsync(mentorId, request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _scheduleController.UpdateScheduleSettings(mentorId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            Assert.That(objectResult?.Value, Is.InstanceOf<Result<SaveScheduleSettingsResponse>>());
            _scheduleServiceMock.Verify(s => s.SaveScheduleSettingsAsync(mentorId, request), Times.Once);
        });
    }

    [Test]
    public async Task UpdateScheduleSettings_ReturnsForbid_WhenMentorIdDoesNotMatchUser()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var userId = Guid.NewGuid(); // Different from mentorId
        var request = new SaveScheduleSettingsRequest();
        var user = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId.ToString())
            })
        );
        _scheduleController.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
        };

        // Act
        var result = await _scheduleController.UpdateScheduleSettings(mentorId, request);

        // Assert
        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }
}
