using Application;
using Infrastructure;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Settings;
using Infrastructure.Services.Authorization;
using MentorPlatformAPI;
using MentorPlatformAPI.Extensions;
using MentorPlatformAPI.Filter;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
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
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSetting"));

builder.Services
    .AddApplicationServices()
    .AddPresentationServices()
    .AddInfrastructureServices(configuration);
//.AddQuartzJobs();

builder.Services.AddHostedService<MailReminderService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUI();
}

app.ApplyMigrations();

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.SeedData();

app.Run();