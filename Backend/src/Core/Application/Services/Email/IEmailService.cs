namespace Application.Services.Email;
public interface IEmailService
{
    Task<bool> SendEmailAsync(string emailTo, string subject, string body);
}
