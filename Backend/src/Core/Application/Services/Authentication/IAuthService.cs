using Contract.Dtos.Authentication.Requests;
using Contract.Shared;

namespace Application.Services.Authentication;

public interface IAuthService
{
    Task<Result<string>> LoginAsync(SignInRequest request);
    Task<Result<string>> RegisterAsync(SignUpRequest request);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    Task<Result<bool>> CheckEmailExistsAsync(string email);
    Task<Result<string>> LoginGithubAsync(OAuthSignInRequest request);
    Task<Result<string>> LoginGoogleAsync(OAuthSignInRequest request);
}