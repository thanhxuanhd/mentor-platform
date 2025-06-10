using Application.Services.CourseResources;
using Contract.Dtos.CourseResources.Requests;
using Contract.Dtos.CourseResources.Responses;
using Contract.Shared;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Security.Claims;

namespace WebAPI.Test;

[TestFixture]
public class CourseResourcesControllerTests
{
    private Mock<ICourseResourceService> _courseResourceServiceMock = null!;
    private CourseResourcesController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _courseResourceServiceMock = new Mock<ICourseResourceService>();
        _controller = new CourseResourcesController(_courseResourceServiceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Test]
    public async Task GetCourseResources_ReturnsExpectedResult()
    {
        // Arrange
        var filterRequest = new FilterResourceRequest { Keyword = "test", PageIndex = 1, PageSize = 5 };
        var expectedResult = Result.Success(new PaginatedList<CourseResourceResponse>(new List<CourseResourceResponse>(), 0, 1, 5), HttpStatusCode.OK);

        _courseResourceServiceMock
            .Setup(s => s.FilterResourceAsync(It.IsAny<FilterResourceRequest>()))
            .ReturnsAsync(expectedResult);

        // Act
        var actionResult = await _controller.GetCourseResources(filterRequest) as ObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(actionResult.Value, Is.EqualTo(expectedResult));
        });
    }

    [Test]
    public async Task GetCourseResourceById_ReturnsExpectedResult()
    {
        // Arrange
        var resourceId = Guid.NewGuid();
        var expectedResult = Result.Success(new CourseResourceResponse { Id = resourceId, Title = "", CourseTitle = "", ResourceUrl = "" }, HttpStatusCode.OK);

        _courseResourceServiceMock
            .Setup(s => s.GetByIdAsync(resourceId))
            .ReturnsAsync(expectedResult);

        // Act
        var actionResult = await _controller.GetCourseResourceById(resourceId) as ObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(actionResult.Value, Is.EqualTo(expectedResult));
        });
    }

    [Test]
    public async Task CreateCourseResource_ValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var mentorId = Guid.Parse(_controller.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var resourceId = Guid.NewGuid(); // Fix: Initialize 'resourceId' to resolve CS0103
        var formData = new CourseResourceRequest
        {
            CourseId = Guid.NewGuid(),
            Title = "A",
            Description = "B",
            Resource = Mock.Of<IFormFile>() // Fix: Properly initialize the 'Resource' property
        };
        var expectedResult = Result.Success(new CourseResourceResponse { Id = resourceId, Title = "", CourseTitle = "", ResourceUrl = "" }, HttpStatusCode.Created);

        _courseResourceServiceMock
            .Setup(s => s.CreateAsync(mentorId, formData.CourseId, formData, It.IsAny<HttpRequest>()))
            .ReturnsAsync(expectedResult);

        // Act
        var actionResult = await _controller.CreateCourseResource(formData) as ObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
            Assert.That(actionResult.Value, Is.EqualTo(expectedResult));
        });
    }

    [Test]
    public async Task DeleteCourseResource_ValidRequest_ReturnsExpectedResult()
    {
        // Arrange
        var mentorId = Guid.Parse(_controller.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var resourceId = Guid.NewGuid();
        var expectedResult = Result.Success(true, HttpStatusCode.OK); // Fix: Ensure the result matches the expected Task<Result<bool>> type

        _courseResourceServiceMock
            .Setup(s => s.DeleteAsync(mentorId, resourceId))
            .ReturnsAsync(expectedResult); // Fix: Use ReturnsAsync to return a Task<Result<bool>>

        // Act
        var actionResult = await _controller.DeleteCourseResource(resourceId) as ObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(actionResult.Value, Is.EqualTo(expectedResult));
        });
    }

    [Test]
    public async Task DownloadFileAsync_ServiceThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var resourceId = Guid.NewGuid();
        var fileName = "file.txt";

        _courseResourceServiceMock
            .Setup(s => s.DownloadFileAsync(resourceId, fileName))
            .ThrowsAsync(new ArgumentException("Invalid argument"));

        // Act
        var actionResult = await _controller.DownloadFileAsync(resourceId, fileName) as BadRequestObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult!.StatusCode, Is.EqualTo(400));
            Assert.That(actionResult.Value, Is.EqualTo("Invalid argument"));
        });
    }

    [Test]
    public async Task DownloadFileAsync_ServiceThrowsNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var resourceId = Guid.NewGuid();
        var fileName = "file.txt";

        _courseResourceServiceMock
            .Setup(s => s.DownloadFileAsync(resourceId, fileName))
            .ThrowsAsync(new FileNotFoundException("File not found"));

        // Act
        var actionResult = await _controller.DownloadFileAsync(resourceId, fileName) as NotFoundObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult!.StatusCode, Is.EqualTo(404));
            Assert.That(actionResult.Value, Is.EqualTo("File not found"));
        });
    }

    [Test]
    public async Task DownloadFileAsync_ServiceThrowsUnexpectedException_ReturnsStatusCode500()
    {
        // Arrange
        var resourceId = Guid.NewGuid();
        var fileName = "file.txt";

        _courseResourceServiceMock
            .Setup(s => s.DownloadFileAsync(resourceId, fileName))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var actionResult = await _controller.DownloadFileAsync(resourceId, fileName) as ObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult!.StatusCode, Is.EqualTo(500));
            Assert.That(actionResult.Value, Is.EqualTo("An error occurred while processing your request"));
        });
    }
}

