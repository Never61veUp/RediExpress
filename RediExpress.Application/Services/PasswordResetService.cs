using Microsoft.Extensions.Caching.Memory;
using RediExpress.EmailService.Model;
using RediExpress.EmailService.Services;

namespace RediExpress.Application.Services;

public class PasswordResetService : IPasswordResetService
{
    private readonly IMemoryCache _cache;
    private readonly IEmailService _emailService;

    public PasswordResetService(IMemoryCache cache, IEmailService emailService)
    {
        _cache = cache;
        _emailService = emailService;
    }

    public async Task<bool> SendResetCodeAsync(string email)
    {
        var code = new Random().Next(100000, 999999).ToString();
        _cache.Set(email, code, TimeSpan.FromMinutes(10));
        
        var emailData = new MailData(new List<string> {email}, "Password Reset Code", 
            code, "rediexpress@yandex.ru", "", "", "",
            new List<string> {"rediexpress@yandex.ru"}, new List<string> {"rediexpress@yandex.ru"});

        await _emailService.SendAsync(emailData);
        return true;
    }

    public bool ValidateResetCode(string email, string code)
    {
        return _cache.TryGetValue(email, out string storedCode) && storedCode == code;
    }
}