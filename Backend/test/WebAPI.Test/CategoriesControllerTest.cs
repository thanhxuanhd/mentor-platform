using Application.Services.Categories;
using Contract.Dtos.Categories.Requests;
using Contract.Dtos.Categories.Responses;
using Contract.Shared;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace WebAPI.Test;

[TestFixture]
public class CategoriesControllerTest
{
    private Mock<ICategoryService> _categoryServiceMock;
    private CategoriesController _categoriesController;

    [SetUp]
    public void Setup()
    {
        _categoryServiceMock = new Mock<ICategoryService>();
        _categoriesController = new CategoriesController(_categoryServiceMock.Object);
    }

    [Test]
    public async Task GetCategories_DefaultParameters_ReturnsOkResultWithPaginatedCategories()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 5;
        var keyword = "";
        var categoriesResponse = new List<GetCategoryResponse>
        {
            new GetCategoryResponse { Id = Guid.NewGuid(), Name = "Category 1" },
            new GetCategoryResponse { Id = Guid.NewGuid(), Name = "Category 2" }
        };
        var paginatedList = new PaginatedList<GetCategoryResponse>(categoriesResponse, categoriesResponse.Count, pageIndex, pageSize);
        var serviceResult = Result.Success(paginatedList, HttpStatusCode.OK);

