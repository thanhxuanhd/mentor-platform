using System.Diagnostics;
using Infrastructure.Persistence.Data;

namespace MentorPlatformAPI;

public class DbInitializer(IServiceProvider serviceProvider, ILogger<DbInitializer> logger)
    : BackgroundService
{
    public const string ActivitySourceName = "Migrations";

    private readonly ActivitySource _activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        using var activity = _activitySource.StartActivity(ActivityKind.Client);
        if (activity != null)
        {
            activity.DisplayName = "Initializing scribe database";
        }
        
        var sw = Stopwatch.StartNew();

        await ApplicationDbExtensions.SeedAsync(dbContext, cancellationToken);
        
        logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms",
            sw.ElapsedMilliseconds);
    }
}