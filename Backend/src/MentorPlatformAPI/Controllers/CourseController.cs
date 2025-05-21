using Application.Services.Users;
using Contract.Dtos.CourseItems.Requests;
using Contract.Dtos.Courses.Requests;
using Contract.Services;
using Infrastructure.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorPlatformAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CourseController(
    ICourseService courseService,
    ICourseItemService courseItemService,
    IUserService userService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CourseListRequest request)
    {
        var result = await courseService.GetAllAsync(request);
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
        var result = await courseService.CreateAsync(request.MentorId, request);
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

    [HttpPut("{id:guid}/publish")]
    [Authorize(Policy = RequiredRole.Admin)]
    public async Task<IActionResult> PublishCourse(Guid id)
    {
        var result = await courseService.PublishCourseAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id:guid}/archive")]
    public async Task<IActionResult> ArchiveCourse(CurrentUser currentUser, Guid id)
    {
        var course = await courseService.GetByIdAsync(id);
        var user = await userService.GetUserByEmailAsync(currentUser.Email);

        if (!currentUser.IsInRole(RequiredRole.Admin) && course.Value!.MentorId != user.Value!.Id) return Forbid();

        var result = await courseService.ArchiveCourseAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id:guid}/resource")]
    public async Task<IActionResult> GetAllCourseItem(Guid id)
    {
        var result = await courseItemService.GetAllByCourseIdAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id:guid}/resource/{resourceId:guid}")]
    public async Task<IActionResult> GetCourseItemById(Guid id, Guid resourceId)
    {
        var result = await courseItemService.GetByIdAsync(id, resourceId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("{id:guid}/resource")]
    public async Task<IActionResult> CreateCourseItem(Guid id, [FromBody] CourseItemCreateRequest request)
    {
        var result = await courseItemService.CreateAsync(id, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id:guid}/resource/{resourceId:guid}")]
    public async Task<IActionResult> UpdateCourseItem(Guid id, Guid resourceId,
        [FromBody] CourseItemUpdateRequest request)
    {
        var result = await courseItemService.UpdateAsync(id, resourceId, request);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{id:guid}/resource/{resourceId:guid}")]
    public async Task<IActionResult> DeleteCourseItem(Guid id, Guid resourceId)
    {
        var result = await courseItemService.DeleteAsync(id, resourceId);
        return StatusCode((int)result.StatusCode, result);
    }
}