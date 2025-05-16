using Domain.Enums;

namespace Contract.Dtos.Users.Responses;

public class GetUserResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int RoleId { get; set; }
    public UserStatus Status { get; set; }
    public DateOnly JoinedDate { get; set; }
    public DateOnly LastActive { get; set; }
}