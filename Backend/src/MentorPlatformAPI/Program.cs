using Application;
using Infrastructure;
using Infrastructure.Persistence.Data;
using MentorPlatformAPI;
using MentorPlatformAPI.Extensions;
using MentorPlatformAPI.Filter;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var allowedOrigins = configuration.GetSection("AllowedOrigins").Value!.Split(';');

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

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AutoValidateFilter>();
});

builder.Services
    .AddApplicationServices()
    .AddPresentationServices()
    .AddInfrastructureServices(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUI();
    app.ApplyMigrations();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.SeedData();

app.Run();
