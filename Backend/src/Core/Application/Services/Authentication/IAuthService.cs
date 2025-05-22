using Contract.Dtos.Authentication.Requests;
using Contract.Dtos.Authentication.Responses;
using Contract.Shared;

namespace Application.Services.Authentication;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(SignUpRequest request);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    Task<Result<bool>> CheckEmailExistsAsync(string email);
    Task<Result<AuthResponse>> LoginGithubAsync(OAuthSignInRequest request);
    Task<Result<AuthResponse>> LoginGoogleAsync(OAuthSignInRequest request);
    Task<Result<AuthResponse>> LoginAsync(SignInRequest request);
}