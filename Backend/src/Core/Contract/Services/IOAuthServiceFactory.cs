using Contract.Services;
using Contract.Shared;

namespace Infrastructure.Services.Authorization;

public interface IOAuthServiceFactory
{
    IOAuthService Create(OAuthProvider provider);
}