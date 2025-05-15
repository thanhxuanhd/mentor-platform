using Contract.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace Infrastructure.Services.Authorization.OAuth;

internal class GitHubOAuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IOAuthService
{
    private readonly IConfigurationSection _gitHubConfigurationSection = configuration.GetSection("GitHub");

    public async Task<string?> GetAccessTokenAsync(string code)
    {
        var client = httpClientFactory.CreateClient();

        var payload = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", _gitHubConfigurationSection["ClientId"]! },
            { "client_secret", _gitHubConfigurationSection["ClientSecret"]! }
        };

        var tokenResponse = await client.PostAsync(
            "https://github.com/login/oauth/access_token",
            new FormUrlEncodedContent(payload)
        );

        tokenResponse.EnsureSuccessStatusCode();

        var responseString = await tokenResponse.Content.ReadAsStringAsync();
        var queryParams = HttpUtility.ParseQueryString(responseString);
        var accessToken = queryParams["access_token"];

        return accessToken;
    }

    public async Task<string?> GetUserEmailDataAsync(string accessToken)
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(_gitHubConfigurationSection["UserAgent"]);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        var userResponse = await client.GetAsync("https://api.github.com/user/emails");
        userResponse.EnsureSuccessStatusCode();
        
        var json = await userResponse.Content.ReadAsStringAsync();
        var emails = JsonSerializer.Deserialize<List<GitHubEmailResponse>>(json);
        var primary = emails!.FirstOrDefault(e => e is { Primary: true, Verified: true });

        return primary?.Email;
    }
}