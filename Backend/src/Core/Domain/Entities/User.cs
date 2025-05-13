using Domain.Abstractions;

namespace Domain.Entities;

public class User : BaseEntity
{
    //Image
    //ContactInformation (PhoneNumber)
    //Bio
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public uint RoleId { get; set; }
    public Role Role { get; set; } = null!;
}