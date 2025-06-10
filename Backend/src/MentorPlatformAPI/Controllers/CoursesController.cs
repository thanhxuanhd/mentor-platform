using Application.Services.CourseResources;
using Application.Services.Courses;

using Contract.Dtos.CourseResources.Requests;
using Contract.Dtos.Courses.Requests;
using Domain.Enums;
using Infrastructure.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Infrastructure.Services.Authorization.Policies.CourseResourcePolicyName;

namespace MentorPlatformAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CoursesController(
    ICourseService courseService,
    ICourseResourceService courseResourceService,
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
        if (!course.IsSuccess)
        {
            return StatusCode((int)course.StatusCode, course);
        }

        var authorization =
            await authorizationService.AuthorizeAsync(HttpContext.User, course.Value, UserCanEditCoursePolicyName);
        if (!authorization.Succeeded)
        {
            return Forbid();
        }

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
        if (!course.IsSuccess)
        {
            return StatusCode((int)course.StatusCode, course);
        }

        var authorization =
            await authorizationService.AuthorizeAsync(HttpContext.User, course.Value, UserCanEditCoursePolicyName);
        if (!authorization.Succeeded)
        {
            return Forbid();
        }

        var serviceResult = await courseService.ArchiveCourseAsync(courseId);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }

    [HttpGet("{courseId:guid}/resources")]
    public async Task<IActionResult> GetAllCourseResource(Guid courseId)
    {
        var course = await courseService.GetByIdAsync(courseId);
        if (!course.IsSuccess)
        {
            return StatusCode((int)course.StatusCode, course);
        }

        var mentorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var roleName = HttpContext.User.FindFirstValue(ClaimTypes.Role)!;

        if (course.Value!.Status != CourseStatus.Draft
            || (course.Value!.Status == CourseStatus.Draft
                && roleName == nameof(UserRole.Admin)) || mentorId == course.Value!.MentorId)
        {
            var serviceResult = await courseResourceService.GetAllByCourseIdAsync(courseId);
            return StatusCode((int)serviceResult.StatusCode, serviceResult);
        }

        return Forbid();
    }

    [HttpGet("{courseId:guid}/resources/{courseResourceId:guid}")]
    public async Task<IActionResult> GetCourseResourceById(Guid courseId, Guid courseResourceId)
    {
        var course = await courseService.GetByIdAsync(courseId);
        if (!course.IsSuccess)
        {
            return StatusCode((int)course.StatusCode, course);
        }

        var mentorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var roleName = HttpContext.User.FindFirstValue(ClaimTypes.Role)!;

        if (course.Value!.Status != CourseStatus.Draft
            || (course.Value!.Status == CourseStatus.Draft
                && roleName == nameof(UserRole.Admin)) || mentorId == course.Value!.MentorId)
        {
            var serviceResult = await courseResourceService.GetByIdAsync(courseResourceId);
            return StatusCode((int)serviceResult.StatusCode, serviceResult);
        }

        return Forbid();
    }

    [HttpPost("{courseId:guid}/resources")]
    [Authorize(Policy = RequiredRole.Mentor)]
    public async Task<IActionResult> CreateCourseResource(Guid courseId, [FromForm] CourseResourceRequest request)
    {
        var mentorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var serviceResult = await courseResourceService.CreateAsync(mentorId, courseId, request, Request);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }

    [HttpPut("{courseId:guid}/resources/{courseResourceId:guid}")]
    [Authorize(Policy = RequiredRole.Mentor)]
    public async Task<IActionResult> UpdateCourseResource(Guid courseResourceId, [FromForm] CourseResourceRequest request)
    {
        var mentorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var serviceResult = await courseResourceService.UpdateAsync(mentorId, courseResourceId, request, Request);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }

    [HttpDelete("{courseId:guid}/resources/{courseResourceId:guid}")]
    [Authorize(Policy = RequiredRole.Mentor)]
    public async Task<IActionResult> DeleteCourseResource(Guid courseId, Guid courseResourceId)
    {
        var mentorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var serviceResult = await courseResourceService.DeleteAsync(mentorId, courseResourceId);
        return StatusCode((int)serviceResult.StatusCode, serviceResult);
    }
}