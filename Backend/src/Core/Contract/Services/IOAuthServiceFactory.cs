using Contract.Shared;

namespace Contract.Services;

public interface IOAuthServiceFactory
{
    IOAuthService Create(OAuthProvider provider);
}