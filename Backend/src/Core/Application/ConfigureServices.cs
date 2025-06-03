using Application.Services.Authentication;
using Application.Services.Availabilities;
using Application.Services.Categories;
using Application.Services.CourseItems;
using Application.Services.Courses;
using Application.Services.Expertises;
using Application.Services.MentorDashboard;
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
        services.AddScoped<ICourseItemService, CourseItemService>();
        services.AddScoped<ITeachingApproachService, TeachingApproachService>();
        services.AddScoped<IExpertiseService, ExpertiseService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<ICourseItemService, CourseItemService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<ISessionBookingService, SessionBookingService>();
        services.AddScoped<IMentorDashboardService, MentorDashboardService>();

        return services;
    }
}