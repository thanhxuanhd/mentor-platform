using Domain.Enums;

using Domain.Enums;

namespace Contract.Dtos.Users.Responses;

public class GetUserResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Role { get; set; }
}