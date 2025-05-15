using Domain.Abstractions;

namespace Domain.Entities;

public class User : BaseEntity<Guid>
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public int RoleId { get; set; }
    
    public virtual Role Role { get; set; } = null!;
}