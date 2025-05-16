namespace Contract.Dtos.Authentication.Requests;

public record SignUpRequest(string Password, string ConfirmPassword, string Email, int RoleId);