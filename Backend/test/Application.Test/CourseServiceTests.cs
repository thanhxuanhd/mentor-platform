using Application.Services.Courses;
using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using Moq;
using System.Net;

namespace Application.Test;

[TestFixture]
public class CourseServiceTests
{
    private Mock<ICourseRepository> _courseRepositoryMock;
    private CourseService _courseService;

    [SetUp]
    public void Setup()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _courseService = new CourseService(_courseRepositoryMock.Object);
    }

    [Test]
    public async Task GetAllAsync_WithValidRequest_ReturnsPaginatedCourses()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var request = new CourseListRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var courses = new List<Course>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Course 1",
                Description = "Description 1",
                Status = CourseStatus.Published,
                Difficulty = CourseDifficulty.Beginner,
                DueDate = DateTime.UtcNow.AddDays(30),
                Category = new Category { Id = Guid.NewGuid(), Name = "Category 1" },
                CourseTags = new List<CourseTag>()
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Course 2",
                Description = "Description 2",
                Status = CourseStatus.Draft,
                Difficulty = CourseDifficulty.Intermediate,
                DueDate = DateTime.UtcNow.AddDays(60),
                Category = new Category { Id = Guid.NewGuid(), Name = "Category 2" },
                CourseTags = new List<CourseTag>()
            }
        }.AsQueryable();

        var paginatedResponse = new PaginatedList<Course>(
            courses.ToList(),
            courses.Count(),
            request.PageIndex,
            request.PageSize
        );

        _courseRepositoryMock.Setup(repo =>
                repo.GetPaginatedCoursesAsync(pageIndex, pageSize, request.CategoryId, request.MentorId))
            .ReturnsAsync(paginatedResponse);
        
        // Act
        var result = await _courseService.GetAllAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(2));
            Assert.That(result.Value.TotalCount, Is.EqualTo(2));
            Assert.That(result.Value.PageIndex, Is.EqualTo(pageIndex));
            Assert.That(result.Value.PageSize, Is.EqualTo(pageSize));

            var firstItem = result.Value.Items.First();
            Assert.That(firstItem.Title, Is.EqualTo("Course 1"));
            Assert.That(firstItem.CategoryName, Is.EqualTo("Category 1"));
        });
    }

    [Test]
    public async Task GetAllAsync_WithCategoryFilter_ReturnsPaginatedCoursesForCategory()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var categoryId = Guid.NewGuid();
        var request = new CourseListRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            CategoryId = categoryId
        };

        var allCourses = new List<Course>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Course 1",
                Description = "Description 1",
                CategoryId = categoryId,
                Status = CourseStatus.Published,
                Category = new Category { Id = categoryId, Name = "Category 1" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Course 2",
                Description = "Description 2",
                CategoryId = Guid.NewGuid(),
                Status = CourseStatus.Draft,
                Category = new Category { Id = Guid.NewGuid(), Name = "Category 2" }
            }
        };

        var filteredList = allCourses.Where(c => c.CategoryId == categoryId).ToList();

        var paginatedList = new PaginatedList<Course>(
            filteredList,
            filteredList.Count,
            pageIndex,
            pageSize
        );

        _courseRepositoryMock.Setup(repo =>
                repo.GetPaginatedCoursesAsync(pageIndex, pageSize, categoryId, request.MentorId))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _courseService.GetAllAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.All(c => c.CategoryId == categoryId), Is.True);
        });
    }

    [Test]
    public async Task GetAllAsync_WithInvalidPageParameters_ReturnsBadRequest()
    {
        // Arrange
        var request = new CourseListRequest
        {
            PageIndex = 0,
            PageSize = 0
        };

        // Act
        var result = await _courseService.GetAllAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Error, Is.EqualTo("Page index and page size must be greater than 0"));
            _courseRepositoryMock.Verify(repo => repo.GetAll(), Times.Never);
            _courseRepositoryMock.Verify(repo => repo.ToPaginatedListAsync(
                    It.IsAny<IQueryable<Course>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()),
                Times.Never);
        });
    }

    [Test]
    public async Task GetByIdAsync_CourseExists_ReturnsCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            Description = "Test Description",
            Status = CourseStatus.Published,
            Difficulty = CourseDifficulty.Beginner,
            DueDate = DateTime.UtcNow.AddDays(30)
        };

        _courseRepositoryMock.Setup(repo => repo.GetCourseWithDetailsAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.GetByIdAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Id, Is.EqualTo(courseId));
            Assert.That(result.Value.Title, Is.EqualTo(course.Title));
        });
    }

    [Test]
    public async Task GetByIdAsync_CourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        _courseRepositoryMock.Setup(repo => repo.GetCourseWithDetailsAsync(courseId))
            .ReturnsAsync(default(Course));

        // Act
        var result = await _courseService.GetByIdAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
            _courseRepositoryMock.Verify(repo => repo.GetCourseWithDetailsAsync(courseId), Times.Once);
        });
    }
}