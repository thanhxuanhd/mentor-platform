using Contract.Dtos.Authentication.Requests;
using Contract.Shared;

namespace Application.Services.Authentication;

public interface IAuthService
{
    Task<Result<string>> LoginAsync(SignInRequest request);
    Task RegisterAsync(SignUpRequest request);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    Task<Result<bool>> CheckEmailExistsAsync(string email);
    Task<Result> LoginGithubAsync(OAuthSignInRequest request);
    Task<Result> LoginGoogleAsync(OAuthSignInRequest request);
}