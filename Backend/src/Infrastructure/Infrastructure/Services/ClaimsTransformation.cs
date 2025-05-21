using System.Security.Claims;
using Infrastructure.Services.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class ClaimsTransformation(CurrentUser currentUser, IHttpContextAccessor httpContextAccessor)
    : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // We're not going to transform anything. We're using this as a hook into authorization
        // to set the current user without adding custom middleware.
        currentUser.Principal = principal;

        if (principal.FindFirstValue(ClaimTypes.NameIdentifier) is { Length: > 0 } id)
        {
            currentUser.Principal = httpContextAccessor.HttpContext!.User;
        }

        return await Task.FromResult(principal);
    }
}