using Application.Services.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5, [FromQuery] string keyword = "")
    {
        keyword = string.IsNullOrEmpty(keyword) ? string.Empty : keyword.Trim();

        var result = await categoryService.GetCategoriesAsync(pageIndex, pageSize, keyword);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id}/courses")]
    public async Task<IActionResult> FilterCourseByCategory(Guid id, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5)
    {
        var result = await categoryService.FilterCourseByCategoryAsync(id, pageIndex, pageSize);
        return StatusCode((int)result.StatusCode, result);
    }
}
