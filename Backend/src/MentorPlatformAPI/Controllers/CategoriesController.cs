using Application.Services.Categories;
using Contract.Dtos.Categories.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [Authorize]
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

    [HttpGet("active-categories")]
    public async Task<IActionResult> GetActiveCategories()
    {
        var result = await categoryService.GetActiveCategoriesAsync();

        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize]
    [HttpGet("{id}/courses")]
    public async Task<IActionResult> FilterCourseByCategory(Guid id, FilterCourseByCategoryRequest request)
    {
        var result = await categoryService.FilterCourseByCategoryAsync(id, request);

        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest request)
    {
        var result = await categoryService.CreateCategoryAsync(request);
        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize]
    [HttpPut]
    [Route("{categoryId}")]
    public async Task<IActionResult> EditCategory(Guid categoryId, [FromBody] CategoryRequest request)
    {
        var result = await categoryService.EditCategoryAsync(categoryId, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize]
    [HttpDelete]
    [Route("{categoryId}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        var result = await categoryService.SoftDeleteCategoryAsync(categoryId);
        return StatusCode((int)result.StatusCode, result);
    }
}