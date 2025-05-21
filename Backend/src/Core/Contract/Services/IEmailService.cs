namespace Contract.Services;
public interface IEmailService
{
    Task<bool> SendEmailAsync(string emailTo, string subject, string body);
}

