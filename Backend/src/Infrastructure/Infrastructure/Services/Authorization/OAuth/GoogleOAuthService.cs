using Contract.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GoogleOAuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    : IOAuthService
{
    private readonly IConfigurationSection _googleConfig = configuration.GetSection("Google");

    public async Task<string?> GetAccessTokenAsync(string code)
    {
        var client = httpClientFactory.CreateClient();

        var payload = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", _googleConfig["ClientId"]! },
            { "client_secret", _googleConfig["ClientSecret"]! },
            { "redirect_uri", _googleConfig["RedirectUri"]! },
            { "grant_type", "authorization_code" }
        };

        var response = await client.PostAsync(
            "https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(payload)
        );

        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();

        return tokenResponse?.AccessToken;
    }

    public async Task<string?> GetUserEmailDataAsync(string accessToken)
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<GoogleUserInfoResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return userInfo?.Email;
    }

    public class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = null!;
    }

    private class GoogleUserInfoResponse
    {
        public string Email { get; init; } = null!;
        public bool EmailVerified { get; init; }
        public string Name { get; init; }
        public string Picture { get; init; }
    }
}
