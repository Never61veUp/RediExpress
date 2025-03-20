using RediExpress.Email.Model;

namespace RediExpress.Email.Services;

public interface IEmailService
{
    Task<bool> SendAsync(MailData mailData);
}