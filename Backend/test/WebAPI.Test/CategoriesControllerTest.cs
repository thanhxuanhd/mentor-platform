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

        _categoryServiceMock.Setup(s => s.GetCategoriesAsync(pageIndex, pageSize, keyword))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _categoriesController.GetCategories();        
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.InstanceOf<Result<PaginatedList<GetCategoryResponse>>>());
            var returnedValue = objectResult.Value as Result<PaginatedList<GetCategoryResponse>>;
            Assert.That(returnedValue, Is.Not.Null);
            Assert.That(returnedValue.IsSuccess, Is.True);
            Assert.That(returnedValue.Value, Is.EqualTo(paginatedList));
            _categoryServiceMock.Verify(s => s.GetCategoriesAsync(pageIndex, pageSize, keyword), Times.Once);
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

        _categoryServiceMock.Setup(s => s.GetCategoriesAsync(pageIndex, pageSize, trimmedKeyword))
            .ReturnsAsync(serviceResult);

        // Act
        await _categoriesController.GetCategories(pageIndex, pageSize, keywordWithSpaces);        
        
        // Assert
        Assert.Multiple(() =>
        {
            _categoryServiceMock.Verify(s => s.GetCategoriesAsync(pageIndex, pageSize, trimmedKeyword), Times.Once);
        });
    }

    [Test]
    public async Task FilterCourseByCategory_ServiceReturnsNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var pageIndex = 1;
        var pageSize = 5;
        var serviceResult = Result.Failure<PaginatedList<FilterCourseByCategoryResponse>>("Category not found", HttpStatusCode.NotFound);

        _categoryServiceMock.Setup(s => s.FilterCourseByCategoryAsync(categoryId, pageIndex, pageSize))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _categoriesController.FilterCourseByCategory(categoryId, pageIndex, pageSize);        
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            var returnedValue = objectResult.Value as Result<PaginatedList<FilterCourseByCategoryResponse>>;
            Assert.That(returnedValue, Is.Not.Null);
            Assert.That(returnedValue.IsSuccess, Is.False);
            Assert.That(returnedValue.Error, Is.EqualTo("Category not found"));
            _categoryServiceMock.Verify(s => s.FilterCourseByCategoryAsync(categoryId, pageIndex, pageSize), Times.Once);
        });
    }

    //[Test]
    //public async Task CreateCategory_WhenSuccessful_ReturnsCreatedResponse()
    //{
    //    // Arrange
    //    var request = new CategoryRequest
    //    {
    //        Name = "New Category",
    //        Description = "Description",
    //        Status = true
    //    };

    //    var expectedResponse = new GetCategoryResponse
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = request.Name,
    //        Description = request.Description,
    //        Courses = 0,
    //        Status = request.Status
    //    };

    //    var result = Result.Success(expectedResponse, HttpStatusCode.Created);

    //    _categoryServiceMock
    //        .Setup(s => s.CreateCategoryAsync(request))
    //        .ReturnsAsync(result);

    //    // Act
    //    var response = await _categoriesController.CreateCategory(request) as ObjectResult;

    //    // Assert
    //    Assert.IsNotNull(response);
    //    Assert.AreEqual((int)HttpStatusCode.Created, response.StatusCode);
    //    Assert.AreEqual(result, response.Value);

    //    _categoryServiceMock.Verify(s => s.CreateCategoryAsync(request), Times.Once);
    //}

    //[Test]
    //public async Task CreateCategory_WhenNameExists_ReturnsBadRequest()
    //{
    //    // Arrange
    //    var request = new CategoryRequest
    //    {
    //        Name = "Existing",
    //        Description = "Description",
    //        Status = true
    //    };

    //    var result = Result.Failure<GetCategoryResponse>("Already have this category", HttpStatusCode.BadRequest);

    //    _categoryServiceMock
    //        .Setup(s => s.CreateCategoryAsync(request))
    //        .ReturnsAsync(result);

    //    // Act
    //    var response = await _categoriesController.CreateCategory(request) as ObjectResult;

    //    // Assert
    //    Assert.IsNotNull(response);
    //    Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
    //    Assert.AreEqual(result, response.Value);

    //    _categoryServiceMock.Verify(s => s.CreateCategoryAsync(request), Times.Once);
    //}

    //[Test]
    //public async Task EditCategory_WhenUpdateSuccessful_ReturnsOkResult()
    //{
    //    // Arrange
    //    var categoryId = Guid.NewGuid();
    //    var request = new CategoryRequest
    //    {
    //        Name = "Updated Category",
    //        Description = "Updated Description",
    //        Status = true
    //    };

    //    var result = Result.Success(true, HttpStatusCode.OK);

    //    _categoryServiceMock
    //        .Setup(s => s.EditCategoryAsync(categoryId, request))
    //        .ReturnsAsync(result);

    //    // Act
    //    var response = await _categoriesController.EditCategory(categoryId, request) as ObjectResult;

    //    // Assert
    //    Assert.IsNotNull(response);
    //    Assert.AreEqual((int)HttpStatusCode.OK, response.StatusCode);
    //    Assert.AreEqual(result, response.Value);

    //    _categoryServiceMock.Verify(s => s.EditCategoryAsync(categoryId, request), Times.Once);
    //}

    //[Test]
    //public async Task EditCategory_WhenCategoryNotFound_ReturnsNotFound()
    //{
    //    // Arrange
    //    var categoryId = Guid.NewGuid();
    //    var request = new CategoryRequest
    //    {
    //        Name = "Does Not Exist",
    //        Description = "Any",
    //        Status = false
    //    };

    //    var result = Result.Failure<bool>("Categories is not found or is deleted", HttpStatusCode.NotFound);

    //    _categoryServiceMock
    //        .Setup(s => s.EditCategoryAsync(categoryId, request))
    //        .ReturnsAsync(result);

    //    // Act
    //    var response = await _categoriesController.EditCategory(categoryId, request) as ObjectResult;

    //    // Assert
    //    Assert.IsNotNull(response);
    //    Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
    //    Assert.AreEqual(result, response.Value);

    //    _categoryServiceMock.Verify(s => s.EditCategoryAsync(categoryId, request), Times.Once);
    //}

    //[Test]
    //public async Task EditCategory_WhenNameAlreadyExists_ReturnsBadRequest()
    //{
    //    // Arrange
    //    var categoryId = Guid.NewGuid();
    //    var request = new CategoryRequest
    //    {
    //        Name = "Existing Name",
    //        Description = "Any",
    //        Status = true
    //    };

    //    var result = Result.Failure<bool>("Already have this category", HttpStatusCode.BadRequest);

    //    _categoryServiceMock
    //        .Setup(s => s.EditCategoryAsync(categoryId, request))
    //        .ReturnsAsync(result);

    //    // Act
    //    var response = await _categoriesController.EditCategory(categoryId, request) as ObjectResult;

    //    // Assert
    //    Assert.IsNotNull(response);
    //    Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
    //    Assert.AreEqual(result, response.Value);

    //    _categoryServiceMock.Verify(s => s.EditCategoryAsync(categoryId, request), Times.Once);
    //}

    //[Test]
    //public async Task ChangeCategoryStatus_WhenSuccessful_ReturnsOk()
    //{
    //    // Arrange
    //    var categoryId = Guid.NewGuid();
    //    var result = Result.Success(true, HttpStatusCode.OK);

    //    _categoryServiceMock
    //        .Setup(s => s.ChangeCategoryStatusAsync(categoryId))
    //        .ReturnsAsync(result);

    //    // Act
    //    var response = await _categoriesController.ChangeCategoryStatus(categoryId) as ObjectResult;

    //    // Assert
    //    Assert.IsNotNull(response);
    //    Assert.AreEqual((int)HttpStatusCode.OK, response.StatusCode);
    //    Assert.AreEqual(result, response.Value);

    //    _categoryServiceMock.Verify(s => s.ChangeCategoryStatusAsync(categoryId), Times.Once);
    //}

    //[Test]
    //public async Task ChangeCategoryStatus_WhenCategoryNotFound_ReturnsNotFound()
    //{
    //    // Arrange
    //    var categoryId = Guid.NewGuid();
    //    var result = Result.Failure<bool>("Categories is not found or is deleted", HttpStatusCode.NotFound);

    //    _categoryServiceMock
    //        .Setup(s => s.ChangeCategoryStatusAsync(categoryId))
    //        .ReturnsAsync(result);

    //    // Act
    //    var response = await _categoriesController.ChangeCategoryStatus(categoryId) as ObjectResult;

    //    // Assert
    //    Assert.IsNotNull(response);
    //    Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
    //    Assert.AreEqual(result, response.Value);

    //    _categoryServiceMock.Verify(s => s.ChangeCategoryStatusAsync(categoryId), Times.Once);
    //}

}
