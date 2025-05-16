namespace Contract.Dtos.Authentication.Requests;

public record SignUpRequest(string FullName, string Password, string ConfirmPassword, string Email, int RoleId);