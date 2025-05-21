using System.Security.Claims;

namespace Infrastructure.Services.Authorization;

public class CurrentUser
{
    public ClaimsPrincipal Principal { get; set; } = null!;
    public string Name => Principal.FindFirstValue(ClaimTypes.Role)!;
    public string Email => Principal.FindFirstValue("email")!;

    public bool IsInRole(string roleName)
    {
        return Name == roleName;
    }
}