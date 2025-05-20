using Contract.Dtos.Authentication.Requests;
using Contract.Dtos.Authentication.Responses;
using Contract.Shared;

namespace Application.Services.Authentication;

public interface IAuthService
{
    Task<Result<string>> LoginAsync(SignInRequest request);
    Task RegisterAsync(SignUpRequest request);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    Task<Result<bool>> CheckEmailExistsAsync(string email);
    Task<Result<SignInResponse>> LoginGithubAsync(OAuthSignInRequest request);
    Task<Result<SignInResponse>> LoginGoogleAsync(OAuthSignInRequest request);
    Task<Result<SignInResponse>> LoginWithStatusAsync(SignInRequest request);
}