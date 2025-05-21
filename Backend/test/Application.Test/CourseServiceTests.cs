using System.Linq.Expressions;
using System.Net;
using Application.Services.Courses;
using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.Test;

[TestFixture]
public class CourseServiceTests
{
    [SetUp]
    public void Setup()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _tagRepositoryMock = new Mock<ITagRepository>();
        _courseService = new CourseService(_courseRepositoryMock.Object, _tagRepositoryMock.Object);
    }

    private Mock<ICourseRepository> _courseRepositoryMock;
    private Mock<ITagRepository> _tagRepositoryMock;
    private CourseService _courseService;

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

        var category1 = new Category { Id = Guid.NewGuid(), Name = "Category 1" };
        var category2 = new Category { Id = Guid.NewGuid(), Name = "Category 2" };

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
                Category = category1,
                CategoryId = category1.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Course 2",
                Description = "Description 2",
                Status = CourseStatus.Draft,
                Difficulty = CourseDifficulty.Intermediate,
                DueDate = DateTime.UtcNow.AddDays(60),
                Category = category2,
                CategoryId = category2.Id
            }
        };

        var courseSummaries = courses.Select(c => new CourseSummary
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            CategoryId = c.CategoryId,
            CategoryName = c.Category?.Name,
            Status = c.Status,
            Difficulty = c.Difficulty,
            DueDate = c.DueDate,
            Items = c.Items.Select(i => new CourseItemDto
            {
                Title = i.Title,
                Description = i.Description,
                MediaType = i.MediaType,
                WebAddress = i.WebAddress
            }).ToList()
        }).ToList();

        var paginatedResponse = new PaginatedList<CourseSummary>(
            courseSummaries,
            courseSummaries.Count,
            request.PageIndex,
            request.PageSize
        );

        _courseRepositoryMock.Setup(repo =>
                repo.GetPaginatedCoursesAsync(pageIndex, pageSize, request.CategoryId, request.MentorId,
                    request.Keyword, request.Status, request.Difficulty))
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
        var mentorId = Guid.NewGuid();
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

        var courseSummaryFiltered =
            allCourses.Where(c => c.CategoryId == categoryId)
                .Select(c => new CourseSummary
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Status = c.Status,
                    Difficulty = c.Difficulty,
                    DueDate = c.DueDate,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category.Name,
                    Items = []
                }).ToList();

        var paginatedResponse = new PaginatedList<CourseSummary>(
            courseSummaryFiltered,
            courseSummaryFiltered.Count,
            request.PageIndex,
            request.PageSize
        );

        _courseRepositoryMock.Setup(repo =>
                repo.GetPaginatedCoursesAsync(request.PageIndex,
                    request.PageSize,
                    request.CategoryId,
                    request.MentorId,
                    request.Keyword,
                    request.Status,
                    request.Difficulty))
            .ReturnsAsync(paginatedResponse);


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
            DueDate = DateTime.UtcNow.AddDays(30),
            Category = new Category { Id = Guid.NewGuid(), Name = "Category 1" },
            Mentor = new User { Id = Guid.NewGuid(), FullName = "John" }
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

    [Test]
    public async Task GetAllAsync_WithKeywordFilter_ReturnsPaginatedCoursesMatchingKeyword()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var keyword = "Programming";
        var request = new CourseListRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Keyword = keyword
        };

        var allCourses = new List<Course>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Programming Basics",
                Description = "Learn programming fundamentals",
                Status = CourseStatus.Published,
                Category = new Category { Id = Guid.NewGuid(), Name = "Programming" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Advanced Math",
                Description = "Learn advanced mathematics",
                Status = CourseStatus.Published,
                Category = new Category { Id = Guid.NewGuid(), Name = "Mathematics" }
            }
        }.AsQueryable();

        var filteredList = allCourses
            .Where(c => c.Title.Contains(keyword) || c.Description.Contains(keyword))
            .Select(c => new CourseSummary
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Status = c.Status,
                Difficulty = c.Difficulty,
                DueDate = c.DueDate,
                Items = new List<CourseItemDto>()
            })
            .ToList();

        var paginatedResponse = new PaginatedList<CourseSummary>(
            filteredList,
            filteredList.Count,
            pageIndex,
            pageSize
        );

        _courseRepositoryMock.Setup(repo =>
                repo.GetPaginatedCoursesAsync(pageIndex, pageSize, request.CategoryId, request.MentorId,
                    request.Keyword, request.Status, request.Difficulty))
            .ReturnsAsync(paginatedResponse);

        // Act
        var result = await _courseService.GetAllAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.First().Title, Is.EqualTo("Programming Basics"));
            _courseRepositoryMock.Verify(repo =>
                repo.GetPaginatedCoursesAsync(pageIndex, pageSize, null, null, keyword, request.Status,
                    request.Difficulty), Times.Once);
        });
    }

    [Test]
    public async Task CreateAsync_ValidRequest_ReturnsCourseWithCreatedStatus()
    {
        // Arrange
        var request = new CourseCreateRequest
        {
            Title = "New Course",
            Description = "Course Description",
            CategoryId = Guid.NewGuid(),
            MentorId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(30),
            Difficulty = CourseDifficulty.Beginner
        };
        var createdCourse = new Course
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            MentorId = request.MentorId,
            DueDate = request.DueDate,
            Status = CourseStatus.Draft,
            Difficulty = request.Difficulty
        };

        Course savedCourse = null;
        _courseRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Course>()))
            .Callback<Course>(course => savedCourse = course)
            .Returns(Task.CompletedTask);

        _courseRepositoryMock.Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(1);

        _courseRepositoryMock.Setup(repo => repo.GetCourseWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() =>
            {
                savedCourse.Category = new Category { Id = request.CategoryId, Name = "Test Category" };
                savedCourse.Mentor = new User { Id = request.MentorId, FullName = "Test Mentor" };
                return savedCourse;
            });

        // Act
        var result = await _courseService.CreateAsync(request.MentorId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Title, Is.EqualTo(request.Title));
            Assert.That(result.Value.Description, Is.EqualTo(request.Description));
            Assert.That(result.Value.CategoryId, Is.EqualTo(request.CategoryId));
            Assert.That(result.Value.CategoryName, Is.EqualTo("Test Category"));
            Assert.That(result.Value.MentorId, Is.EqualTo(request.MentorId));
            Assert.That(result.Value.MentorName, Is.EqualTo("Test Mentor"));
            Assert.That(savedCourse, Is.Not.Null);
            Assert.That(savedCourse.Status, Is.EqualTo(CourseStatus.Draft));

            _courseRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Course>()), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.GetCourseWithDetailsAsync(savedCourse.Id), Times.Once);
        });
    }

    [Test]
    public async Task UpdateAsync_CourseExists_ReturnsCourseWithUpdatedStatus()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var existingCourse = new Course
        {
            Id = courseId,
            Title = "Old Title",
            Description = "Old Description",
            Status = CourseStatus.Draft,
            CategoryId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow,
            Difficulty = CourseDifficulty.Beginner,
            Mentor = new User { Id = Guid.NewGuid(), FullName = "Test Mentor" },
            Category = new Category { Id = Guid.NewGuid(), Name = "Test Category" }
        };

        var request = new CourseUpdateRequest
        {
            Title = "Updated Title",
            Description = "Updated Description",
            CategoryId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(30),
            Difficulty = CourseDifficulty.Intermediate
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, It.IsAny<Expression<Func<Course, object>>?>()))
            .ReturnsAsync(existingCourse);

        _courseRepositoryMock.Setup(repo => repo.GetCourseWithDetailsAsync(courseId))
            .ReturnsAsync(existingCourse);

        // Act
        var result = await _courseService.UpdateAsync(courseId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Title, Is.EqualTo(request.Title));
            Assert.That(result.Value.Description, Is.EqualTo(request.Description));
            Assert.That(result.Value.CategoryId, Is.EqualTo(request.CategoryId));
            Assert.That(result.Value.Difficulty, Is.EqualTo(request.Difficulty));
            _courseRepositoryMock.Verify(repo => repo.GetCourseWithDetailsAsync(courseId), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        });
    }

    [Test]
    public async Task UpdateAsync_CourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CourseUpdateRequest
        {
            Title = "Updated Title",
            Description = "Updated Description",
            CategoryId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(30),
            Difficulty = CourseDifficulty.Intermediate
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(default(Course));

        // Act
        var result = await _courseService.UpdateAsync(courseId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
            _courseRepositoryMock.Verify(repo => repo.GetCourseWithDetailsAsync(courseId), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task DeleteAsync_CourseExists_ReturnsSuccess()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var existingCourse = new Course
        {
            Id = courseId,
            Title = "Course to Delete",
            Description = "Description",
            Status = CourseStatus.Draft
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(existingCourse);

        // Act
        var result = await _courseService.DeleteAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            _courseRepositoryMock.Verify(repo => repo.GetByIdAsync(courseId, null), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.Delete(existingCourse), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        });
    }

    [Test]
    public async Task DeleteAsync_CourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(default(Course));

        // Act
        var result = await _courseService.DeleteAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
            _courseRepositoryMock.Verify(repo => repo.GetByIdAsync(courseId, null), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Course>()), Times.Never);
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task GetAllAsync_WithMentorFilter_ReturnsPaginatedCoursesForMentor()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var mentorId = Guid.NewGuid();
        var request = new CourseListRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            MentorId = mentorId
        };

        var coursesForMentor = new List<Course>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Mentor Course",
                Description = "Description",
                MentorId = mentorId,
                Status = CourseStatus.Published,
                Category = new Category { Id = Guid.NewGuid(), Name = "Category" }
            }
        };

        var paginatedResponse = new PaginatedList<CourseSummary>(
            coursesForMentor.Select(c => new CourseSummary
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Status = c.Status,
                Difficulty = c.Difficulty,
                DueDate = c.DueDate,
                Items = new List<CourseItemDto>()
            }).ToList(),
            coursesForMentor.Count,
            pageIndex,
            pageSize
        );

        _courseRepositoryMock.Setup(repo =>
                repo.GetPaginatedCoursesAsync(pageIndex, pageSize, null, mentorId,
                    null, null, null))
            .ReturnsAsync(paginatedResponse);

        // Act
        var result = await _courseService.GetAllAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.First().Title, Is.EqualTo("Mentor Course"));
        });
    }

    [Test]
    public async Task GetAllAsync_WithStatusFilter_ReturnsPaginatedCoursesWithMatchingStatus()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var status = CourseStatus.Published;
        var request = new CourseListRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Status = status
        };

        var publishedCourses = new List<Course>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Published Course",
                Description = "Description",
                Status = CourseStatus.Published,
                Category = new Category { Id = Guid.NewGuid(), Name = "Category" }
            }
        };

        var paginatedResponse = new PaginatedList<CourseSummary>(
            publishedCourses.Select(c => new CourseSummary
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Status = c.Status,
                Difficulty = c.Difficulty,
                DueDate = c.DueDate,
                Items = new List<CourseItemDto>()
            }).ToList(),
            publishedCourses.Count,
            pageIndex,
            pageSize
        );

        _courseRepositoryMock.Setup(repo =>
                repo.GetPaginatedCoursesAsync(pageIndex, pageSize, null, null,
                    null, status, null))
            .ReturnsAsync(paginatedResponse);

        // Act
        var result = await _courseService.GetAllAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.First().Title, Is.EqualTo("Published Course"));
        });
    }

    [Test]
    public async Task GetAllAsync_WithMultipleFilters_ReturnsPaginatedCoursesMatchingAllCriteria()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var categoryId = Guid.NewGuid();
        var mentorId = Guid.NewGuid();
        var status = CourseStatus.Published;
        var difficulty = CourseDifficulty.Intermediate;

        var request = new CourseListRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            CategoryId = categoryId,
            MentorId = mentorId,
            Status = status,
            Difficulty = difficulty,
            Keyword = "Advanced"
        };

        var matchingCourses = new List<Course>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Advanced Course",
                Description = "Advanced level course",
                CategoryId = categoryId,
                MentorId = mentorId,
                Status = status,
                Difficulty = difficulty,
                Category = new Category { Id = categoryId, Name = "Category" }
            }
        };

        var paginatedResponse = new PaginatedList<CourseSummary>(
            matchingCourses.Select(c => new CourseSummary
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Status = c.Status,
                Difficulty = c.Difficulty,
                DueDate = c.DueDate,
                Items = new List<CourseItemDto>()
            }).ToList(),
            matchingCourses.Count,
            pageIndex,
            pageSize
        );

        _courseRepositoryMock.Setup(repo =>
                repo.GetPaginatedCoursesAsync(pageIndex, pageSize, categoryId, mentorId,
                    "Advanced", status, difficulty))
            .ReturnsAsync(paginatedResponse);

        // Act
        var result = await _courseService.GetAllAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.First().Title, Is.EqualTo("Advanced Course"));
            _courseRepositoryMock.Verify(repo =>
                repo.GetPaginatedCoursesAsync(pageIndex, pageSize, categoryId, mentorId,
                    "Advanced", status, difficulty), Times.Once);
        });
    }

    [Test]
    public async Task PublishCourseAsync_WhenCourseExists_ReturnsPublishedCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            Status = CourseStatus.Draft,
            Mentor = new User(),
            Category = new Category { Id = Guid.NewGuid(), Name = "Category" }
        };

        _courseRepositoryMock.Setup(repo => repo.GetCourseWithDetailsAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.PublishCourseAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value.Status, Is.EqualTo(CourseStatus.Published));
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        });
    }

    [Test]
    public async Task PublishCourseAsync_WhenCourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _courseRepositoryMock.Setup(repo => repo.GetCourseWithDetailsAsync(courseId))
            .ReturnsAsync((Course)null);

        // Act
        var result = await _courseService.PublishCourseAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task PublishCourseAsync_WhenCourseAlreadyPublished_ReturnsBadRequest()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            Status = CourseStatus.Published
        };

        _courseRepositoryMock.Setup(repo => repo.GetCourseWithDetailsAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.PublishCourseAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Error, Is.EqualTo("Course is already published"));
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task ArchiveCourseAsync_WhenCourseExists_ReturnsArchivedCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            Status = CourseStatus.Published,
            Mentor = new User(),
            Category = new Category { Id = Guid.NewGuid(), Name = "Category" }
        };

        _courseRepositoryMock.Setup(repo => repo.GetCourseWithDetailsAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.ArchiveCourseAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value.Status, Is.EqualTo(CourseStatus.Archived));
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        });
    }

    [Test]
    public async Task ArchiveCourseAsync_WhenCourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _courseRepositoryMock.Setup(repo => repo.GetCourseWithDetailsAsync(courseId))
            .ReturnsAsync((Course)null);

        // Act
        var result = await _courseService.ArchiveCourseAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task ArchiveCourseAsync_WhenCourseAlreadyArchived_ReturnsBadRequest()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            Status = CourseStatus.Archived
        };

        _courseRepositoryMock.Setup(repo => repo.GetCourseWithDetailsAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.ArchiveCourseAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Error, Is.EqualTo("Course is already archived"));
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }
}