using System.Text.Json.Serialization;

namespace Infrastructure.Services.Authorization.OAuth;

public class GitHubEmailResponse
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("primary")]
    public bool Primary { get; set; }
    [JsonPropertyName("verified")]
    public bool Verified { get; set; }
}