using Application.Services.Authentication;
using Application.Services.Availabilities;
using Application.Services.Categories;
using Application.Services.CourseResources;
using Application.Services.Courses;
using Application.Services.Expertises;
using Application.Services.MentorApplications;
using Application.Services.Schedule;
using Application.Services.SessionBooking;
using Application.Services.TeachingApproaches;
using Application.Services.Users;
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
        services.AddScoped<ICourseResourceService, CourseResourceService>();
        services.AddScoped<ITeachingApproachService, TeachingApproachService>();
        services.AddScoped<IExpertiseService, ExpertiseService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IMentorApplicationService, MentorApplicationService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<ISessionBookingService, SessionBookingService>();

        return services;
    }
}