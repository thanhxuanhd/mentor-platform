using Contract.Services;
using Contract.Shared;
using Infrastructure.Services.Authorization.OAuth;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Authorization;

public class OAuthServiceFactory : IOAuthServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public OAuthServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IOAuthService Create(OAuthProvider provider)
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        return provider switch
        {
            OAuthProvider.GitHub => scopedProvider.GetRequiredService<GitHubOAuthService>(),
            OAuthProvider.Google => scopedProvider.GetRequiredService<GoogleOAuthService>(),
            _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
        };
    }
}