using System.Security.Claims;
using Contract.Dtos.Courses.Responses;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Services.Authorization.Policies;

public class UserCanEditCourseAccessHandler :
    AuthorizationHandler<UserCanEditCourseRequirement, CourseSummaryResponse>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserCanEditCourseRequirement requirement,
        CourseSummaryResponse resource)
    {
        if (context.User.IsInRole(nameof(UserRole.Admin)))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (!context.User.IsInRole(nameof(UserRole.Mentor)))
        {
            context.Fail(new AuthorizationFailureReason(this, "You are not authorized to edit this course."));
            return Task.CompletedTask;
        }

        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId) || userId != resource.MentorId)
        {
            context.Fail(new AuthorizationFailureReason(this, "You are not authorized to edit this course."));
            return Task.CompletedTask;
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}