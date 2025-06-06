using Contract.Services;
using Infrastructure.Persistence.Settings;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly MailSettings _mailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
    {
        _mailSettings = mailSettings.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string emailTo, string subject, string body)
    {
        _logger.LogInformation("Email: {0}, {1}, {2}, {3}, {4}", _mailSettings.SenderEmail, _mailSettings.SenderName, _mailSettings.Server, _mailSettings.Password, _mailSettings.Port);
        MailMessage message = new MailMessage(_mailSettings.SenderEmail, emailTo, subject, body);
        message.IsBodyHtml = true;
        message.BodyEncoding = Encoding.UTF8;
        message.SubjectEncoding = Encoding.UTF8;

        message.ReplyToList.Add(new MailAddress(_mailSettings.SenderEmail));
        using var smtpClient = new SmtpClient(_mailSettings.Server);
        smtpClient.Port = _mailSettings.Port;
        smtpClient.EnableSsl = true;
        smtpClient.Credentials = new NetworkCredential(_mailSettings.SenderEmail, _mailSettings.Password);
        try
        {
            await smtpClient.SendMailAsync(message);
            return true;
        }
        catch
        {
            return false;
        }
    }
}