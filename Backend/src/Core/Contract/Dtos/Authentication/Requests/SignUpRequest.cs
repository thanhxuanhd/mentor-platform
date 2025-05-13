namespace Contract.Dtos.Authentication.Requests;

public record SignUpRequest(string Username, string Password, string Email, uint RoleId);