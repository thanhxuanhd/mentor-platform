using Contract.Dtos.Users.Requests;
using FluentValidation;
using MentorPlatformAPI.ExceptionHandler;
using MentorPlatformAPI.Extensions;
using System.Text.Json.Serialization;

namespace MentorPlatformAPI;

public static class ConfigureServices
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer()
            .AddSwaggerGenWithAuth();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
            });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddValidatorsFromAssemblyContaining<EditUserRequestValidator>();

        return services;
    }
}