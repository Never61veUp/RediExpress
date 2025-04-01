using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RediExpress.EmailService.Configuration;
using RediExpress.EmailService.Model;

namespace RediExpress.EmailService.Services;

public class EmailService : IEmailService
{
    private readonly MailSettings _settings;

    public EmailService(IOptions<MailSettings> settings) {
        _settings = settings.Value;
    }

    public async Task<bool> SendAsync(MailData mailData) {
        try {
            var mail = new MimeMessage();
            
            // Sender
            mail.From.Add(new MailboxAddress(_settings.DisplayName, mailData.From ?? _settings.From));
            mail.Sender = new MailboxAddress(mailData.DisplayName ?? _settings.DisplayName, mailData.From ?? _settings.From);

            // Receiver
            foreach (var mailAddress in mailData.To)
                mail.To.Add(MailboxAddress.Parse(mailAddress));
            
            if(!string.IsNullOrEmpty(mailData.ReplyTo))
                mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));

            // BCC
            foreach (var mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));

            // CC
            foreach (var mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));

            var body = new BodyBuilder();
            mail.Subject = mailData.Subject;
            body.HtmlBody = mailData.Body;
            mail.Body = body.ToMessageBody();

            using var client = new SmtpClient();
            try {
                await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_settings.UserName, _settings.Password);
                await client.SendAsync(mail);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
            finally {
                await client.DisconnectAsync(true);
            }

            return true;
        }
        catch (Exception) {
            return false;
        }
    }
}