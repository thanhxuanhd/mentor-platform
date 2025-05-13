using Contract.Dtos.Authentication.Requests;
using Contract.Shared;

namespace Application.Services.Authentication;

public interface IAuthService
{
    Task<Result<string>> LoginAsync(SignInRequest request);
    Task RegisterAsync(SignUpRequest request);
}