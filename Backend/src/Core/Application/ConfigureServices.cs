using Application.Services.Authentication;
using Application.Services.Availabilities;
using Application.Services.Categories;
using Application.Services.Courses;
using Application.Services.Expertises;
using Application.Services.MentorApplication;
using Application.Services.TeachingApproaches;
using Application.Services.Users;
using Contract.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add Application
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ITeachingApproachService, TeachingApproachService>();
        services.AddScoped<IExpertiseService, ExpertiseService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IMentorApplicationService, MentorApplicationService>();
        
        return services;
    }
}