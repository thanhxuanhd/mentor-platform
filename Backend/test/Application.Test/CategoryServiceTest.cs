using Application.Services.Categories;
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
    private Mock<ICategoryRepository> _categoryRepositoryMock;
    private CategoryService _categoryService;

    [SetUp]
    public void Setup()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _categoryService = new CategoryService(_categoryRepositoryMock.Object);
    }

    [Test]
    public async Task GetCategoriesAsync_WithoutKeyword_ReturnsPaginatedCategories()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Category 1", Description = "Desc 1", Status = true, Courses = new List<Course>() },
            new Category { Id = Guid.NewGuid(), Name = "Category 2", Description = "Desc 2", Status = false, Courses = new List<Course>() }
        }.AsQueryable();

        var paginatedList = new PaginatedList<GetCategoryResponse>(
            categories.Select(c => new GetCategoryResponse { Id = c.Id, Name = c.Name, Description = c.Description, Courses = c.Courses.Count(), Status = c.Status }).ToList(),
            categories.Count(),
            pageIndex,
            pageSize
        );

        _categoryRepositoryMock.Setup(repo => repo.GetAll()).Returns(categories);
        _categoryRepositoryMock.Setup(repo => repo.ToPaginatedListAsync<GetCategoryResponse>(It.IsAny<IQueryable<GetCategoryResponse>>(), pageSize, pageIndex))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _categoryService.GetCategoriesAsync(pageIndex, pageSize, string.Empty);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Items.Count, Is.EqualTo(2));
            _categoryRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.ToPaginatedListAsync<GetCategoryResponse>(It.IsAny<IQueryable<GetCategoryResponse>>(), pageSize, pageIndex), Times.Once);
        });
    }

    [Test]
    public async Task GetCategoriesAsync_WithKeyword_ReturnsFilteredPaginatedCategories()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var keyword = "Category 1";
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Category 1", Description = "Desc 1", Status = true, Courses = new List<Course>() },
            new Category { Id = Guid.NewGuid(), Name = "Another Category", Description = "Desc 2", Status = false, Courses = new List<Course>() }
        }.AsQueryable();

        var filteredCategoriesQuery = categories.Where(c => c.Name.Contains(keyword));

        var paginatedList = new PaginatedList<GetCategoryResponse>(
            filteredCategoriesQuery.Select(c => new GetCategoryResponse { Id = c.Id, Name = c.Name, Description = c.Description, Courses = c.Courses.Count(), Status = c.Status }).ToList(),
            filteredCategoriesQuery.Count(),
            pageIndex,
            pageSize
        );

        _categoryRepositoryMock.Setup(repo => repo.GetAll()).Returns(categories);
        _categoryRepositoryMock.Setup(repo => repo.ToPaginatedListAsync<GetCategoryResponse>(It.Is<IQueryable<GetCategoryResponse>>(q => q.Count() == filteredCategoriesQuery.Count()), pageSize, pageIndex))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _categoryService.GetCategoriesAsync(pageIndex, pageSize, keyword);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Items.Count, Is.EqualTo(1));
            Assert.That(result.Value.Items.First().Name, Is.EqualTo("Category 1"));
            _categoryRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.ToPaginatedListAsync<GetCategoryResponse>(It.Is<IQueryable<GetCategoryResponse>>(q => q.Count() == filteredCategoriesQuery.Count()), pageSize, pageIndex), Times.Once);
        });
    }

    [Test]
    [TestCase(0, 10)]
    [TestCase(10, 0)]
    [TestCase(-1, 10)]
    [TestCase(10, -1)]
    public async Task GetCategoriesAsync_InvalidPageIndexOrPageSize_ReturnsBadRequest(int pageIndex, int pageSize)
    {
        // Arrange
        var keyword = string.Empty;

        // Act
        var result = await _categoryService.GetCategoriesAsync(pageIndex, pageSize, keyword);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Error, Is.EqualTo("Page index and page size must be greater than or equal to 0"));
            _categoryRepositoryMock.Verify(repo => repo.GetAll(), Times.Never);
            _categoryRepositoryMock.Verify(repo => repo.ToPaginatedListAsync<GetCategoryResponse>(It.IsAny<IQueryable<GetCategoryResponse>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        });
    }

    [Test]
    public async Task FilterCourseByCategoryAsync_CategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var pageIndex = 1;
        var pageSize = 10;
        _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId, null)).ReturnsAsync(default(Category));

        // Act
        var result = await _categoryService.FilterCourseByCategoryAsync(categoryId, pageIndex, pageSize);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Category not found"));
            _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, null), Times.Once);
        });
    }

    [Test]
    public async Task FilterCourseByCategoryAsync_CategoryFound_ReturnsPaginatedCourses()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var pageIndex = 1;
        var pageSize = 10;
        var category = new Category { Id = categoryId, Name = "Test Category", Courses = new List<Course>() };
        var coursesForCategory = new List<Course>
        {
            new Course { Id = Guid.NewGuid(), Title = "Course 1", Description = "Desc 1", Status = CourseStatus.Published, Difficulty = CourseDifficulty.Beginner, DueDate = DateTime.UtcNow.AddDays(10), CourseTags = new List<CourseTag>(), CategoryId = categoryId, Category = category },
            new Course { Id = Guid.NewGuid(), Title = "Course 2", Description = "Desc 2", Status = CourseStatus.Draft, Difficulty = CourseDifficulty.Intermediate, DueDate = DateTime.UtcNow.AddDays(20), CourseTags = new List<CourseTag>(), CategoryId = categoryId, Category = category }
        }.AsQueryable();

        var paginatedList = new PaginatedList<FilterCourseByCategoryResponse>(
            coursesForCategory.Select(c => new FilterCourseByCategoryResponse { Id = c.Id, Title = c.Title, CategoryName = category.Name, Status = c.Status.ToString(), Description = c.Description, Difficulty = c.Difficulty.ToString(), DueDate = c.DueDate, Tags = c.CourseTags.Select(ct => ct.Tag.Name).ToList() }).ToList(),
            coursesForCategory.Count(),
            pageIndex,
            pageSize
        );

        _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(categoryId, null)).ReturnsAsync(category);
        _categoryRepositoryMock.Setup(repo => repo.FilterCourseByCategory(categoryId)).Returns(coursesForCategory);
        _categoryRepositoryMock.Setup(repo => repo.ToPaginatedListAsync<FilterCourseByCategoryResponse>(It.Is<IQueryable<FilterCourseByCategoryResponse>>(q => q.Count() == coursesForCategory.Count()), pageSize, pageIndex))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _categoryService.FilterCourseByCategoryAsync(categoryId, pageIndex, pageSize);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Items.Count, Is.EqualTo(2));
            _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, null), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.FilterCourseByCategory(categoryId), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.ToPaginatedListAsync<FilterCourseByCategoryResponse>(It.Is<IQueryable<FilterCourseByCategoryResponse>>(q => q.Count() == coursesForCategory.Count()), pageSize, pageIndex), Times.Once);
        });
    }

    [Test]
    [TestCase(0, 10)]
    [TestCase(10, 0)]
    [TestCase(-1, 10)]
    [TestCase(10, -1)]
    public async Task FilterCourseByCategoryAsync_InvalidPageIndexOrPageSize_ReturnsBadRequest(int pageIndex, int pageSize)
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act
        var result = await _categoryService.FilterCourseByCategoryAsync(categoryId, pageIndex, pageSize);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Error, Is.EqualTo("Page index and page size must be greater than or equal to 0"));
            _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>(), null), Times.Never);
            _categoryRepositoryMock.Verify(repo => repo.FilterCourseByCategory(It.IsAny<Guid>()), Times.Never);
            _categoryRepositoryMock.Verify(repo => repo.ToPaginatedListAsync<FilterCourseByCategoryResponse>(It.IsAny<IQueryable<FilterCourseByCategoryResponse>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        });
    }

    [Test]
    public async Task SoftDeleteCategoryAsync_WhenCategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(categoryId, null))
            .ReturnsAsync((Category)null);

        // Act
        var result = await _categoryService.SoftDeleteCategoryAsync(categoryId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Categories is not found or is deleted"));

            _categoryRepositoryMock.Verify(r => r.GetByIdAsync(categoryId, null), Times.Once);
            _categoryRepositoryMock.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
            _categoryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task SoftDeleteCategoryAsync_WhenSuccessful_SetsIsDeletedAndReturnsSuccess()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Test Category",
            IsDeleted = false
        };

        _categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(categoryId, null))
            .ReturnsAsync(category);

        _categoryRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _categoryService.SoftDeleteCategoryAsync(categoryId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.True);
            Assert.That(category.IsDeleted, Is.True);

            _categoryRepositoryMock.Verify(r => r.GetByIdAsync(categoryId, null), Times.Once);
            _categoryRepositoryMock.Verify(r => r.Update(category), Times.Once);
            _categoryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        });


    }

}
