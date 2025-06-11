using Contract.Repositories;
using Domain.Enums;
using Infrastructure.Persistence.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Logging;

public class MailReminderService : BackgroundService
{
    private readonly MailSettings _mailSettings;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MailReminderService> _logger;

    public MailReminderService(
        IOptions<MailSettings> mailSettings,
        IServiceScopeFactory scopeFactory,
        ILogger<MailReminderService> logger)
    {
        _mailSettings = mailSettings.Value;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var sessionBookingRepository = scope.ServiceProvider.GetRequiredService<ISessionsRepository>();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                await SendRemindersAsync(sessionBookingRepository, userRepository);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task SendRemindersAsync(ISessionsRepository sessionBookingRepository, IUserRepository userRepository)
    {
        var now = DateTime.UtcNow;
        var targetTime = now.AddMinutes(30);
        var sessions = await sessionBookingRepository.GetAllBookingAsync();

        foreach (var session in sessions)
        {
            if (session.Status != SessionStatus.Approved)
                continue;

            var sessionDateTime = session.TimeSlot.Date.ToDateTime(session.TimeSlot.StartTime);
            var timeUntilSession = sessionDateTime - targetTime;

            if (timeUntilSession.TotalMinutes >= 0 && timeUntilSession.TotalMinutes <= 1)
            {
                var user = await userRepository.GetByIdAsync(session.LearnerId);
                if (user == null || string.IsNullOrEmpty(user.Email)) continue;

                await SendEmailAsync(
                    user.Email,
                    "⏰ Reminder: Your session is in 30 minutes",
                    $"Hi {user.FullName}, your session is at {sessionDateTime:HH:mm} today. Please be ready!"
                );
            }
        }
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            using var smtp = new SmtpClient(_mailSettings.Server, _mailSettings.Port)
            {
                Credentials = new NetworkCredential(_mailSettings.SenderEmail, _mailSettings.Password),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(_mailSettings.SenderEmail, _mailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            message.To.Add(toEmail);
            await smtp.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email sending failed to {ToEmail}", toEmail);
        }
    }
}
