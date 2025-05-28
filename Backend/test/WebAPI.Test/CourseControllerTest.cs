using System.Net;
using System.Security.Claims;
using Application.Services.CourseItems;
using Application.Services.Courses;
using Contract.Dtos.CourseItems.Requests;
using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Shared;
using Domain.Enums;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace WebAPI.Test;

[TestFixture]
public class CourseControllerTest
{
    [SetUp]
    public void Setup()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _courseItemServiceMock = new Mock<ICourseItemService>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();

        _user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, nameof(UserRole.Admin))
        ], "TestAuthentication"));

        _controller = new CourseController(_courseServiceMock.Object, _courseItemServiceMock.Object,
            _authorizationServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            }
        };
    }

    private Mock<ICourseService> _courseServiceMock = null!;
    private Mock<ICourseItemService> _courseItemServiceMock = null!;
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
            Items = new List<CourseItemResponse>(),
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
            Items = new List<CourseItemResponse>(),
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
    public async Task GetAllCourseItem_WhenCourseAccessible_ReturnsOkResult()
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
            Items = [],
            Tags = ["FetchedTag"]
        };
        var items = new List<CourseItemResponse>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Item 1",
                Description = "Description of Item 1",
                MediaType = CourseMediaType.Video,
                WebAddress = "http://example.com/item1"
            }
        };
        var courseServiceResult = Result.Success(courseSummary, HttpStatusCode.OK);
        var courseItemServiceResult = Result.Success(items, HttpStatusCode.OK);

        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(courseServiceResult);
        _courseItemServiceMock.Setup(s => s.GetAllByCourseIdAsync(courseId))
            .ReturnsAsync(courseItemServiceResult);


        // Act
        var result = await _controller.GetAllCourseItem(courseId);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)result;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(courseItemServiceResult));
        });
        _courseItemServiceMock.Verify(s => s.GetAllByCourseIdAsync(courseId), Times.Once);
    }

    [Test]
    public async Task GetCourseItemById_WhenItemExists_ReturnsOkResult()
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
            Items = [],
            Tags = ["FetchedTag"]
        };
        var itemId = Guid.NewGuid();
        var item = new CourseItemResponse
        {
            Id = itemId,
            Title = "Test Item",
            Description = "Description of Test Item",
            MediaType = CourseMediaType.Pdf,
            WebAddress = "http://example.com/testitem"
        };
        var serviceResult = Result.Success(item, HttpStatusCode.OK);
        _courseItemServiceMock.Setup(s => s.GetByIdAsync(itemId)).ReturnsAsync(serviceResult);
        var courseSummaryServiceResult = Result.Success(courseSummary, HttpStatusCode.OK);
        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(courseSummaryServiceResult);

        // Act
        var result = await _controller.GetCourseItemById(courseId, itemId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseItemServiceMock.Verify(s => s.GetByIdAsync(itemId), Times.Once);
    }

    [Test]
    public async Task CreateCourseItem_WhenRequestValid_ReturnsCreatedResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CourseItemCreateRequest
        {
            Title = "New Item",
            Description = "New Item Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "http://example.com/new"
        };

        var createdItem = new CourseItemResponse
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            MediaType = request.MediaType,
            WebAddress = request.WebAddress
        };
        var serviceResult = Result.Success(createdItem, HttpStatusCode.Created);
        _courseItemServiceMock.Setup(s => s.CreateAsync(courseId, request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.CreateCourseItem(courseId, request);

        // Assert
        AssertObjectResult(result, HttpStatusCode.Created, serviceResult);
        _courseItemServiceMock.Verify(s => s.CreateAsync(courseId, request), Times.Once);
    }

    [Test]
    public async Task UpdateCourseItem_WhenRequestValid_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var request = new CourseItemUpdateRequest
        {
            Title = "Updated Item",
            Description = "Updated Item Description",
            MediaType = CourseMediaType.Pdf,
            WebAddress = "http://example.com/updated"
        };

        var updatedItem = new CourseItemResponse
        {
            Id = itemId,
            Title = request.Title,
            Description = request.Description,
            MediaType = request.MediaType,
            WebAddress = request.WebAddress
        };
        var serviceResult = Result.Success(updatedItem, HttpStatusCode.OK);
        _courseItemServiceMock.Setup(s => s.UpdateAsync(itemId, request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.UpdateCourseItem(courseId, itemId, request);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseItemServiceMock.Verify(s => s.UpdateAsync(itemId, request), Times.Once);
    }

    [Test]
    public async Task DeleteCourseItem_WhenItemExists_ReturnsOk()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var serviceResult = Result.Success(true, HttpStatusCode.OK);
        _courseItemServiceMock.Setup(s => s.DeleteAsync(itemId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.DeleteCourseItem(courseId, itemId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseItemServiceMock.Verify(s => s.DeleteAsync(itemId), Times.Once);
    }
}