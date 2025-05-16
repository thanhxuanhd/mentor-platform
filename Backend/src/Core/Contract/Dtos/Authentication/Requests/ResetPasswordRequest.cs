namespace Contract.Dtos.Authentication.Requests;

public record ResetPasswordRequest(string Email, string NewPassword);

