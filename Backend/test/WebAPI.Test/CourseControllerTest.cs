using System.Net;
using System.Security.Claims;
using Contract.Dtos.CourseItems.Requests;
using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Services;
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
            new Claim(ClaimTypes.Name, "administrator"),
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

    private Mock<ICourseService> _courseServiceMock;
    private Mock<ICourseItemService> _courseItemServiceMock;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private CourseController _controller;
    private ClaimsPrincipal _user;

    private static void AssertObjectResult<TValue>(IActionResult actionResult, HttpStatusCode expectedStatusCode,
        Result<TValue> expectedServiceResult)
    {
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)expectedStatusCode));
            Assert.That(objectResult.Value, Is.EqualTo(expectedServiceResult));
        });
    }

    private static void AssertObjectResult(IActionResult actionResult, HttpStatusCode expectedStatusCode,
        Result expectedServiceResult)
    {
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)expectedStatusCode));
            Assert.That(objectResult.Value, Is.EqualTo(expectedServiceResult));
        });
    }


    private CourseSummary SetupCourseFoundAndAuthorized(Guid courseId, string policyName = "CourseModifyAccess")
    {
        var courseSummary = new CourseSummary
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
        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(Result.Success(courseSummary, HttpStatusCode.OK));
        _authorizationServiceMock.Setup(s =>
                s.AuthorizeAsync(_user, courseSummary, policyName))
            .ReturnsAsync(AuthorizationResult.Success());
        return courseSummary;
    }

    private CourseSummary SetupCourseFoundButNotAuthorized(Guid courseId, string policyName = "CourseModifyAccess")
    {
        var courseSummary = new CourseSummary
        {
            Id = courseId,
            Title = "Fetched Course (Unauthorized)",
            Description = "Fetched course description (Unauthorized)",
            MentorId = Guid.NewGuid(),
            MentorName = "Fetched Mentor (Unauthorized)",
            CategoryId = Guid.NewGuid(),
            CategoryName = "Fetched Category (Unauthorized)",
            Difficulty = CourseDifficulty.Beginner,
            DueDate = DateTime.UtcNow.AddDays(5),
            Status = CourseStatus.Draft,
            Items = [],
            Tags = ["FetchedTagUnauthorized"]
        };

        _authorizationServiceMock.Setup(s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), courseSummary, policyName))
            .ReturnsAsync(AuthorizationResult.Failed);

        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(Result.Success(courseSummary, HttpStatusCode.OK));
        _courseServiceMock.Setup(s => s.PublishCourseAsync(courseId))
            .ReturnsAsync(Result.Success(courseSummary, HttpStatusCode.OK));
        _courseServiceMock.Setup(s => s.ArchiveCourseAsync(courseId))
            .ReturnsAsync(Result.Success(courseSummary, HttpStatusCode.OK));

        return courseSummary;
    }

    private Result<CourseSummary> SetupCourseNotFound(Guid courseId)
    {
        var failureResult = Result.Failure<CourseSummary>("Course not found", HttpStatusCode.NotFound);
        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(failureResult);
        _courseServiceMock.Setup(s => s.PublishCourseAsync(courseId))
            .ReturnsAsync(failureResult);
        return failureResult;
    }

    private Result<List<CourseItemDto>> SetupCourseItemNotFound(Guid courseId)
    {
        var failureResult = Result.Failure<List<CourseItemDto>>("Course not found", HttpStatusCode.NotFound);
        _courseItemServiceMock.Setup(s => s.GetAllByCourseIdAsync(courseId))
            .ReturnsAsync(failureResult);
        return failureResult;
    }

    [Test]
    public async Task GetAll_DefaultParameters_ReturnsOkResultWithPaginatedCourses()
    {
        // Arrange
        var request = new CourseListRequest { PageIndex = 1, PageSize = 5 };
        var items = new List<CourseSummary>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Course 1",
                Description = "Description for Course 1",
                CategoryId = Guid.NewGuid(),
                CategoryName = "Category A",
                MentorId = Guid.NewGuid(),
                MentorName = "Mentor X",
                Difficulty = CourseDifficulty.Beginner,
                Status = CourseStatus.Published,
                Items = [],
                Tags = ["Tag1"]
            }
        };
        var paginatedList = new PaginatedList<CourseSummary>(items, items.Count, request.PageIndex, request.PageSize);
        var serviceResult = Result.Success(paginatedList, HttpStatusCode.OK);

        _courseServiceMock.Setup(s => s.GetAllAsync(request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetAll(request);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseServiceMock.Verify(s => s.GetAllAsync(request), Times.Once);
    }

    [Test]
    public async Task GetById_CourseExists_ReturnsOkResultWithCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var courseResponse = new CourseSummary
        {
            Id = courseId,
            Title = "Test Course",
            Description = "Full description of Test Course",
            CategoryId = Guid.NewGuid(),
            CategoryName = "Test Category",
            MentorId = Guid.NewGuid(),
            MentorName = "Test Mentor",
            Difficulty = CourseDifficulty.Intermediate,
            DueDate = DateTime.UtcNow.AddMonths(1),
            Status = CourseStatus.Draft,
            Items = [], // Initialize with empty list or sample items
            Tags = ["TestTag1", "TestTag2"] // Initialize with empty list or sample tags
        };
        var serviceResult = Result.Success(courseResponse, HttpStatusCode.OK);
        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetById(courseId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
    }

    [Test]
    public async Task GetById_CourseNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var serviceResult = SetupCourseNotFound(courseId);

        // Act
        var result = await _controller.GetById(courseId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.NotFound, serviceResult);
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
    }

    [Test]
    public async Task Create_ValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var mentorId = Guid.NewGuid();
        var request = new CourseCreateRequest
        {
            Title = "New Course",
            Description = "New Course Description",
            Difficulty = CourseDifficulty.Beginner,
            DueDate = DateTime.UtcNow,
            CategoryId = categoryId,
            MentorId = mentorId,
            Tags = ["NewTag1", "NewTag2"]
        };
        var courseResponse = new CourseSummary
        {
            Id = Guid.NewGuid(),
            CategoryName = "Existing Category",
            MentorName = "Existing Mentor",
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            MentorId = request.MentorId,
            DueDate = request.DueDate,
            Difficulty = request.Difficulty,
            Tags = request.Tags,
            Items = [], // Assuming new course has no items yet
            Status = CourseStatus.Draft // Default status for new course
        };
        var serviceResult = Result.Success(courseResponse, HttpStatusCode.Created);
        _courseServiceMock.Setup(s => s.CreateAsync(request.MentorId, request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Create(request);

        // Assert
        AssertObjectResult(result, HttpStatusCode.Created, serviceResult);
        _courseServiceMock.Verify(s => s.CreateAsync(request.MentorId, request), Times.Once);
    }

    [Test]
    public async Task Create_ServiceReturnsFailure_ReturnsAppropriateError()
    {
        // Arrange
        var request = new CourseCreateRequest
        {
            Title = "New Course",
            Description = "New Course Description",
            Difficulty = CourseDifficulty.Beginner,
            DueDate = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            MentorId = Guid.NewGuid(),
            Tags = []
        };
        var serviceResult = Result.Failure<CourseSummary>("Creation failed", HttpStatusCode.BadRequest);
        _courseServiceMock.Setup(s => s.CreateAsync(request.MentorId, request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Create(request);

        // Assert
        AssertObjectResult(result, HttpStatusCode.BadRequest, serviceResult);
        _courseServiceMock.Verify(s => s.CreateAsync(request.MentorId, request), Times.Once);
    }

    [Test]
    public async Task Update_WhenCourseFoundAndAuthorizedAndRequestIsValid_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CourseUpdateRequest
        {
            Title = "Updated Course",
            Description = "Updated Course Description",
            Difficulty = CourseDifficulty.Advanced,
            DueDate = DateTime.UtcNow.AddDays(30),
            CategoryId = Guid.NewGuid(),
            Tags = ["UpdatedTag"]
        };
        var fetchedCourse = SetupCourseFoundAndAuthorized(courseId);
        var updatedCourseResponse = new CourseSummary
        {
            Id = courseId,
            CategoryName = "Updated Category Name", // Assuming category name might change
            MentorName = fetchedCourse.MentorName,
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            MentorId = fetchedCourse.MentorId,
            DueDate = request.DueDate,
            Difficulty = request.Difficulty,
            Tags = request.Tags,
            Items = fetchedCourse.Items, // Assuming items are preserved or managed separately
            Status = fetchedCourse.Status // Assuming status isn't changed by this update call
        };
        var serviceResult = Result.Success(updatedCourseResponse, HttpStatusCode.OK);
        _courseServiceMock.Setup(s => s.UpdateAsync(courseId, request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Update(courseId, request);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
        _authorizationServiceMock.Verify(s => s.AuthorizeAsync(_user, fetchedCourse, "CourseModifyAccess"), Times.Once);
        _courseServiceMock.Verify(s => s.UpdateAsync(courseId, request), Times.Once);
    }

    [Test]
    public async Task Update_WhenCourseNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CourseUpdateRequest
        {
            Title = "Updated Course",
            Description = "NonExistent Course Description",
            Difficulty = CourseDifficulty.Beginner,
            DueDate = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Tags = []
        };
        var expectedFailureResult = SetupCourseNotFound(courseId);

        // Act
        var result = await _controller.Update(courseId, request);

        // Assert
        AssertObjectResult(result, HttpStatusCode.NotFound, expectedFailureResult);
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
        _authorizationServiceMock.Verify(
            s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        _courseServiceMock.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CourseUpdateRequest>()), Times.Never);
    }

    [Test]
    public async Task Update_WhenUserIsNotAuthorized_ReturnsForbidResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CourseUpdateRequest
        {
            Title = "Updated Course",
            Description = "Unauthorized Update Description",
            Difficulty = CourseDifficulty.Intermediate,
            DueDate = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Tags = []
        };
        var fetchedCourse = SetupCourseFoundButNotAuthorized(courseId);

        // Act
        var result = await _controller.Update(courseId, request);

        // Assert
        Assert.That(result, Is.InstanceOf<ForbidResult>());
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
        _authorizationServiceMock.Verify(s => s.AuthorizeAsync(_user, fetchedCourse, "CourseModifyAccess"), Times.Once);
        _courseServiceMock.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CourseUpdateRequest>()), Times.Never);
    }

    [Test]
    public async Task Delete_WhenCourseFoundAndAuthorized_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var fetchedCourse = SetupCourseFoundAndAuthorized(courseId);
        var serviceResult = Result.Success(true, HttpStatusCode.OK);
        _courseServiceMock.Setup(s => s.DeleteAsync(courseId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Delete(courseId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
        _authorizationServiceMock.Verify(s => s.AuthorizeAsync(_user, fetchedCourse, "CourseModifyAccess"), Times.Once);
        _courseServiceMock.Verify(s => s.DeleteAsync(courseId), Times.Once);
    }

    [Test]
    public async Task Delete_WhenCourseNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var expectedFailureResult = SetupCourseNotFound(courseId);

        // Act
        var result = await _controller.Delete(courseId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.NotFound, expectedFailureResult);
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
        _authorizationServiceMock.Verify(
            s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        _courseServiceMock.Verify(s => s.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task Delete_WhenUserIsNotAuthorized_ReturnsForbidResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var fetchedCourse = SetupCourseFoundButNotAuthorized(courseId);

        // Act
        var result = await _controller.Delete(courseId);

        // Assert
        Assert.That(result, Is.InstanceOf<ForbidResult>());
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
        _authorizationServiceMock.Verify(s => s.AuthorizeAsync(_user, fetchedCourse, "CourseModifyAccess"), Times.Once);
        _courseServiceMock.Verify(s => s.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task PublishCourse_WhenCourseNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var expectedFailureResult = SetupCourseNotFound(courseId);

        // Act
        var result = await _controller.PublishCourse(courseId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.NotFound, expectedFailureResult);
        _courseServiceMock.Verify(s => s.PublishCourseAsync(courseId), Times.Once);
        _authorizationServiceMock.Verify(
            s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()), Times.Never);
    }


    [Test]
    public async Task ArchiveCourse_WhenCourseFoundAndAuthorized_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var fetchedCourse = SetupCourseFoundAndAuthorized(courseId); // Initial status is Draft
        var archivedCourse = new CourseSummary
        {
            Id = courseId,
            Title = fetchedCourse.Title,
            Description = fetchedCourse.Description,
            CategoryId = fetchedCourse.CategoryId,
            CategoryName = fetchedCourse.CategoryName,
            MentorId = fetchedCourse.MentorId,
            MentorName = fetchedCourse.MentorName,
            Difficulty = fetchedCourse.Difficulty,
            DueDate = fetchedCourse.DueDate,
            Status = CourseStatus.Archived, // Status changed
            Items = fetchedCourse.Items,
            Tags = fetchedCourse.Tags
        };
        var serviceResult = Result.Success(archivedCourse, HttpStatusCode.OK);
        _courseServiceMock.Setup(s => s.ArchiveCourseAsync(courseId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ArchiveCourse(courseId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
        _authorizationServiceMock.Verify(s => s.AuthorizeAsync(_user, fetchedCourse, "CourseModifyAccess"), Times.Once);
        _courseServiceMock.Verify(s => s.ArchiveCourseAsync(courseId), Times.Once);
    }

    [Test]
    public async Task ArchiveCourse_WhenCourseNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var expectedFailureResult = SetupCourseNotFound(courseId);
        _courseServiceMock.Setup(s => s.ArchiveCourseAsync(courseId))
            .ReturnsAsync(Result.Failure<CourseSummary>("Cannot archive non-existent course", HttpStatusCode.NotFound));

        // Act
        var result = await _controller.ArchiveCourse(courseId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.NotFound, expectedFailureResult);
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
        _authorizationServiceMock.Verify(
            s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        _courseServiceMock.Verify(s => s.ArchiveCourseAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task ArchiveCourse_WhenUserIsNotAuthorized_ReturnsForbidResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var fetchedCourse = SetupCourseFoundButNotAuthorized(courseId);

        // Act
        var result = await _controller.ArchiveCourse(courseId);

        // Assert
        Assert.That(result, Is.InstanceOf<ForbidResult>());
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
        _authorizationServiceMock.Verify(s => s.AuthorizeAsync(_user, fetchedCourse, "CourseModifyAccess"), Times.Once);
        _courseServiceMock.Verify(s => s.ArchiveCourseAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task GetAllCourseItem_WhenCourseAccessible_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var fetchedCourse = SetupCourseFoundAndAuthorized(courseId);
        var items = new List<CourseItemDto>
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
        var serviceResult = Result.Success(items, HttpStatusCode.OK);
        _courseItemServiceMock.Setup(s => s.GetAllByCourseIdAsync(courseId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetAllCourseItem(courseId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseItemServiceMock.Verify(s => s.GetAllByCourseIdAsync(courseId), Times.Once);
    }

    [Test]
    public async Task GetAllCourseItem_WhenCourseNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var expectedFailureResult = SetupCourseItemNotFound(courseId);

        // Act
        var result = await _controller.GetAllCourseItem(courseId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.NotFound, expectedFailureResult);
        _courseItemServiceMock.Verify(s => s.GetAllByCourseIdAsync(courseId), Times.Once);
    }

    [Test]
    public async Task GetCourseItemById_WhenCourseAccessibleAndItemExists_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var fetchedCourse = SetupCourseFoundAndAuthorized(courseId);
        var item = new CourseItemDto
        {
            Id = resourceId,
            Title = "Test Item",
            Description = "Description of Test Item",
            MediaType = CourseMediaType.Pdf,
            WebAddress = "http://example.com/testitem"
        };
        var serviceResult = Result.Success(item, HttpStatusCode.OK);
        _courseItemServiceMock.Setup(s => s.GetByIdAsync(courseId, resourceId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetCourseItemById(courseId, resourceId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseItemServiceMock.Verify(s => s.GetByIdAsync(courseId, resourceId), Times.Once);
    }

    [Test]
    public async Task CreateCourseItem_WhenCourseAuthorizedAndRequestValid_ReturnsCreatedResult()
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
        var fetchedCourse = SetupCourseFoundAndAuthorized(courseId);
        var createdItem = new CourseItemDto
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
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
        _authorizationServiceMock.Verify(s => s.AuthorizeAsync(_user, fetchedCourse, "CourseModifyAccess"), Times.Once);
        _courseItemServiceMock.Verify(s => s.CreateAsync(courseId, request), Times.Once);
    }

    [Test]
    public async Task UpdateCourseItem_WhenCourseAuthorizedAndRequestValid_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var request = new CourseItemUpdateRequest
        {
            Title = "Updated Item",
            Description = "Updated Item Description",
            MediaType = CourseMediaType.Pdf,
            WebAddress = "http://example.com/updated"
        };
        var fetchedCourse = SetupCourseFoundAndAuthorized(courseId);
        var updatedItem = new CourseItemDto
        {
            Id = resourceId,
            Title = request.Title,
            Description = request.Description,
            MediaType = request.MediaType,
            WebAddress = request.WebAddress
        };
        var serviceResult = Result.Success(updatedItem, HttpStatusCode.OK);
        _courseItemServiceMock.Setup(s => s.UpdateAsync(courseId, resourceId, request)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.UpdateCourseItem(courseId, resourceId, request);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _authorizationServiceMock.Verify(s => s.AuthorizeAsync(_user, fetchedCourse, "CourseModifyAccess"), Times.Once);
        _courseItemServiceMock.Verify(s => s.UpdateAsync(courseId, resourceId, request), Times.Once);
    }

    [Test]
    public async Task DeleteCourseItem_WhenCourseAuthorized_ReturnsOk()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var fetchedCourse = SetupCourseFoundAndAuthorized(courseId);
        var serviceResult = Result.Success(true, HttpStatusCode.OK);
        _courseItemServiceMock.Setup(s => s.DeleteAsync(courseId, resourceId)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.DeleteCourseItem(courseId, resourceId);

        // Assert
        AssertObjectResult(result, HttpStatusCode.OK, serviceResult);
        _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
        _authorizationServiceMock.Verify(s => s.AuthorizeAsync(_user, fetchedCourse, "CourseModifyAccess"), Times.Once);
        _courseItemServiceMock.Verify(s => s.DeleteAsync(courseId, resourceId), Times.Once);
    }
}