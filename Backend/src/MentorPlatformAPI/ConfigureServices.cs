using System.Text.Json.Serialization;
using Contract.Dtos.Users.Requests;
using FluentValidation;
using MentorPlatformAPI.ExceptionHandler;
using MentorPlatformAPI.Extensions;
using MentorPlatformAPI.Filter;
using MentorPlatformAPI.Hubs;

namespace MentorPlatformAPI;

public static class ConfigureServices
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer()
            .AddSwaggerGenWithAuth();

        services.AddControllers(options => 
                options.Filters.Add<AutoValidateFilter>())
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
            });

        services.AddSignalR().AddHubOptions<MessageHub>(options =>
        {
            options.EnableDetailedErrors = true;
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddValidatorsFromAssemblyContaining<EditUserRequestValidator>();

        return services;
    }
}