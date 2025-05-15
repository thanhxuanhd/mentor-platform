namespace Contract.Dtos.Authentication.Requests;

public record SignUpRequest(string Email, string Password, string ConfirmPassword, int RoleId);