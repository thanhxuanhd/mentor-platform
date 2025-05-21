using System.Security.Claims;
using Contract.Dtos.Courses.Responses;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Services.Authorization.Policies;

public class CourseModifyAccessHandler :
    AuthorizationHandler<CourseModifyAccessRequirement, CourseSummary>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        CourseModifyAccessRequirement requirement,
        CourseSummary resource)
    {
        if (context.User.IsInRole(nameof(UserRole.Admin)))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var userId = Guid.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (userId == resource.MentorId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}