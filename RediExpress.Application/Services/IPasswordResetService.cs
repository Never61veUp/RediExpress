namespace RediExpress.Application.Services;

public interface IPasswordResetService
{
    Task<bool> SendResetCodeAsync(string email);
    bool ValidateResetCode(string email, string code);
}