using Application.Services.Categories;
using Contract.Dtos.Categories.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var result = await categoryService.GetCategoryByIdAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }
    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] FilterCategoryRequest request)
    {
        request.Keyword = string.IsNullOrEmpty(request.Keyword) ? string.Empty : request.Keyword.Trim();

        var result = await categoryService.GetCategoriesAsync(request);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id}/courses")]
    public async Task<IActionResult> FilterCourseByCategory(Guid id, FilterCourseByCategoryRequest request)
    {
        var result = await categoryService.FilterCourseByCategoryAsync(id, request);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest request)
    {
        var result = await categoryService.CreateCategoryAsync(request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut]
    [Route("{categoryId}")]
    public async Task<IActionResult> EditCategory(Guid categoryId, [FromBody] CategoryRequest request)
    {
        var result = await categoryService.EditCategoryAsync(categoryId, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete]
    [Route("{categoryId}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        var result = await categoryService.SoftDeleteCategoryAsync(categoryId);
        return StatusCode((int)result.StatusCode, result);
    }
}