using Application.Services.CourseResources;
using Application.Services.Courses;
using Contract.Dtos.CourseResources.Requests;
using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Shared;
using Domain.Enums;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Security.Claims;

namespace WebAPI.Test;

[TestFixture]
public class CourseControllerTest
{
    [SetUp]
    public void Setup()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _courseResourceServiceMock = new Mock<ICourseResourceService>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();

        _user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, nameof(UserRole.Admin))
        ], "TestAuthentication"));

        _controller = new CourseController(_courseServiceMock.Object, _courseResourceServiceMock.Object,
            _authorizationServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            }
        };
    }

    private Mock<ICourseService> _courseServiceMock = null!;
    private Mock<ICourseResourceService> _courseResourceServiceMock = null!;
    private Mock<IAuthorizationService> _authorizationServiceMock = null!;
    private CourseController _controller = null!;
    private ClaimsPrincipal _user = null!;

    private static void AssertObjectResult<TValue>(IActionResult actionResult, HttpStatusCode expectedStatusCode,
        Result<TValue> expectedServiceResult)
    {
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)expectedStatusCode));
            Assert.That(objectResult.Value, Is.EqualTo(expectedServiceResult));
        }
    }

    private static void AssertObjectResult(IActionResult actionResult, HttpStatusCode expectedStatusCode,
        Result expectedServiceResult)
    {
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)expectedStatusCode));
            Assert.That(objectResult.Value, Is.EqualTo(expectedServiceResult));
        }
    }

    private CourseSummaryResponse SetupCourseFoundAndAuthorized(Guid courseId, string policyName = "CourseModifyAccess")
    {
        var courseSummary = new CourseSummaryResponse
        {
            Id = courseId,
            Title = "Fetched Course",
            Description = "Fetched course description",
            MentorId = Guid.NewGuid(),
            MentorName = "Fetched Mentor",
            CategoryId = Guid.NewGuid(),
            CategoryName = "Fetched Category",
            Difficulty = CourseDifficulty.Intermediate,
            DueDate = DateTime.UtcNow.AddDays(10),
            Status = CourseStatus.Draft,
            Resources = new List<CourseResourceResponse>(),
            Tags = new List<string> { "FetchedTag" }
        };

        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(Result.Success(courseSummary, HttpStatusCode.OK));
        _authorizationServiceMock.Setup(s =>
                s.AuthorizeAsync(_user, courseSummary, policyName))
            .ReturnsAsync(AuthorizationResult.Success());

        return courseSummary;
    }

    private Result<CourseSummaryResponse> SetupCourseNotFound(Guid courseId)
    {
        var failureResult = Result.Failure<CourseSummaryResponse>("Course not found", HttpStatusCode.NotFound);
        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(failureResult);
        return failureResult;
    }

    [Test]
    public async Task GetAll_DefaultParameters_ReturnsOkResultWithPaginatedCourses()
    {
        // Arrange
        var request = new CourseListRequest { PageIndex = 1, PageSize = 10 };
        var courses = new PaginatedList<CourseSummaryResponse>(new List<CourseSummaryResponse>(), 0, 1, 10);
        var serviceResult = Result.Success(courses, HttpStatusCode.OK);
        var userId = Guid.Parse(_user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = Enum.Parse<UserRole>(_user.FindFirst(ClaimTypes.Role)!.Value);

        _courseServiceMock.Setup(s => s.GetAllAsync(userId, role, request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetAll(request);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseServiceMock.Verify(s => s.GetAllAsync(userId, role, request), Times.Once);
    }

    [Test]
    public async Task GetById_CourseExists_ReturnsOkResultWithCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var courseResponse = new CourseSummaryResponse
        {
            Id = courseId,
            Title = "Test Course",
            Description = "Test Course Description",
            CategoryId = Guid.NewGuid(),
            CategoryName = "Test Category",
            MentorId = Guid.NewGuid(),
            MentorName = "Test Mentor",
            Difficulty = CourseDifficulty.Intermediate,
            DueDate = DateTime.UtcNow.AddMonths(1),
            Status = CourseStatus.Draft,
            Resources = new List<CourseResourceResponse>(),
            Tags = new List<string> { "TestTag1", "TestTag2" }
        };
        var serviceResult = Result.Success(courseResponse, HttpStatusCode.OK);
        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetById(courseId); // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)result;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
        });
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
    }

    [Test]
    public async Task GetAllCourseResource_WhenCourseAccessible_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var courseSummary = new CourseSummaryResponse
        {
            Id = courseId,
            Title = "Fetched Course",
            Description = "Fetched course description",
            MentorId = Guid.NewGuid(),
            MentorName = "Fetched Mentor",
            CategoryId = Guid.NewGuid(),
            CategoryName = "Fetched Category",
            Difficulty = CourseDifficulty.Intermediate,
            DueDate = DateTime.UtcNow.AddDays(10),
            Status = CourseStatus.Draft,
            Resources = [],
            Tags = ["FetchedTag"]
        };
        var items = new List<CourseResourceResponse>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Resource 1",
                Description = "Description of Resource 1",
                ResourceType = FileType.Video,
                ResourceUrl = "http://example.com/item1"
            }
        };
        var courseServiceResult = Result.Success(courseSummary, HttpStatusCode.OK);
        var courseResourceServiceResult = Result.Success(items, HttpStatusCode.OK);

        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(courseServiceResult);
        _courseResourceServiceMock.Setup(s => s.GetAllByCourseIdAsync(courseId))
            .ReturnsAsync(courseResourceServiceResult);


        // Act
        var result = await _controller.GetAllCourseResource(courseId);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)result;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(courseResourceServiceResult));
        });
        _courseResourceServiceMock.Verify(s => s.GetAllByCourseIdAsync(courseId), Times.Once);
    }

    [Test]
    public async Task GetCourseResourceById_WhenResourceExists_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var courseSummary = new CourseSummaryResponse
        {
            Id = courseId,
            Title = "Fetched Course",
            Description = "Fetched course description",
            MentorId = Guid.NewGuid(),
            MentorName = "Fetched Mentor",
            CategoryId = Guid.NewGuid(),
            CategoryName = "Fetched Category",
            Difficulty = CourseDifficulty.Intermediate,
            DueDate = DateTime.UtcNow.AddDays(10),
            Status = CourseStatus.Draft,
            Resources = [],
            Tags = ["FetchedTag"]
        };
        var itemId = Guid.NewGuid();
        var item = new CourseResourceResponse
        {
            Id = itemId,
            Title = "Test Resource",
            Description = "Description of Test Resource",
            ResourceType = FileType.Pdf,
            ResourceUrl = "http://example.com/testitem"
        };
        var serviceResult = Result.Success(item, HttpStatusCode.OK);
        _courseResourceServiceMock.Setup(s => s.GetByIdAsync(itemId)).ReturnsAsync(serviceResult);
        var courseSummaryServiceResult = Result.Success(courseSummary, HttpStatusCode.OK);
        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(courseSummaryServiceResult);

        // Act
        var result = await _controller.GetCourseResourceById(courseId, itemId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseResourceServiceMock.Verify(s => s.GetByIdAsync(itemId), Times.Once);
    }

    [Test]
    public async Task CreateCourseResource_WhenRequestValid_ReturnsCreatedResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CourseResourceCreateRequest
        {
            Title = "New Resource",
            Description = "New Resource Description",
            ResourceType = FileType.Video,
            ResourceUrl = "http://example.com/new"
        };

        var createdResource = new CourseResourceResponse
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            ResourceType = request.ResourceType,
            ResourceUrl = request.ResourceUrl
        };
        var serviceResult = Result.Success(createdResource, HttpStatusCode.Created);
        _courseResourceServiceMock.Setup(s => s.CreateAsync(courseId, request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.CreateCourseResource(courseId, request);

        // Assert
        AssertObjectResult(result, HttpStatusCode.Created, serviceResult);
        _courseResourceServiceMock.Verify(s => s.CreateAsync(courseId, request), Times.Once);
    }

    [Test]
    public async Task UpdateCourseResource_WhenRequestValid_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var request = new CourseResourceUpdateRequest
        {
            Title = "Updated Resource",
            Description = "Updated Resource Description",
            ResourceType = FileType.Pdf,
            ResourceUrl = "http://example.com/updated"
        };

        var updatedResource = new CourseResourceResponse
        {
            Id = itemId,
            Title = request.Title,
            Description = request.Description,
            ResourceType = request.ResourceType,
            ResourceUrl = request.ResourceUrl
        };
        var serviceResult = Result.Success(updatedResource, HttpStatusCode.OK);
        _courseResourceServiceMock.Setup(s => s.UpdateAsync(itemId, request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.UpdateCourseResource(courseId, itemId, request);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseResourceServiceMock.Verify(s => s.UpdateAsync(itemId, request), Times.Once);
    }

    [Test]
    public async Task DeleteCourseResource_WhenResourceExists_ReturnsOk()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var serviceResult = Result.Success(true, HttpStatusCode.OK);
        _courseResourceServiceMock.Setup(s => s.DeleteAsync(itemId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.DeleteCourseResource(courseId, itemId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseResourceServiceMock.Verify(s => s.DeleteAsync(itemId), Times.Once);
    }
}