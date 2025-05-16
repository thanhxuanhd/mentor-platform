namespace Contract.Services;

public interface IOAuthService
{
    Task<string?> GetAccessTokenAsync(string code);
    Task<string?> GetUserEmailDataAsync(string accessToken);
}