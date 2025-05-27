namespace RediExpress.Application.Services;

public interface IPasswordResetService
{
    Task<bool> SendResetCodeAsync(string email);
    Task<bool> ValidateResetCode(string email, string code);
}