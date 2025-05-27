using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using RediExpress.EmailService.Model;
using RediExpress.EmailService.Services;
using StackExchange.Redis;

namespace RediExpress.Application.Services;

public class PasswordResetService : IPasswordResetService
{
    private readonly IEmailService _emailService;
    private readonly IDatabase _redis;

    public PasswordResetService(IEmailService emailService,
        [FromKeyedServices("password-reset")] IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
        _emailService = emailService;
    }

    public async Task<bool> SendResetCodeAsync(string email)
    {
        var code = new Random().Next(100000, 999999).ToString();
        await _redis.StringSetAsync(email, code, TimeSpan.FromMinutes(10));
        
        var emailData = new MailData(new List<string> {email}, "Password Reset Code", 
            code, "rediexpress@yandex.ru", "", "", "",
            new List<string> {"rediexpress@yandex.ru"}, new List<string> {"rediexpress@yandex.ru"});

        await _emailService.SendAsync(emailData);
        return true;
    }

    public async Task<bool> ValidateResetCode(string email, string code)
    {
        var storedCode = await _redis.StringGetAsync(email);
        return storedCode.HasValue && storedCode == code;
    }
}