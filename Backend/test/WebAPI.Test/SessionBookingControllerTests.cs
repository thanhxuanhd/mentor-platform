using System.Net;
using System.Security.Claims;
using Application.Services.SessionBooking;
using Contract.Dtos.SessionBooking.Requests;
using Contract.Dtos.SessionBooking.Response;
using Contract.Shared;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace WebAPI.Test;

[TestFixture]
public class SessionBookingControllerTests
{
    private Mock<ISessionBookingService> _sessionBookingServiceMock = null!;
    private Mock<IAuthorizationService> _authorizationServiceMock = null!;
    private SessionBookingController _controller = null!;
    private ClaimsPrincipal _user = null!;

    [SetUp]
    public void Setup()
    {
        _sessionBookingServiceMock = new Mock<ISessionBookingService>();

        _user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "Learner")
        ], "TestAuthentication"));

        _controller = new SessionBookingController(_sessionBookingServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            }
        };
    }

    [Test]
    public async Task GetAllAvailableMentors_ReturnsOkResultWithPaginatedMentors()
    {
        // Arrange
        var request = new AvailableMentorForBookingListRequest { PageIndex = 1, PageSize = 10 };
        var mentors = new List<AvailableMentorForBookingResponse>
        {
            new()
            {
                MentorId = Guid.NewGuid(),
                MentorName = "Test Mentor",
                MentorExpertise = ["Test Expertise"],
                MentorAvatarUrl = "https://example.com/avatar",
                WorkingStartTime = TimeOnly.FromDateTime(DateTime.UtcNow),
                WorkingEndTime = TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(1))
            }
        };
        var paginatedList = new PaginatedList<AvailableMentorForBookingResponse>(mentors, 1, 1, 10);
        var serviceResult = Result.Success(paginatedList, HttpStatusCode.OK);

        _sessionBookingServiceMock.Setup(s => s.GetAllAvailableMentorsAsync(request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetAllAvailableMentors(request);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)result;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
        });
        _sessionBookingServiceMock.Verify(s => s.GetAllAvailableMentorsAsync(request), Times.Once);
    }

    [Test]
    public async Task RequestBooking_WhenValidRequest_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.Parse(_user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var request = new CreateSessionBookingRequest
        {
            TimeSlotId = Guid.NewGuid(),
            SessionType = Domain.Enums.SessionType.OneOnOne
        };
        var response = new SessionSlotStatusResponse
        {
            MentorId = Guid.NewGuid(),
            BookingStatus = Domain.Enums.SessionStatus.Processing,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };
        var serviceResult = Result.Success(response, HttpStatusCode.OK);

        _sessionBookingServiceMock.Setup(s => s.RequestBookingAsync(request, userId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.RequestBooking(request);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)result;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
        });
        _sessionBookingServiceMock.Verify(s => s.RequestBookingAsync(request, userId), Times.Once);
    }

    [Test]
    public async Task AcceptBooking_WhenValidRequest_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.Parse(_user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var bookingId = Guid.NewGuid();
        var response = new SessionSlotStatusResponse
        {
            MentorId = Guid.NewGuid(),
            BookingStatus = Domain.Enums.SessionStatus.Confirmed,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };
        var serviceResult = Result.Success(response, HttpStatusCode.OK);

        _sessionBookingServiceMock.Setup(s => s.AcceptBookingAsync(bookingId, userId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.AcceptBooking(bookingId);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)result;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
        });
        _sessionBookingServiceMock.Verify(s => s.AcceptBookingAsync(bookingId, userId), Times.Once);
    }

    [Test]
    public async Task CancelBooking_WhenValidRequest_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.Parse(_user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var bookingId = Guid.NewGuid();
        var response = new SessionSlotStatusResponse
        {
            MentorId = Guid.NewGuid(),
            BookingStatus = Domain.Enums.SessionStatus.Available,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };
        var serviceResult = Result.Success(response, HttpStatusCode.OK);

        _sessionBookingServiceMock.Setup(s => s.CancelBookingAsync(bookingId, userId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.CancelBooking(bookingId);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)result;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
        });
        _sessionBookingServiceMock.Verify(s => s.CancelBookingAsync(bookingId, userId), Times.Once);
    }
}
