using Application;
using Infrastructure;
using Infrastructure.Persistence.Data;
using MentorPlatformAPI;
using MentorPlatformAPI.Extensions;
using MentorPlatformAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

#region Configurations

var configuration = builder.Configuration;
configuration.AddUserSecrets<Program>();
var allowedOrigins = configuration.GetSection("AllowedOrigins").Value!.Split(';');

builder.Logging
    .AddLog4Net("log4net.config", true)
    .AddAzureWebAppDiagnostics();

#endregion

#region Services

// Add CORS policy to allow requests from specific origins
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowedOrigins",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

builder.Services
    .AddApplicationServices()
    .AddPresentationServices()
    .AddInfrastructureServices(configuration);

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwaggerWithUI();

app.ApplyMigrations();

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<MessageHub>("/message-hub");

app.UseStaticFiles();

app.SeedData();

app.Run();