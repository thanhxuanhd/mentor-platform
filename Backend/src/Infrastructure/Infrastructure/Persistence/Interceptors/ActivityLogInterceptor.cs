using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Services.Logging;
using Infrastructure.Services.Logging.Strategies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Infrastructure.Persistence.Interceptors;

public class ActivityLogInterceptor(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
    : SaveChangesInterceptor
{
    private readonly Dictionary<Type, IEntityLoggingStrategy> _loggingStrategies = new()
    {
        { typeof(User), new UserLoggingStrategy() }
    };
    private readonly List<ActivityLog> _pendingLogs = new();

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext is null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

        Guid? currentUserId = null;
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.User.Identity?.IsAuthenticated == true)
        {
            var claimUserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(claimUserId, out var userId))
            {
                var user = await dbContext.Set<User>().AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id.Equals(userId), cancellationToken);
                currentUserId = user?.Id ?? userId;
            }
        }

        var entries = dbContext.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(e => _loggingStrategies.ContainsKey(e.Entity.GetType()))
            .ToList();

        _pendingLogs.Clear();
        foreach (var entry in entries)
        {
            var strategy = _loggingStrategies[entry.Entity.GetType()];
            var action = strategy.GetLoggingAction(entry);
            if (string.IsNullOrEmpty(action)) continue;

            _pendingLogs.Add(new ActivityLog
            {
                UserId = currentUserId,
                Action = action,
                Timestamp = DateTime.UtcNow
            });
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (_pendingLogs.Count <= 0) return await base.SavedChangesAsync(eventData, result, cancellationToken);

        await using var applicationDbContext = serviceProvider.GetService<ApplicationDbContext>();
        if (applicationDbContext is null)
        {
            throw new InvalidOperationException("Could not resolve DbContext for logging.");
        }

        try
        {
            applicationDbContext.Set<ActivityLog>().AddRange(_pendingLogs);
            await applicationDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save activity logs: {ex.Message}");
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}