        var request = new FilterCategoryRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Keyword = keyword
        };

        _categoryServiceMock.Setup(s => s.GetCategoriesAsync(It.Is<FilterCategoryRequest>(r =>
            r.PageIndex == pageIndex && r.PageSize == pageSize && r.Keyword == keyword)))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _categoriesController.GetCategories(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult; Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.InstanceOf<Result<PaginatedList<GetCategoryResponse>>>());
            var returnedValue = objectResult?.Value as Result<PaginatedList<GetCategoryResponse>>;
            Assert.That(returnedValue, Is.Not.Null);
            Assert.That(returnedValue?.IsSuccess, Is.True);
            Assert.That(returnedValue.Value, Is.EqualTo(paginatedList));
            _categoryServiceMock.Verify(s => s.GetCategoriesAsync(It.Is<FilterCategoryRequest>(r =>
                r.PageIndex == pageIndex && r.PageSize == pageSize && r.Keyword == keyword)), Times.Once);
        });
    }

    [Test]
    public async Task GetCategories_WithKeywordWithSpaces_TrimsKeywordAndCallsService()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 5;
        var keywordWithSpaces = "  TestKeyword  ";
        var trimmedKeyword = "TestKeyword";
        var categoriesResponse = new List<GetCategoryResponse>();
        var paginatedList = new PaginatedList<GetCategoryResponse>(categoriesResponse, 0, pageIndex, pageSize);
        var serviceResult = Result.Success(paginatedList, HttpStatusCode.OK);

        var request = new FilterCategoryRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Keyword = keywordWithSpaces
        };

        _categoryServiceMock.Setup(s => s.GetCategoriesAsync(It.Is<FilterCategoryRequest>(r =>
            r.PageIndex == pageIndex && r.PageSize == pageSize && r.Keyword == trimmedKeyword)))
            .ReturnsAsync(serviceResult);

        // Act
        await _categoriesController.GetCategories(request);

        // Assert
        Assert.Multiple(() =>
        {
            _categoryServiceMock.Verify(s => s.GetCategoriesAsync(It.Is<FilterCategoryRequest>(r =>
                r.PageIndex == pageIndex && r.PageSize == pageSize && r.Keyword == trimmedKeyword)), Times.Once);
        });
    }

    [Test]
    public async Task FilterCourseByCategory_ServiceReturnsNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var serviceResult = Result.Failure<List<FilterCourseByCategoryResponse>>("Category not found", HttpStatusCode.NotFound);

        _categoryServiceMock.Setup(s => s.FilterCourseByCategoryAsync(categoryId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _categoriesController.FilterCourseByCategory(categoryId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            var returnedValue = objectResult?.Value as Result<List<FilterCourseByCategoryResponse>>;
            Assert.That(returnedValue?.IsSuccess, Is.False);
            Assert.That(returnedValue?.Error, Is.EqualTo("Category not found"));
            _categoryServiceMock.Verify(s => s.FilterCourseByCategoryAsync(categoryId), Times.Once);
        });
    }
    
    [Test]
    public async Task GetCategoryById_WhenCategoryExists_ReturnsOkResultWithCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categoryResponse = new GetCategoryResponse { Id = categoryId, Name = "Test Category" };
        var serviceResult = Result.Success(categoryResponse, HttpStatusCode.OK);

        _categoryServiceMock.Setup(s => s.GetCategoryByIdAsync(categoryId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _categoriesController.GetCategoryById(categoryId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _categoryServiceMock.Verify(s => s.GetCategoryByIdAsync(categoryId), Times.Once);
        });
    }

    [Test]
    public async Task GetCategoryById_WhenCategoryNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var serviceResult = Result.Failure<GetCategoryResponse>("Category not found", HttpStatusCode.NotFound);

        _categoryServiceMock.Setup(s => s.GetCategoryByIdAsync(categoryId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _categoriesController.GetCategoryById(categoryId);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
        _categoryServiceMock.Verify(s => s.GetCategoryByIdAsync(categoryId), Times.Once);
    }

    [Test]
    public async Task CreateCategory_ValidRequest_ReturnsCreatedResultWithCategory()
    {
        // Arrange
        var request = new CategoryRequest { Name = "New Category" };
        var createdCategoryResponse = new GetCategoryResponse { Id = Guid.NewGuid(), Name = request.Name };
        var serviceResult = Result.Success(createdCategoryResponse, HttpStatusCode.Created);

        _categoryServiceMock.Setup(s => s.CreateCategoryAsync(request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _categoriesController.CreateCategory(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            var returnedValue = objectResult?.Value as Result<GetCategoryResponse>;
            Assert.That(returnedValue, Is.Not.Null);
            Assert.That(returnedValue?.IsSuccess, Is.True);
            Assert.That(returnedValue?.Value, Is.EqualTo(createdCategoryResponse));
            _categoryServiceMock.Verify(s => s.CreateCategoryAsync(request), Times.Once);
        });
    }

    [Test]
    public async Task CreateCategory_ServiceReturnsError_ReturnsErrorStatusCode()
    {
        // Arrange
        var request = new CategoryRequest { Name = "New Category" };
        var serviceResult = Result.Failure<GetCategoryResponse>("Error creating category", HttpStatusCode.BadRequest);

        _categoryServiceMock.Setup(s => s.CreateCategoryAsync(request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _categoriesController.CreateCategory(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _categoryServiceMock.Verify(s => s.CreateCategoryAsync(request), Times.Once);
        });
    }

    [Test]
    public async Task EditCategory_CategoryExistsAndRequestIsValid_ReturnsOkResult()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new CategoryRequest { Name = "Updated Category" };
        var serviceResult = Result.Success(true, HttpStatusCode.OK);

        _categoryServiceMock.Setup(s => s.EditCategoryAsync(categoryId, request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _categoriesController.EditCategory(categoryId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
            _categoryServiceMock.Verify(s => s.EditCategoryAsync(categoryId, request), Times.Once);
        });
    }

    [Test]
    public async Task DeleteCategory_CategoryExists_ReturnsOkResult()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var serviceResult = Result.Success(true, HttpStatusCode.OK); // Assuming service returns bool for delete

        _categoryServiceMock.Setup(s => s.DeleteCategoryAsync(categoryId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _categoriesController.DeleteCategory(categoryId);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(objectResult?.Value, Is.EqualTo(serviceResult));
        _categoryServiceMock.Verify(s => s.DeleteCategoryAsync(categoryId), Times.Once);
    }
}
