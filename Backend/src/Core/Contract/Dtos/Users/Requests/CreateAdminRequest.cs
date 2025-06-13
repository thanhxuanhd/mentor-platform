using Domain.Enums;

namespace Contract.Dtos.Users.Requests;

public record class CreateAdminRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
