using Application.Services.MentorApplications;
using Contract.Dtos.MentorApplications.Requests;
using Contract.Dtos.MentorApplications.Responses;
using Contract.Shared;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Security.Claims;

namespace WebAPI.Test;

[TestFixture]
public class MentorApplicationControllerTests
{
    private Mock<IMentorApplicationService> _mockMentorApplicationService;
    private MentorApplicationController _mentorApplicationController;

    [SetUp]
    public void Setup()
    {
        _mockMentorApplicationService = new Mock<IMentorApplicationService>();
        _mentorApplicationController = new MentorApplicationController(_mockMentorApplicationService.Object);

        // Mock HttpContext for User Claims
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
        }, "mock"));

        _mentorApplicationController.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    [Test]
    public async Task GetAllMentorApplications_ReturnsOkResult_WithPaginatedApplications()
    {
        // Arrange
        var request = new FilterMentorApplicationRequest();
        var response = new PaginatedList<FilterMentorApplicationResponse>(new List<FilterMentorApplicationResponse>(), 0, 1, 10);
        var result = Result.Success(response, HttpStatusCode.OK);
        _mockMentorApplicationService.Setup(s => s.GetAllMentorApplicationsAsync(request)).ReturnsAsync(result);

        // Act
        var actionResult = await _mentorApplicationController.GetAllMentorApplications(request);

        // Assert
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(objectResult.Value, Is.EqualTo(result));
    }

    [Test]
    public async Task GetMentorApplicationById_ReturnsOkResult_WithApplicationDetail()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var response = new MentorApplicationDetailResponse { MentorApplicationId = applicationId };
        var result = Result.Success(response, HttpStatusCode.OK);
        _mockMentorApplicationService.Setup(s => s.GetMentorApplicationByIdAsync(It.IsAny<Guid>(), applicationId)).ReturnsAsync(result);

        // Act
        var actionResult = await _mentorApplicationController.GetMentorApplicationById(applicationId);

        // Assert
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(objectResult.Value, Is.EqualTo(result));
    }

    [Test]
    public async Task RequestApplicationInfo_ReturnsOkResult_WithSuccessMessage()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var request = new RequestApplicationInfoRequest { Note = "Please provide more details." };
        var response = new RequestApplicationInfoResponse { Message = "Information request sent." };
        var result = Result.Success(response, HttpStatusCode.OK);
        _mockMentorApplicationService.Setup(s => s.RequestApplicationInfoAsync(adminId, applicationId, request)).ReturnsAsync(result);

        // Act
        var actionResult = await _mentorApplicationController.RequestApplicationInfo(applicationId, request);

        // Assert
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(objectResult.Value, Is.EqualTo(result));
    }

    [Test]
    public async Task UpdateApplicationStatus_ReturnsOkResult_WithSuccessMessage()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var request = new UpdateApplicationStatusRequest { Status = Domain.Enums.ApplicationStatus.Approved, Note = "Approved" };
        var response = new UpdateApplicationStatusResponse { Message = "Status updated." };
        var result = Result.Success(response, HttpStatusCode.OK);
        _mockMentorApplicationService.Setup(s => s.UpdateApplicationStatusAsync(adminId, applicationId, request)).ReturnsAsync(result);

        // Act
        var actionResult = await _mentorApplicationController.UpdateApplicationStatus(applicationId, request);

        // Assert
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(objectResult.Value, Is.EqualTo(result));
    }
}
