using Contract.Dtos.Authentication.Requests;
using Contract.Shared;

namespace Application.Services.Authentication;

public interface IAuthService
{
    Task<Result<string>> LoginAsync(SignInRequest request);
    Task RegisterAsync(SignUpRequest request);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    Task<Result<bool>> CheckEmailExistsAsync(string email);
    Task<(string token, string refreshToken, string role, string userId)> RefreshTokenAsync(string refreshToken);
}