using Contract.Repositories;
using Contract.Services;
using Domain.Enums;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Settings;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Base;
using Infrastructure.Services.Authorization;
using Infrastructure.Services.Authorization.OAuth;
using Infrastructure.Services.Background;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add HttpClient
        services.AddHttpClient();

        // Add services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<GitHubOAuthService>();
        services.AddScoped<GoogleOAuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddSingleton<IOAuthServiceFactory, OAuthServiceFactory>();

        // Add Persistence
        services.Configure<JwtSetting>(configuration.GetSection("JwtSetting"));

        // Add repositories
        services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IExpertiseRepository, ExpertiseRepository>();
        services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
        services.AddScoped<ITeachingApproachRepository, TeachingApproachRepository>();
        services.AddScoped<IMentorApplicationRepository, MentorApplicationRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ICourseItemRepository, CourseItemRepository>();
        services.AddHostedService(provider =>
        new UserProfilePhotoCleanupService(
        provider,
        provider.GetRequiredService<IWebHostEnvironment>(),
        provider.GetRequiredService<ILogger<UserProfilePhotoCleanupService>>()
        ));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            // options.EnableSensitiveDataLogging();
        });
        services.Configure<MailSettings>(configuration.GetSection("MailSetting"));
        // Add JWT Authentication
        services.AddAuthentication(options =>
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