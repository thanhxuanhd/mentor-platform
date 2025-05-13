using System.Security.Claims;
using System.Text;
using Contract.Services;
using Domain.Entities;
using Infrastructure.Persistence.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public sealed class JwtService(IOptions<JwtSetting> jwtSetting) : IJwtService
{
    private readonly JwtSetting _jwtSetting = jwtSetting.Value;
    public string GenerateToken(User user)
    {
        var secretKey = _jwtSetting.Secret!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name!)
            ]),
            Issuer = _jwtSetting.Issuer,
            Audience = _jwtSetting.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSetting.ExpirationInMinutes),
            SigningCredentials = credentials
        };

        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return token;
    }
}