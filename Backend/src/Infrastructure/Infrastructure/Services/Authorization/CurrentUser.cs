using System.Security.Claims;
using Domain.Entities;

namespace Infrastructure.Services.Authorization;

public class CurrentUser
{
    public User? User { get; set; }
    public ClaimsPrincipal Principal { get; set; } = null!;
    public string Name => Principal.FindFirstValue(ClaimTypes.Role)!;
    public bool IsInRole(string roleName) => Name == roleName;
}