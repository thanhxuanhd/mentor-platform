using System.Linq.Expressions;
using System.Net;
using Application.Services.Categories;
using Contract.Dtos.Categories.Requests;
using Contract.Dtos.Categories.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using Moq;
using System.Net;

namespace Application.Test;

[TestFixture]
public class CategoryServiceTest
{
    [SetUp]
    public void Setup()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _categoryService = new CategoryService(_mockCategoryRepository.Object);
    }

    private Mock<ICategoryRepository> _mockCategoryRepository;
    private CategoryService _categoryService;
    private const int pageIndex = 1;
    private const int pageSize = 10;

    [Test]
    public async Task GetCategoriesAsync_WithoutKeyword_ReturnsPaginatedCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new()
            {
                Id = Guid.NewGuid(), Name = "Category 1", Description = "Desc 1", Status = true,
                Courses = new List<Course>()
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Category 2", Description = "Desc 2", Status = false,
                Courses = new List<Course>()
            }
        }.AsQueryable();

        var paginatedList = new PaginatedList<GetCategoryResponse>(
            categories.Select(c => new GetCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description!,
                Courses = c.Courses!.Count,
                Status = c.Status
            }).ToList(),
            categories.Count(),
            pageIndex,
            pageSize
        );

        _mockCategoryRepository.Setup(repo => repo.GetAll()).Returns(categories);
        _mockCategoryRepository.Setup(repo =>
                repo.ToPaginatedListAsync(It.IsAny<IQueryable<GetCategoryResponse>>(), pageSize, pageIndex))
            .ReturnsAsync(paginatedList); // Act
        var request = new FilterCategoryRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Keyword = string.Empty
        };
        var result = await _categoryService.GetCategoriesAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(2));
            _mockCategoryRepository.Verify(repo => repo.GetAll(), Times.Once);
            _mockCategoryRepository.Verify(
                repo => repo.ToPaginatedListAsync(It.IsAny<IQueryable<GetCategoryResponse>>(), pageSize, pageIndex),
                Times.Once);
        });
    }

    [Test]
    public async Task GetCategoriesAsync_WithKeyword_ReturnsFilteredPaginatedCategories()
    {
        // Arrange
        const string keyword = "Category 1";
        var categories = new List<Category>
        {
            new()
            {
                Id = Guid.NewGuid(), Name = "Category 1", Description = "Desc 1", Status = true,
                Courses = new List<Course>()
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Another Category", Description = "Desc 2", Status = false,
                Courses = new List<Course>()
            }
        }.AsQueryable();

        var filteredCategoriesQuery = categories.Where(c => c.Name.Contains(keyword));

        var paginatedList = new PaginatedList<GetCategoryResponse>(
            filteredCategoriesQuery.Select(c => new GetCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Courses = c.Courses.Count(),
                Status = c.Status
                Id = c.Id,
                Name = c.Name,
                Description = c.Description!,
                Courses = c.Courses!.Count,
                Status = c.Status
            }).ToList(),
            filteredCategoriesQuery.Count(),
            pageIndex,
            pageSize
        );

        _mockCategoryRepository.Setup(repo => repo.GetAll()).Returns(categories);
        _mockCategoryRepository.Setup(repo => repo.ToPaginatedListAsync(
                    It.Is<IQueryable<GetCategoryResponse>>(q => q.Count() == filteredCategoriesQuery.Count()), pageSize,
                    pageIndex))
            .ReturnsAsync(paginatedList);

        // Act
        var request = new FilterCategoryRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Keyword = keyword
        };
        var result = await _categoryService.GetCategoriesAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.First().Name, Is.EqualTo("Category 1"));
            _mockCategoryRepository.Verify(repo => repo.GetAll(), Times.Once);
            _mockCategoryRepository.Verify(
                repo => repo.ToPaginatedListAsync(
                    It.Is<IQueryable<GetCategoryResponse>>(q => q.Count() == filteredCategoriesQuery.Count()), pageSize,
                    pageIndex), Times.Once);
        });
    }
    [Test]
    public async Task GetCategoriesAsync_WithStatusFilter_ReturnsFilteredPaginatedCategories()
    {
        // Arrange
        const bool statusFilter = true;
        var categories = new List<Category>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Category 1",
                Description = "Desc 1",
                Status = true,
                Courses = new List<Course>()
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Category 2",
                Description = "Desc 2",
                Status = false,
                Courses = new List<Course>()
            }
        }.AsQueryable();

        var filteredCategoriesQuery = categories.Where(c => c.Status == statusFilter);

        var paginatedList = new PaginatedList<GetCategoryResponse>(
            filteredCategoriesQuery.Select(c => new GetCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description!,
                Courses = c.Courses!.Count,
                Status = c.Status
            }).ToList(),
            filteredCategoriesQuery.Count(),
            pageIndex,
            pageSize
        );

        _mockCategoryRepository.Setup(repo => repo.GetAll()).Returns(categories);
        _mockCategoryRepository.Setup(repo =>
                repo.ToPaginatedListAsync(
                    It.Is<IQueryable<GetCategoryResponse>>(q => q.Count() == filteredCategoriesQuery.Count()),
                    pageSize,
                    pageIndex))
            .ReturnsAsync(paginatedList);

        // Act
        var request = new FilterCategoryRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Keyword = string.Empty,
            Status = statusFilter
        };
        var result = await _categoryService.GetCategoriesAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.First().Name, Is.EqualTo("Category 1"));
            Assert.That(result.Value.Items.First().Status, Is.True);
            _mockCategoryRepository.Verify(repo => repo.GetAll(), Times.Once);
            _mockCategoryRepository.Verify(
                repo => repo.ToPaginatedListAsync(
                    It.Is<IQueryable<GetCategoryResponse>>(q => q.Count() == filteredCategoriesQuery.Count()),
                    pageSize,
                    pageIndex),
                Times.Once);
        });
    }

    [Test]
    public async Task FilterCourseByCategoryAsync_CategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId, null)).ReturnsAsync(default(Category));

        // Act
        var result = await _categoryService.FilterCourseByCategoryAsync(categoryId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Category not found"));
            _mockCategoryRepository.Verify(repo => repo.GetByIdAsync(categoryId, null), Times.Once);
        });
    }

    [Test]
    public async Task FilterCourseByCategoryAsync_CategoryFound_ReturnsCourses()
    {        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Test Category", Courses = new List<Course>() };
        var coursesForCategory = new List<Course>
        {
            new()
            {
                Id = Guid.NewGuid(), Title = "Course 1", Description = "Desc 1", Status = CourseStatus.Published,
                Difficulty = CourseDifficulty.Beginner, DueDate = DateTime.UtcNow.AddDays(10),
                CourseTags = new List<CourseTag>(), CategoryId = categoryId, Category = category
            },
            new()
            {
                Id = Guid.NewGuid(), Title = "Course 2", Description = "Desc 2", Status = CourseStatus.Draft,
                Difficulty = CourseDifficulty.Intermediate, DueDate = DateTime.UtcNow.AddDays(20),
                CourseTags = new List<CourseTag>(), CategoryId = categoryId, Category = category
            }
        }.AsQueryable();

        var list = coursesForCategory
            .Select(c => new FilterCourseByCategoryResponse
            {
                Id = c.Id,
                Title = c.Title,
                CategoryName = category.Name,
                Status = c.Status.ToString(),
                Description = c.Description,
                Difficulty = c.Difficulty.ToString(),
                DueDate = c.DueDate,
                Tags = c.CourseTags.Select(ct => ct.Tag.Name).ToList()
            }).ToList();

        _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId, null)).ReturnsAsync(category);
        _mockCategoryRepository.Setup(repo => repo.FilterCourseByCategory(categoryId)).Returns(coursesForCategory);
        _mockCategoryRepository.Setup(repo => repo.ToListAsync(It.Is<IQueryable<FilterCourseByCategoryResponse>>(q => q.Count() == coursesForCategory.Count())))
            .ReturnsAsync(list);

        // Act
        var result = await _categoryService.FilterCourseByCategoryAsync(categoryId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!, Has.Count.EqualTo(2));
            _mockCategoryRepository.Verify(repo => repo.GetByIdAsync(categoryId, null), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.FilterCourseByCategory(categoryId), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.ToListAsync(It.Is<IQueryable<FilterCourseByCategoryResponse>>(q => q.Count() == coursesForCategory.Count())), Times.Once);
        });
    }

    [Test]
    public async Task GetCategoryByIdAsync_CategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<Expression<Func<Category, object>>>()))
            .ReturnsAsync(default(Category));

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Category not found"));
            _mockCategoryRepository.Verify(repo => repo.GetByIdAsync(categoryId, It.IsAny<Expression<Func<Category, object>>>()), Times.Once);
        });
    }

    [Test]
    public async Task GetCategoryByIdAsync_CategoryFound_ReturnsCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Test Category",
            Description = "Test Description",
            Status = true,
            Courses = new List<Course>()
        };

        _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<Expression<Func<Category, object>>>()))
            .ReturnsAsync(category);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Id, Is.EqualTo(categoryId));
            Assert.That(result.Value.Name, Is.EqualTo("Test Category"));
            Assert.That(result.Value.Description, Is.EqualTo("Test Description"));
            Assert.That(result.Value.Courses, Is.EqualTo(0));
            Assert.That(result.Value.Status, Is.True);
            _mockCategoryRepository.Verify(repo => repo.GetByIdAsync(categoryId, It.IsAny<Expression<Func<Category, object>>>()), Times.Once);
        });
    }

    [Test]
    public async Task CreateCategoryAsync_CategoryExists_ReturnsBadRequest()
    {
        // Arrange
        var request = new CategoryRequest
        {
            Name = "Test Category",
            Description = "Test Description",
            Status = true
        };
        _mockCategoryRepository.Setup(repo => repo.ExistByNameAsync(request.Name)).ReturnsAsync(true);

        // Act
        var result = await _categoryService.CreateCategoryAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Error, Is.EqualTo("Already have this category"));
            _mockCategoryRepository.Verify(repo => repo.ExistByNameAsync(request.Name), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.AddAsync(It.IsAny<Category>()), Times.Never);
            _mockCategoryRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task CreateCategoryAsync_CategoryDoesNotExist_CreatesAndReturnsCategory()
    {
        // Arrange
        var request = new CategoryRequest
        {
            Name = "Test Category",
            Description = "Test Description",
            Status = true
        };
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Status = request.Status
        };

        _mockCategoryRepository.Setup(repo => repo.ExistByNameAsync(request.Name)).ReturnsAsync(false);
        _mockCategoryRepository.Setup(repo => repo.AddAsync(It.IsAny<Category>())).Callback<Category>(c =>
        {
            c.Id = category.Id;
        });
        _mockCategoryRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _categoryService.CreateCategoryAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Name, Is.EqualTo(request.Name));
            Assert.That(result.Value.Description, Is.EqualTo(request.Description));
            Assert.That(result.Value.Status, Is.EqualTo(request.Status));
            Assert.That(result.Value.Courses, Is.EqualTo(0));
            _mockCategoryRepository.Verify(repo => repo.ExistByNameAsync(request.Name), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.AddAsync(It.IsAny<Category>()), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        });
    }

    [Test]
    public async Task EditCategoryAsync_CategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new CategoryRequest
        {
            Name = "Updated Category",
            Description = "Updated Description",
            Status = false
        };
        _mockCategoryRepository.Setup(repo => repo.ExistByNameExcludeAsync(categoryId, request.Name)).ReturnsAsync(false);
        _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId, null)).ReturnsAsync(default(Category));

        // Act
        var result = await _categoryService.EditCategoryAsync(categoryId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Categories is not found or is deleted"));
            _mockCategoryRepository.Verify(repo => repo.GetByIdAsync(categoryId, null), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.ExistByNameExcludeAsync(categoryId, request.Name), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.Update(It.IsAny<Category>()), Times.Never);
            _mockCategoryRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task EditCategoryAsync_CategoryNameExists_ReturnsBadRequest()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new CategoryRequest
        {
            Name = "Existing Category",
            Description = "Updated Description",
            Status = false
        };
        var category = new Category
        {
            Id = categoryId,
            Name = "Original Category",
            Description = "Original Description",
            Status = true
        };

        _mockCategoryRepository.Setup(repo => repo.ExistByNameExcludeAsync(categoryId, request.Name)).ReturnsAsync(true);

        // Act
        var result = await _categoryService.EditCategoryAsync(categoryId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Error, Is.EqualTo("Already have this category"));
            _mockCategoryRepository.Verify(repo => repo.ExistByNameExcludeAsync(categoryId, request.Name), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.Update(It.IsAny<Category>()), Times.Never);
            _mockCategoryRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task EditCategoryAsync_ValidRequest_UpdatesAndReturnsSuccess()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new CategoryRequest
        {
            Name = "Updated Category",
            Description = "Updated Description",
            Status = false
        };
        var category = new Category
        {
            Id = categoryId,
            Name = "Original Category",
            Description = "Original Description",
            Status = true
        };

        _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId, null)).ReturnsAsync(category);
        _mockCategoryRepository.Setup(repo => repo.ExistByNameExcludeAsync(categoryId, request.Name)).ReturnsAsync(false);
        _mockCategoryRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _categoryService.EditCategoryAsync(categoryId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.True);
            Assert.That(category.Name, Is.EqualTo(request.Name));
            Assert.That(category.Description, Is.EqualTo(request.Description));
            Assert.That(category.Status, Is.EqualTo(request.Status));
            _mockCategoryRepository.Verify(repo => repo.GetByIdAsync(categoryId, null), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.ExistByNameExcludeAsync(categoryId, request.Name), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.Update(category), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        });
    }

    [Test]
    public async Task DeleteCategoryAsync_WhenCategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId, c => c.Courses!))
            .ReturnsAsync((Category)null!);

        // Act
        var result = await _categoryService.DeleteCategoryAsync(categoryId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Categories is not found or is deleted"));

            _mockCategoryRepository.Verify(r => r.GetByIdAsync(categoryId, c => c.Courses!), Times.Once);
            _mockCategoryRepository.Verify(r => r.Delete(It.IsAny<Category>()), Times.Never);
            _mockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task DeleteCategoryAsync_WhenSuccessful_SetsIsDeletedAndReturnsSuccess()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Test Category",
        };

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId, c => c.Courses!))
            .ReturnsAsync(category);

        _mockCategoryRepository
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _categoryService.DeleteCategoryAsync(categoryId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.True);

            _mockCategoryRepository.Verify(r => r.GetByIdAsync(categoryId, c => c.Courses!), Times.Once);
            _mockCategoryRepository.Verify(r => r.Delete(category), Times.Once);
            _mockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        });
    }

}