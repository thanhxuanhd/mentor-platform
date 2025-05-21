using System.Net;
using System.Security.Claims;
using Contract.Dtos.CourseItems.Requests;
using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Services;
using Contract.Shared;
using Domain.Enums;
using Infrastructure.Services.Authorization;
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
        _controller = new CourseController(_courseServiceMock.Object, _courseItemServiceMock.Object,
            _authorizationServiceMock.Object);
        
        _controller.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([
                new Claim(ClaimTypes.Role, nameof(UserRole.Admin))
            ]))
        };
    }

    private Mock<ICourseService> _courseServiceMock;
    private Mock<ICourseItemService> _courseItemServiceMock;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private CourseController _controller;

    [Test]
    public async Task GetAll_DefaultParameters_ReturnsOkResultWithPaginatedCourses()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 5;
        var items = new List<CourseSummary>
        {
            new() { Id = Guid.NewGuid(), Title = "Course 1", Description = "Description 1", Items = [] },
            new() { Id = Guid.NewGuid(), Title = "Course 2", Description = "Description 2", Items = [] }
        };
        var paginatedList = new PaginatedList<CourseSummary>(items, items.Count, pageIndex, pageSize);
        var serviceResult = Result.Success(paginatedList, HttpStatusCode.OK);

        _courseServiceMock.Setup(s => s.GetAllAsync(It.Is<CourseListRequest>(r =>
                r.PageIndex == pageIndex && r.PageSize == pageSize)))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetAll(new CourseListRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.GetAllAsync(It.Is<CourseListRequest>(r =>
                r.PageIndex == pageIndex && r.PageSize == pageSize)), Times.Once);
        });
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
            Description = "Test Description",
            Items = []
        };
        var serviceResult = Result.Success(courseResponse, HttpStatusCode.OK);

        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetById(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
        });
    }

    [Test]
    public async Task GetById_CourseNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var serviceResult = Result.Failure<CourseSummary>("Course not found", HttpStatusCode.NotFound);

        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetById(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.GetByIdAsync(courseId), Times.Once);
        });
    }

    [Test]
    public async Task GetAll_WithAllFilters_ReturnsFilteredResults()
    {
        // Arrange
        var request = new CourseListRequest
        {
            PageIndex = 1,
            PageSize = 10,
            CategoryId = Guid.NewGuid(),
            MentorId = Guid.NewGuid(),
            Keyword = "test",
            Status = CourseStatus.Published,
            Difficulty = CourseDifficulty.Intermediate
        };

        var items = new List<CourseSummary>
        {
            new() { Id = Guid.NewGuid(), Title = "Filtered Course", Description = "Description", Items = [] }
        };
        var paginatedList = new PaginatedList<CourseSummary>(items, items.Count, request.PageIndex, request.PageSize);
        var serviceResult = Result.Success(paginatedList, HttpStatusCode.OK);

        _courseServiceMock.Setup(s => s.GetAllAsync(It.Is<CourseListRequest>(r =>
                r.PageIndex == request.PageIndex &&
                r.PageSize == request.PageSize &&
                r.CategoryId == request.CategoryId &&
                r.MentorId == request.MentorId &&
                r.Keyword == request.Keyword &&
                r.Status == request.Status &&
                r.Difficulty == request.Difficulty)))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetAll(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.GetAllAsync(It.Is<CourseListRequest>(r =>
                r.PageIndex == request.PageIndex &&
                r.PageSize == request.PageSize &&
                r.CategoryId == request.CategoryId &&
                r.MentorId == request.MentorId &&
                r.Keyword == request.Keyword &&
                r.Status == request.Status &&
                r.Difficulty == request.Difficulty)), Times.Once);
        });
    }

    [Test]
    public async Task Create_ValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var request = new CourseCreateRequest
        {
            Title = "New Course",
            Description = "New Description",
            CategoryId = Guid.NewGuid(),
            MentorId = Guid.NewGuid(),
            DueDate = DateTime.Now,
            Difficulty = CourseDifficulty.Beginner
        };
        var courseResponse = new CourseSummary
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Items = []
        };
        var serviceResult = Result.Success(courseResponse, HttpStatusCode.Created);

        _courseServiceMock.Setup(s => s.CreateAsync(request.MentorId, request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Create(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.CreateAsync(request.MentorId, request), Times.Once);
        });
    }

    [Test]
    public async Task Create_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new CourseCreateRequest
        {
            Title = "New Course",
            Description = "New Description",
            CategoryId = Guid.NewGuid(),
            MentorId = Guid.NewGuid(),
            DueDate = DateTime.Now,
            Difficulty = CourseDifficulty.Beginner
        };

        var serviceResult = Result.Failure<CourseSummary>("Invalid request", HttpStatusCode.BadRequest);

        _courseServiceMock.Setup(s => s.CreateAsync(request.MentorId, request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Create(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.CreateAsync(request.MentorId, request), Times.Once);
        });
    }

    [Test]
    public async Task Update_CourseExistsAndRequestIsValid_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CourseUpdateRequest
        {
            Title = "Updated Course",
            Description = "Updated Description",
            CategoryId = Guid.NewGuid(),
            DueDate = DateTime.Now,
            Difficulty = CourseDifficulty.Beginner
        };
        var courseResponse = new CourseSummary
        {
            Id = courseId,
            Title = request.Title,
            Description = request.Description,
            Items = []
        };
        var serviceResult = Result.Success(courseResponse, HttpStatusCode.OK);

        _courseServiceMock.Setup(s => s.UpdateAsync(courseId, request))
            .ReturnsAsync(serviceResult);
        
        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(serviceResult);

        _authorizationServiceMock.Setup(s => s.AuthorizeAsync(_controller.ControllerContext.HttpContext.User, serviceResult.Value, "CourseModifyAccess"))
            .ReturnsAsync(AuthorizationResult.Success);


        // Act
        var result = await _controller.Update(courseId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.UpdateAsync(courseId, request), Times.Once);
        });
    }

    [Test]
    public async Task Update_CourseNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CourseUpdateRequest
        {
            Title = "Updated Course",
            Description = "Updated Description",
            CategoryId = Guid.NewGuid(),
            DueDate = DateTime.Now,
            Difficulty = CourseDifficulty.Beginner
        };

        var serviceResult = Result.Failure<CourseSummary>("Course not found", HttpStatusCode.NotFound);

        _courseServiceMock.Setup(s => s.UpdateAsync(courseId, request))
            .ReturnsAsync(serviceResult);
        
        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(serviceResult);

        _authorizationServiceMock.Setup(s => s.AuthorizeAsync(_controller.ControllerContext.HttpContext.User, serviceResult.Value, "CourseModifyAccess"))
            .ReturnsAsync(AuthorizationResult.Success);
        
        // Act
        var result = await _controller.Update(courseId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.UpdateAsync(courseId, request), Times.Once);
        });
    }

    [Test]
    public async Task Update_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CourseUpdateRequest
        {
            Title = "Updated Course",
            Description = "Updated Description",
            CategoryId = Guid.NewGuid(),
            DueDate = DateTime.Now,
            Difficulty = CourseDifficulty.Beginner
        };

        var serviceResult = Result.Failure<CourseSummary>("Invalid request", HttpStatusCode.BadRequest);

        _courseServiceMock.Setup(s => s.UpdateAsync(courseId, request))
            .ReturnsAsync(serviceResult);
        
        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(serviceResult);

        _authorizationServiceMock.Setup(s => s.AuthorizeAsync(_controller.ControllerContext.HttpContext.User, serviceResult.Value, "CourseModifyAccess"))
            .ReturnsAsync(AuthorizationResult.Success);


        // Act
        var result = await _controller.Update(courseId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.UpdateAsync(courseId, request), Times.Once);
        });
    }

    [Test]
    public async Task Delete_CourseExists_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var serviceResult = Result.Success(true, HttpStatusCode.OK);

        _courseServiceMock.Setup(s => s.DeleteAsync(courseId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Delete(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.DeleteAsync(courseId), Times.Once);
        });
    }

    [Test]
    public async Task Delete_CourseDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var serviceResult = Result.Failure<bool>("Course not found", HttpStatusCode.NotFound);

        _courseServiceMock.Setup(s => s.DeleteAsync(courseId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Delete(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.DeleteAsync(courseId), Times.Once);
        });
    }

    [Test]
    public async Task GetAllCourseItem_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var items = new List<CourseItemDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Item 1",
                Description = "Description 1",
                MediaType = CourseMediaType.Video,
                WebAddress = "http://example.com/1"
            }
        };
        var serviceResult = Result.Success(items, HttpStatusCode.OK);

        _courseItemServiceMock.Setup(s => s.GetAllByCourseIdAsync(courseId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetAllCourseItem(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
        });
    }

    [Test]
    public async Task GetCourseItemById_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var item = new CourseItemDto
        {
            Id = resourceId,
            Title = "Test Item",
            Description = "Test Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "http://example.com"
        };
        var serviceResult = Result.Success(item, HttpStatusCode.OK);

        _courseItemServiceMock.Setup(s => s.GetByIdAsync(courseId, resourceId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetCourseItemById(courseId, resourceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
        });
    }

    [Test]
    public async Task CreateCourseItem_ReturnsCreatedResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CourseItemCreateRequest
        {
            Title = "New Item",
            Description = "New Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "http://example.com"
        };
        var createdItem = new CourseItemDto
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            MediaType = request.MediaType,
            WebAddress = request.WebAddress
        };
        var serviceResult = Result.Success(createdItem, HttpStatusCode.Created);

        _courseItemServiceMock.Setup(s => s.CreateAsync(courseId, request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.CreateCourseItem(courseId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
        });
    }

    [Test]
    public async Task UpdateCourseItem_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var request = new CourseItemUpdateRequest
        {
            Title = "Updated Item",
            Description = "Updated Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "http://example.com"
        };
        var updatedItem = new CourseItemDto
        {
            Id = resourceId,
            Title = request.Title,
            Description = request.Description,
            MediaType = request.MediaType,
            WebAddress = request.WebAddress
        };
        var serviceResult = Result.Success(updatedItem, HttpStatusCode.OK);

        _courseItemServiceMock.Setup(s => s.UpdateAsync(courseId, resourceId, request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.UpdateCourseItem(courseId, resourceId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
        });
    }

    [Test]
    public async Task DeleteCourseItem_ReturnsNoContentResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var serviceResult = Result.Success(HttpStatusCode.NoContent);

        _courseItemServiceMock.Setup(s => s.DeleteAsync(courseId, resourceId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.DeleteCourseItem(courseId, resourceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.NoContent));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
        });
    }

    [Test]
    public async Task PublishCourse_WhenCourseExists_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var courseResponse = new CourseSummary
        {
            Id = courseId,
            Status = CourseStatus.Published
        };
        var serviceResult = Result.Success(courseResponse, HttpStatusCode.OK);

        _courseServiceMock.Setup(s => s.PublishCourseAsync(courseId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.PublishCourse(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.PublishCourseAsync(courseId), Times.Once);
        });
    }

    [Test]
    public async Task PublishCourse_WhenCourseNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var serviceResult = Result.Failure<CourseSummary>("Course not found", HttpStatusCode.NotFound);

        _courseServiceMock.Setup(s => s.PublishCourseAsync(courseId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.PublishCourse(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.PublishCourseAsync(courseId), Times.Once);
        });
    }

    [Test]
    public async Task ArchiveCourse_WhenCourseExists_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var courseResponse = new CourseSummary
        {
            Id = courseId,
            Status = CourseStatus.Archived
        };
        var serviceResult = Result.Success(courseResponse, HttpStatusCode.OK);
        
        _courseServiceMock.Setup(s => s.ArchiveCourseAsync(courseId))
            .ReturnsAsync(serviceResult);
        
        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(serviceResult);

        _authorizationServiceMock.Setup(s => s.AuthorizeAsync(_controller.ControllerContext.HttpContext.User, serviceResult.Value, "CourseModifyAccess"))
            .ReturnsAsync(AuthorizationResult.Success);

        // Act
        var result = await _controller.ArchiveCourse(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.ArchiveCourseAsync(courseId), Times.Once);
        });
    }

    [Test]
    public async Task ArchiveCourse_WhenCourseNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var serviceResult = Result.Failure<CourseSummary>("Course not found", HttpStatusCode.NotFound);

        _courseServiceMock.Setup(s => s.ArchiveCourseAsync(courseId))
            .ReturnsAsync(serviceResult);
        
        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(serviceResult);

        _authorizationServiceMock.Setup(s => s.AuthorizeAsync(_controller.ControllerContext.HttpContext.User, serviceResult.Value, "CourseModifyAccess"))
            .ReturnsAsync(AuthorizationResult.Success);


        // Act
        var result = await _controller.ArchiveCourse(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.ArchiveCourseAsync(courseId), Times.Once);
        });
    }
}