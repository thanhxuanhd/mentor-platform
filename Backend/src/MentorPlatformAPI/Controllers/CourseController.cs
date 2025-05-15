using Contract.Dtos.Courses.Requests;
using Contract.Services;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseController(ICourseService courseService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 5)
    {
        var result = await courseService.GetAllAsync(new CourseListRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        });
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await courseService.GetByIdAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CourseCreateRequest request)
    {
        var result = await courseService.CreateAsync(request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CourseUpdateRequest request)
    {
        var result = await courseService.UpdateAsync(id, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await courseService.DeleteAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }
}