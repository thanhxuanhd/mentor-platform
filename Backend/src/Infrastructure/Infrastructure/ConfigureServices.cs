using Contract.Repositories;
using Contract.Services;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Settings;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Domain.Enums;
using Infrastructure.Services.Authorization;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services
        services.AddScoped<IJwtService, JwtService>();

        // Add Persistence
        services.Configure<JwtSetting>(configuration.GetSection("JwtSetting"));

        // Add repositories
        services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        // Add JWT Authentication
        services.AddAuthentication((options) =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var jwtSettings = configuration.GetSection("JwtSetting").Get<JwtSetting>();
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings!.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret!))
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(RequiredRole.Admin, policy => policy.RequireRole(UserRole.Admin.ToString()));
            options.AddPolicy(RequiredRole.Mentor, policy => policy.RequireRole(UserRole.Mentor.ToString()));
            options.AddPolicy(RequiredRole.Learner, policy => policy.RequireRole(UserRole.Learner.ToString()));
        });

        return services;
    }
}