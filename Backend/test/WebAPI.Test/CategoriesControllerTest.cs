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
            var objectResult = result as ObjectResult;            Assert.That(objectResult, Is.Not.Null);
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
        var pageIndex = 1;
        var pageSize = 5;
        var serviceResult = Result.Failure<PaginatedList<FilterCourseByCategoryResponse>>("Category not found", HttpStatusCode.NotFound);

        var request = new FilterCourseByCategoryRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        _categoryServiceMock.Setup(s => s.FilterCourseByCategoryAsync(categoryId, It.Is<FilterCourseByCategoryRequest>(r => 
            r.PageIndex == pageIndex && r.PageSize == pageSize)))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _categoriesController.FilterCourseByCategory(categoryId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            var returnedValue = objectResult?.Value as Result<PaginatedList<FilterCourseByCategoryResponse>>;
            Assert.That(returnedValue, Is.Not.Null);
            Assert.That(returnedValue?.IsSuccess, Is.False);
            Assert.That(returnedValue?.Error, Is.EqualTo("Category not found"));
            _categoryServiceMock.Verify(s => s.FilterCourseByCategoryAsync(categoryId, It.Is<FilterCourseByCategoryRequest>(r => 
                r.PageIndex == pageIndex && r.PageSize == pageSize)), Times.Once);
        });
    }
}
