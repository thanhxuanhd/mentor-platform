using Application.Services.Categories;
using Application.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5, [FromQuery] string keyword = "")
        {
            keyword = string.IsNullOrEmpty(keyword) ? string.Empty : keyword.Trim();

            var result = await categoryService.GetCategoriesAsync(pageIndex, pageSize, keyword);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}
