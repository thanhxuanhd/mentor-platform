namespace Contract.Dtos.Users.Responses;

public class GetUserResponse
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Role { get; set; }
}