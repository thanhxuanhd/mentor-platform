using Application.Services.Courses;
using Contract.Dtos.CourseResources.Responses;
using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Net;

namespace Application.Test;

[TestFixture]
public class CourseServiceTests
{
    [SetUp]
    public void Setup()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _tagRepositoryMock = new Mock<ITagRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
        _courseService = new CourseService(
            _courseRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _webHostEnvironmentMock.Object
        );
    }

    private Mock<ICourseRepository> _courseRepositoryMock;
    private Mock<ITagRepository> _tagRepositoryMock;
    private Mock<ICategoryRepository> _categoryRepositoryMock;
    private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
    private CourseService _courseService;

    [Test]
    public async Task GetAllAsync_WithValidRequest_ReturnsPaginatedCourses()
    {
        // Arrange
        var request = new CourseListRequest
        {
            PageIndex = 1,
            PageSize = 10
        };

        var category1 = new Category { Id = Guid.NewGuid(), Name = "Category 1" };
        var category2 = new Category { Id = Guid.NewGuid(), Name = "Category 2" };
        var mentor1 = new User { Id = Guid.NewGuid(), FullName = "Mentor 1" };
        var mentor2 = new User { Id = Guid.NewGuid(), FullName = "Mentor 2" };
        List<Course> courses =
        [
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Course 1",
                Description = "Description 1",
                Status = CourseStatus.Published,
                Difficulty = CourseDifficulty.Beginner,
                DueDate = DateTime.UtcNow.AddDays(30),
                Category = category1,
                CategoryId = category1.Id,
                Mentor = mentor1,
                MentorId = mentor1.Id
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
                CategoryId = category2.Id,
                Mentor = mentor2,
                MentorId = mentor2.Id
            }
        ];

        _courseRepositoryMock.Setup(repo => repo.GetAll()).Returns(courses.AsQueryable());

        var paginatedCourses = new PaginatedList<CourseSummaryResponse>(
            courses.Select(c => c.ToCourseSummaryResponse()).ToList(),
            courses.Count,
            request.PageIndex,
            request.PageSize);

        _courseRepositoryMock.Setup(repo =>
                repo.ToPaginatedListAsync(
                    It.Is<IQueryable<CourseSummaryResponse>>(q => q.Count() == courses.Count),
                    request.PageSize, request.PageIndex))
            .ReturnsAsync(paginatedCourses);

        // Act
        var result = await _courseService.GetAllAsync(It.IsAny<Guid>(), UserRole.Admin, request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(2));
            Assert.That(result.Value.TotalCount, Is.EqualTo(2));
            Assert.That(result.Value.PageIndex, Is.EqualTo(request.PageIndex));
            Assert.That(result.Value.PageSize, Is.EqualTo(request.PageSize));

            var firstResource = result.Value.Items.First();
            Assert.That(firstResource.Title, Is.EqualTo("Course 1"));
            Assert.That(firstResource.CategoryName, Is.EqualTo("Category 1"));
        }
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

        var courses = new List<Course>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Course 1",
                Description = "Description 1",
                CategoryId = categoryId,
                Category = new Category { Id = categoryId, Name = "Category 1" },
                Mentor = new User { Id = Guid.NewGuid(), FullName = "Mentor 1" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Course 2",
                Description = "Description 2",
                CategoryId = Guid.NewGuid(),
                Category = new Category { Id = Guid.NewGuid(), Name = "Category 2" },
                Mentor = new User { Id = Guid.NewGuid(), FullName = "Mentor 2" }
            }
        }.AsQueryable();

        var courseSummaryResponseFiltered =
            courses.Where(c => c.CategoryId == categoryId)
                .Select(c => c.ToCourseSummaryResponse()).ToList();

        var paginatedResponse = new PaginatedList<CourseSummaryResponse>(
            courseSummaryResponseFiltered,
            courseSummaryResponseFiltered.Count,
            request.PageIndex,
            request.PageSize
        );

        _courseRepositoryMock.Setup(repo => repo.GetAll()).Returns(courses);
        _courseRepositoryMock.Setup(repo =>
                repo.ToPaginatedListAsync(
                    It.IsAny<IQueryable<CourseSummaryResponse>>(),
                    request.PageSize,
                    request.PageIndex))
            .ReturnsAsync(paginatedResponse);

        // Act
        var result = await _courseService.GetAllAsync(mentorId, UserRole.Mentor, request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.All(c => c.CategoryId == categoryId), Is.True);
        }
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

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.GetByIdAsync(courseId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Id, Is.EqualTo(courseId));
            Assert.That(result.Value.Title, Is.EqualTo(course.Title));
        }
    }

    [Test]
    public async Task GetByIdAsync_CourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(default(Course));

        // Act
        var result = await _courseService.GetByIdAsync(courseId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
            _courseRepositoryMock.Verify(repo => repo.GetByIdAsync(courseId, null), Times.Once);
        }
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
            .Select(c => new CourseSummaryResponse
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Status = c.Status,
                Difficulty = c.Difficulty,
                DueDate = c.DueDate,
                Resources = new List<CourseResourceResponse>()
            })
            .ToList();

        var paginatedResponse = new PaginatedList<CourseSummaryResponse>(
            filteredList,
            filteredList.Count,
            pageIndex,
            pageSize
        );

        _courseRepositoryMock.Setup(repo =>
                repo.GetAll())
            .Returns(allCourses);

        _courseRepositoryMock.Setup(repo => repo.ToPaginatedListAsync(
                It.IsAny<IQueryable<CourseSummaryResponse>>(),
                request.PageSize,
                request.PageIndex))
            .ReturnsAsync(paginatedResponse);

        // Act
        var result = await _courseService.GetAllAsync(It.IsAny<Guid>(), UserRole.Admin, request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.First().Title, Is.EqualTo("Programming Basics"));
            _courseRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        }
    }

    [Test]
    public async Task CreateAsync_ValidRequest_ReturnsCourseWithCreatedStatus()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var request = new CourseCreateRequest
        {
            Title = "New Course",
            Description = "Course Description",
            CategoryId = categoryId,
            DueDate = DateTime.UtcNow.AddDays(30),
            Difficulty = CourseDifficulty.Beginner,
            Tags = ["tag1", "tag2"]
        };
        var category = new Category
        {
            Id = categoryId,
            Name = "Test Category",
            Status = true
        };
        var mentor = new User
        {
            Id = mentorId,
            FullName = "Test Mentor"
        };

        _courseRepositoryMock.Setup(repo => repo.GetByTitleAsync(request.Title))
            .ReturnsAsync(default(Course));

        _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId, null)).ReturnsAsync(category);

        var tags = request.Tags.Select(t => new Tag { Name = t }).ToList();
        _tagRepositoryMock.Setup(repo => repo.UpsertAsync(It.IsAny<HashSet<string>>()))
            .ReturnsAsync(tags);
        Course course = null!;

        _courseRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Course>()))
            .Callback<Course>(c => course = c)
            .Returns(Task.CompletedTask);

        _courseRepositoryMock.Setup(repo => repo.LoadReferencedEntities(It.IsAny<Course>()))
            .Callback<Course>(c =>
            {
                c.Category = category;
                c.Mentor = mentor;
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _courseService.CreateAsync(mentorId, request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Title, Is.EqualTo(request.Title));
            Assert.That(result.Value.Description, Is.EqualTo(request.Description));
            Assert.That(result.Value.CategoryId, Is.EqualTo(request.CategoryId));
            Assert.That(result.Value.CategoryName, Is.EqualTo("Test Category"));
            Assert.That(result.Value.MentorId, Is.EqualTo(mentorId));
            Assert.That(result.Value.MentorName, Is.EqualTo("Test Mentor"));
            Assert.That(result.Value.Status, Is.EqualTo(CourseStatus.Draft));
            Assert.That(result.Value.Difficulty, Is.EqualTo(request.Difficulty));


            // Verify repository interactions
            _courseRepositoryMock.Verify(repo => repo.GetByTitleAsync(request.Title), Times.Once);
            _tagRepositoryMock.Verify(repo => repo.UpsertAsync(It.IsAny<HashSet<string>>()), Times.Once);
            _tagRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Course>()), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.UpdateTagsCollection(tags, It.IsAny<Course>()), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.LoadReferencedEntities(It.IsAny<Course>()), Times.Once);
        }
    }

    [Test]
    public async Task CreateAsync_InactiveCategory_ReturnsBadRequest()
    {
        // Arrange
        var mentorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var request = new CourseCreateRequest
        {
            Title = "New Course",
            Description = "Course Description",
            CategoryId = categoryId,
            DueDate = DateTime.UtcNow.AddDays(30),
            Difficulty = CourseDifficulty.Beginner,
            Tags = ["tag1", "tag2"]
        };

        var inactiveCategory = new Category
        {
            Id = categoryId,
            Name = "Inactive Category",
            Status = false
        };

        _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId, null))
            .ReturnsAsync(inactiveCategory);

        // Act
        var result = await _courseService.CreateAsync(mentorId, request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Error, Does.Contain("Category is not active"));
        }
    }


    [Test]
    public async Task UpdateAsync_CourseExists_ReturnsCourseWithUpdatedStatus()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
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
            CategoryId = categoryId,
            DueDate = DateTime.UtcNow.AddDays(30),
            Difficulty = CourseDifficulty.Intermediate,
            Tags = ["tag1", "tag2"]
        };

        var category = new Category
        {
            Id = categoryId,
            Name = "New Category",
            Status = true
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(existingCourse);

        _courseRepositoryMock.Setup(repo => repo.GetByTitleAsync(request.Title))
            .ReturnsAsync(default(Course));

        _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId, null))
            .ReturnsAsync(category);

        var tags = request.Tags.Select(t => new Tag { Name = t }).ToList();
        _tagRepositoryMock.Setup(repo => repo.UpsertAsync(It.IsAny<HashSet<string>>()))
            .ReturnsAsync(tags);

        _courseRepositoryMock.Setup(repo => repo.LoadReferencedEntities(It.IsAny<Course>()))
            .Callback<Course>(c =>
            {
                c.Category = category;
                c.Mentor = existingCourse.Mentor;
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _courseService.UpdateAsync(courseId, request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Title, Is.EqualTo(request.Title));
            Assert.That(result.Value.Description, Is.EqualTo(request.Description));
            Assert.That(result.Value.CategoryId, Is.EqualTo(request.CategoryId));
            Assert.That(result.Value.CategoryName, Is.EqualTo(category.Name));
            Assert.That(result.Value.Difficulty, Is.EqualTo(request.Difficulty));
            Assert.That(result.Value.DueDate, Is.EqualTo(request.DueDate));
            Assert.That(result.Value.Status, Is.EqualTo(CourseStatus.Draft));

            // Verify repository interactions
            _courseRepositoryMock.Verify(repo => repo.GetByIdAsync(courseId, null), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.GetByTitleAsync(request.Title), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, null), Times.Once);
            _tagRepositoryMock.Verify(repo => repo.UpsertAsync(It.IsAny<HashSet<string>>()), Times.Once);
            _tagRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.UpdateTagsCollection(tags, existingCourse), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.LoadReferencedEntities(existingCourse), Times.Once);
        }
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
            _courseRepositoryMock.Verify(repo => repo.GetByIdAsync(courseId, null), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
            _courseRepositoryMock.Verify(repo => repo.GetByIdAsync(courseId, null), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Course>()), Times.Never);
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
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

        _courseRepositoryMock.Setup(repo => repo.GetAll())
            .Returns(coursesForMentor.AsQueryable());

        var paginatedResponse = new PaginatedList<CourseSummaryResponse>(
            coursesForMentor.Select(c => new CourseSummaryResponse
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Status = c.Status,
                Difficulty = c.Difficulty,
                DueDate = c.DueDate,
                Resources = new List<CourseResourceResponse>()
            }).ToList(),
            coursesForMentor.Count,
            pageIndex,
            pageSize
        );

        _courseRepositoryMock.Setup(repo => repo.ToPaginatedListAsync(
                It.IsAny<IQueryable<CourseSummaryResponse>>(),
                request.PageSize,
                request.PageIndex))
            .ReturnsAsync(paginatedResponse);

        // Act
        var result = await _courseService.GetAllAsync(mentorId, UserRole.Mentor, request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.First().Title, Is.EqualTo("Mentor Course"));
        }
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

        _courseRepositoryMock.Setup(repo => repo.GetAll())
            .Returns(publishedCourses.AsQueryable());

        var paginatedResponse = new PaginatedList<CourseSummaryResponse>(
            publishedCourses.Select(c => new CourseSummaryResponse
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Status = c.Status,
                Difficulty = c.Difficulty,
                DueDate = c.DueDate,
                Resources = new List<CourseResourceResponse>()
            }).ToList(),
            publishedCourses.Count,
            pageIndex,
            pageSize
        );

        _courseRepositoryMock.Setup(repo => repo.ToPaginatedListAsync(
                It.IsAny<IQueryable<CourseSummaryResponse>>(),
                request.PageSize,
                request.PageIndex))
            .ReturnsAsync(paginatedResponse);

        // Act
        var result = await _courseService.GetAllAsync(It.IsAny<Guid>(), UserRole.Admin, request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.First().Title, Is.EqualTo("Published Course"));
        }
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

        _courseRepositoryMock.Setup(repo => repo.GetAll())
            .Returns(matchingCourses.AsQueryable());

        var paginatedResponse = new PaginatedList<CourseSummaryResponse>(
            matchingCourses.Select(c => new CourseSummaryResponse
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Status = c.Status,
                Difficulty = c.Difficulty,
                DueDate = c.DueDate,
                Resources = new List<CourseResourceResponse>()
            }).ToList(),
            matchingCourses.Count,
            pageIndex,
            pageSize
        );

        _courseRepositoryMock.Setup(repo => repo.ToPaginatedListAsync(
                It.IsAny<IQueryable<CourseSummaryResponse>>(),
                request.PageSize,
                request.PageIndex))
            .ReturnsAsync(paginatedResponse);

        // Act
        var result = await _courseService.GetAllAsync(mentorId, UserRole.Mentor, request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.First().Title, Is.EqualTo("Advanced Course"));
            _courseRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        }
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

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.PublishCourseAsync(courseId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value.Status, Is.EqualTo(CourseStatus.Published));
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }

    [Test]
    public async Task PublishCourseAsync_WhenCourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync((Course)null);

        // Act
        var result = await _courseService.PublishCourseAsync(courseId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
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

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.PublishCourseAsync(courseId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Error, Is.EqualTo("Course is already published"));
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
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

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.ArchiveCourseAsync(courseId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value.Status, Is.EqualTo(CourseStatus.Archived));
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }

    [Test]
    public async Task ArchiveCourseAsync_WhenCourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync((Course)null);

        // Act
        var result = await _courseService.ArchiveCourseAsync(courseId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
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

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.ArchiveCourseAsync(courseId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Error, Is.EqualTo("Course is already archived"));
            _courseRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
    }
}