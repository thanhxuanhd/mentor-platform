using System.Security.Claims;
using Application.Services.CourseItems;
using Application.Services.Courses;
using Contract.Dtos.CourseItems.Requests;
using Contract.Dtos.Courses.Requests;
using Domain.Enums;
using Infrastructure.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Infrastructure.Services.Authorization.Policies.CourseResourcePolicyName;

namespace MentorPlatformAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CourseController(
    ICourseService courseService,
    ICourseItemService courseItemService,
    IAuthorizationService authorizationService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CourseListRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var roleName = Enum.Parse<UserRole>(HttpContext.User.FindFirstValue(ClaimTypes.Role)!);
        var serviceResult = await courseService.GetAllAsync(userId, roleName, request);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }

    [HttpGet("{courseId:guid}")]
    public async Task<IActionResult> GetById(Guid courseId)
    {
        var serviceResult = await courseService.GetByIdAsync(courseId);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }

    [HttpPost]
    [Authorize(Policy = RequiredRole.Mentor)]
    public async Task<IActionResult> Create([FromBody] CourseCreateRequest request)
    {
        var mentorId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var serviceResult = await courseService.CreateAsync(mentorId, request);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }

    [HttpPut("{courseId:guid}")]
    [Authorize(Policy = RequiredRole.Mentor)]
    public async Task<IActionResult> Update(Guid courseId, [FromBody] CourseUpdateRequest request)
    {
        var serviceResult = await courseService.UpdateAsync(courseId, request);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }

    [HttpDelete("{courseId:guid}")]
    public async Task<IActionResult> Delete(Guid courseId)
    {
        var course = await courseService.GetByIdAsync(courseId);
        if (!course.IsSuccess) return StatusCode((int)course.StatusCode, course);

        var authorization =
            await authorizationService.AuthorizeAsync(HttpContext.User, course.Value, UserCanEditCoursePolicyName);
        if (!authorization.Succeeded) return Forbid();

        var serviceResult = await courseService.DeleteAsync(courseId);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }

    [HttpPut("{courseId:guid}/publish")]
    [Authorize(Policy = RequiredRole.Admin)]
    public async Task<IActionResult> PublishCourse(Guid courseId)
    {
        var serviceResult = await courseService.PublishCourseAsync(courseId);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }

    [HttpPut("{courseId:guid}/archive")]
    public async Task<IActionResult> ArchiveCourse(Guid courseId)
    {
        var course = await courseService.GetByIdAsync(courseId);
        if (!course.IsSuccess) return StatusCode((int)course.StatusCode, course);

        var authorization =
            await authorizationService.AuthorizeAsync(HttpContext.User, course.Value, UserCanEditCoursePolicyName);
        if (!authorization.Succeeded) return Forbid();

        var serviceResult = await courseService.ArchiveCourseAsync(courseId);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }

    [HttpGet("{courseId:guid}/resource")]
    public async Task<IActionResult> GetAllCourseItem(Guid courseId)
    {
        var course = await courseService.GetByIdAsync(courseId);
        if (!course.IsSuccess) return StatusCode((int)course.StatusCode, course);

        var mentorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var roleName = HttpContext.User.FindFirstValue(ClaimTypes.Role)!;

        if (course.Value!.Status != CourseStatus.Draft
            || (course.Value!.Status == CourseStatus.Draft
                && roleName == nameof(UserRole.Admin)) || mentorId == course.Value!.MentorId)
        {
            var serviceResult = await courseItemService.GetAllByCourseIdAsync(courseId);
            return StatusCode((int)serviceResult.StatusCode, serviceResult);
        }

        return Forbid();
    }

    [HttpGet("{courseId:guid}/resource/{courseItemId:guid}")]
    public async Task<IActionResult> GetCourseItemById(Guid courseId, Guid courseItemId)
    {
        var course = await courseService.GetByIdAsync(courseId);
        if (!course.IsSuccess) return StatusCode((int)course.StatusCode, course);

        var mentorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var roleName = HttpContext.User.FindFirstValue(ClaimTypes.Role)!;

        if (course.Value!.Status != CourseStatus.Draft
            || (course.Value!.Status == CourseStatus.Draft
                && roleName == nameof(UserRole.Admin)) || mentorId == course.Value!.MentorId)
        {
            var serviceResult = await courseItemService.GetByIdAsync(courseItemId);
            return StatusCode((int)serviceResult.StatusCode, serviceResult);
        }

        return Forbid();
    }

    [HttpPost("{courseId:guid}/resource")]
    [Authorize(Policy = RequiredRole.Mentor)]
    public async Task<IActionResult> CreateCourseItem(Guid courseId, [FromBody] CourseItemCreateRequest request)
    {
        var serviceResult = await courseItemService.CreateAsync(courseId, request);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }

    [HttpPut("{courseId:guid}/resource/{courseItemId:guid}")]
    [Authorize(Policy = RequiredRole.Mentor)]
    public async Task<IActionResult> UpdateCourseItem(Guid courseId, Guid courseItemId,
        [FromBody] CourseItemUpdateRequest request)
    {
        var serviceResult = await courseItemService.UpdateAsync(courseItemId, request);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }

    [HttpDelete("{courseId:guid}/resource/{courseItemId:guid}")]
    [Authorize(Policy = RequiredRole.Mentor)]
    public async Task<IActionResult> DeleteCourseItem(Guid courseId, Guid courseItemId)
    {
        var serviceResult = await courseItemService.DeleteAsync(courseItemId);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }
}