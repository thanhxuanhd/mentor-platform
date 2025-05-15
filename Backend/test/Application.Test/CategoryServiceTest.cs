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
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(2, result.Value.Items.Count);
        _categoryRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.ToPaginatedListAsync<GetCategoryResponse>(It.IsAny<IQueryable<GetCategoryResponse>>(), pageSize, pageIndex), Times.Once);
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
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(1, result.Value.Items.Count);
        Assert.AreEqual("Category 1", result.Value.Items.First().Name);
        _categoryRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.ToPaginatedListAsync<GetCategoryResponse>(It.Is<IQueryable<GetCategoryResponse>>(q => q.Count() == filteredCategoriesQuery.Count()), pageSize, pageIndex), Times.Once);
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
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.AreEqual("Page index and page size must be greater than or equal to 0", result.Error);
        _categoryRepositoryMock.Verify(repo => repo.GetAll(), Times.Never);
        _categoryRepositoryMock.Verify(repo => repo.ToPaginatedListAsync<GetCategoryResponse>(It.IsAny<IQueryable<GetCategoryResponse>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
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
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        Assert.AreEqual("Category not found", result.Error);
        _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, null), Times.Once);
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
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(2, result.Value.Items.Count);
        _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, null), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.FilterCourseByCategory(categoryId), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.ToPaginatedListAsync<FilterCourseByCategoryResponse>(It.Is<IQueryable<FilterCourseByCategoryResponse>>(q => q.Count() == coursesForCategory.Count()), pageSize, pageIndex), Times.Once);
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
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.AreEqual("Page index and page size must be greater than or equal to 0", result.Error);
        _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>(), null), Times.Never);
        _categoryRepositoryMock.Verify(repo => repo.FilterCourseByCategory(It.IsAny<Guid>()), Times.Never);
        _categoryRepositoryMock.Verify(repo => repo.ToPaginatedListAsync<FilterCourseByCategoryResponse>(It.IsAny<IQueryable<FilterCourseByCategoryResponse>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task CreateCategoryAsync_WhenCategoryNameExists_ReturnsFailure()
    {
        // Arrange
        var request = new CategoryRequest
        {
            Name = "Existing Category",
            Description = "Some description",
            Status = true
        };

        _categoryRepositoryMock.Setup(repo => repo.ExistByNameAsync(request.Name))
            .ReturnsAsync(true);

        // Act
        var result = await _categoryService.CreateCategoryAsync(request);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.AreEqual("Already have this category", result.Error);

        _categoryRepositoryMock.Verify(repo => repo.ExistByNameAsync(request.Name), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Category>()), Times.Never);
        _categoryRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task CreateCategoryAsync_WhenCategoryNameIsUnique_ReturnsSuccess()
    {
        // Arrange
        var request = new CategoryRequest
        {
            Name = "New Category",
            Description = "Description",
            Status = true
        };

        _categoryRepositoryMock.Setup(repo => repo.ExistByNameAsync(request.Name))
            .ReturnsAsync(false);

        _categoryRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        _categoryRepositoryMock.Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _categoryService.CreateCategoryAsync(request);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.AreEqual(request.Name, result.Value.Name);
        Assert.AreEqual(request.Description, result.Value.Description);
        Assert.AreEqual(request.Status, result.Value.Status);
        Assert.AreEqual(0, result.Value.Courses);

        _categoryRepositoryMock.Verify(repo => repo.ExistByNameAsync(request.Name), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Category>(c =>
            c.Name == request.Name &&
            c.Description == request.Description &&
            c.Status == request.Status
        )), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task EditCategoryAsync_WhenNameAlreadyExists_ReturnsBadRequest()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new CategoryRequest
        {
            Name = "Existing Name",
            Description = "Updated Desc",
            Status = true
        };

        _categoryRepositoryMock.Setup(r => r.ExistByNameAsync(request.Name))
            .ReturnsAsync(true);

        // Act
        var result = await _categoryService.EditCategoryAsync(categoryId, request);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.AreEqual("Already have this category", result.Error);

        _categoryRepositoryMock.Verify(r => r.ExistByNameAsync(request.Name), Times.Once);
        _categoryRepositoryMock.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
        _categoryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task EditCategoryAsync_WhenCategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new CategoryRequest
        {
            Name = "Unique Name",
            Description = "Updated Desc",
            Status = true
        };

        _categoryRepositoryMock.Setup(r => r.ExistByNameAsync(request.Name))
            .ReturnsAsync(false);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId, null))
            .ReturnsAsync((Category)null);

        // Act
        var result = await _categoryService.EditCategoryAsync(categoryId, request);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        Assert.AreEqual("Categories is not found or is deleted", result.Error);

        _categoryRepositoryMock.Verify(r => r.ExistByNameAsync(request.Name), Times.Once);
        _categoryRepositoryMock.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
        _categoryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task EditCategoryAsync_WhenValidRequest_UpdatesCategoryAndReturnsSuccess()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new CategoryRequest
        {
            Name = "New Name",
            Description = "New Desc",
            Status = false
        };

        var existingCategory = new Category
        {
            Id = categoryId,
            Name = "Old Name",
            Description = "Old Desc",
            Status = true
        };

        _categoryRepositoryMock.Setup(r => r.ExistByNameAsync(request.Name))
            .ReturnsAsync(false);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId, null))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _categoryService.EditCategoryAsync(categoryId, request);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        Assert.IsTrue(result.Value);

        Assert.AreEqual(request.Name, existingCategory.Name);
        Assert.AreEqual(request.Description, existingCategory.Description);
        Assert.AreEqual(request.Status, existingCategory.Status);

        _categoryRepositoryMock.Verify(r => r.Update(existingCategory), Times.Once);
        _categoryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task ChangeCategoryStatusAsync_WhenCategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId, null))
            .ReturnsAsync((Category)null);

        // Act
        var result = await _categoryService.ChangeCategoryStatusAsync(categoryId);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        Assert.AreEqual("Categories is not found or is deleted", result.Error);

        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(categoryId, null), Times.Once);
        _categoryRepositoryMock.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
        _categoryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task ChangeCategoryStatusAsync_WhenCategoryExists_TogglesStatusAndReturnsSuccess()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new Category
        {
            Id = categoryId,
            Name = "Test",
            Description = "Desc",
            Status = true
        };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId, null))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _categoryService.ChangeCategoryStatusAsync(categoryId);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        Assert.IsTrue(result.Value);
        Assert.IsFalse(existingCategory.Status);

        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(categoryId, null), Times.Once);
        _categoryRepositoryMock.Verify(r => r.Update(existingCategory), Times.Once);
        _categoryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

}
