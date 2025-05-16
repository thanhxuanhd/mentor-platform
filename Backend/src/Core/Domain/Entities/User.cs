using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class User : BaseEntity<Guid>
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PasswordHash { get; set; }
    public UserStatus Status { get; set; }
    public DateOnly JoinedDate { get; set; }
    public DateOnly LastActive { get; set; }
    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;
}