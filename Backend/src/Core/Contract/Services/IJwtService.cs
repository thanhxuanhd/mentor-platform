using Domain.Entities;

namespace Contract.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}