using Contract.Repositories;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services.Background;

public class AutoCompletedSessionService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var sessionBookingRepository = scope.ServiceProvider.GetRequiredService<ISessionsRepository>();
            await AutoCompleteSessionAsync(sessionBookingRepository);

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private static async Task AutoCompleteSessionAsync(ISessionsRepository sessionBookingRepository)
    {
        var now = DateTime.UtcNow;
        var currentDate = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);
        var query = sessionBookingRepository.GetAllBookingAsync()
            .Where(s => s.Status == SessionStatus.Approved)
            .Where(s => s.TimeSlot.Date < currentDate || (s.TimeSlot.Date == currentDate && s.TimeSlot.EndTime <= currentTime));

        var pastSessions = await sessionBookingRepository.ToListAsync(query);
        foreach (var session in pastSessions)
        {
            session.Status = SessionStatus.Completed;
            sessionBookingRepository.Update(session);
        }

        await sessionBookingRepository.SaveChangesAsync();
    }
}