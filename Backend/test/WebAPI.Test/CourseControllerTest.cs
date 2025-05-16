using System.Net;
using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Services;
using Contract.Shared;
using Domain.Enums;
using MentorPlatformAPI.Controllers;
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
        _courseController = new CourseController(_courseServiceMock.Object);
    }

    private Mock<ICourseService> _courseServiceMock;
    private CourseController _courseController;

    [Test]
    public async Task GetAll_DefaultParameters_ReturnsOkResultWithPaginatedCourses()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 5;
        var items = new List<CourseSummary>
        {
            new() { Id = Guid.NewGuid(), Title = "Course 1", Description = "Description 1" },
            new() { Id = Guid.NewGuid(), Title = "Course 2", Description = "Description 2" }
        };
        var paginatedList = new CourseListResponse(items, items.Count, pageIndex, pageSize);
        var serviceResult = Result.Success(paginatedList, HttpStatusCode.OK);

        _courseServiceMock.Setup(s => s.GetAllAsync(It.Is<CourseListRequest>(r =>
                r.PageIndex == pageIndex && r.PageSize == pageSize)))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _courseController.GetAll(new CourseListRequest
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
            Description = "Test Description"
        };
        var serviceResult = Result.Success(courseResponse, HttpStatusCode.OK);

        _courseServiceMock.Setup(s => s.GetByIdAsync(courseId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _courseController.GetById(courseId);

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
            Description = request.Description
        };
        var serviceResult = Result.Success(courseResponse, HttpStatusCode.Created);

        _courseServiceMock.Setup(s => s.CreateAsync(request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _courseController.Create(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _courseServiceMock.Verify(s => s.CreateAsync(request), Times.Once);
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
            Description = request.Description
        };
        var serviceResult = Result.Success(courseResponse, HttpStatusCode.OK);

        _courseServiceMock.Setup(s => s.UpdateAsync(courseId, request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _courseController.Update(courseId, request);

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
    public async Task Delete_CourseExists_ReturnsOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var serviceResult = Result.Success(HttpStatusCode.OK);

        _courseServiceMock.Setup(s => s.DeleteAsync(courseId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _courseController.Delete(courseId);

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
        var serviceResult = Result.Failure("Course not found", HttpStatusCode.NotFound);

        _courseServiceMock.Setup(s => s.DeleteAsync(courseId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _courseController.Delete(courseId);

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
}