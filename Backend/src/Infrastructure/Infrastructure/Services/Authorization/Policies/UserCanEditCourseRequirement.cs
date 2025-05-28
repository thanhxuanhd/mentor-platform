using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Services.Authorization.Policies;

public class UserCanEditCourseRequirement : IAuthorizationRequirement
{
}