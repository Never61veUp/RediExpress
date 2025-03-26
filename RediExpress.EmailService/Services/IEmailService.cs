using RediExpress.EmailService.Model;

namespace RediExpress.EmailService.Services;

public interface IEmailService
{
    Task<bool> SendAsync(MailData mailData);
}