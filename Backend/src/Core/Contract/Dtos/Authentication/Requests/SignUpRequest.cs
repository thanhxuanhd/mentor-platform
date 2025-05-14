namespace Contract.Dtos.Authentication.Requests;

public record SignUpRequest(string FullName, string Password, string Email, int RoleId);