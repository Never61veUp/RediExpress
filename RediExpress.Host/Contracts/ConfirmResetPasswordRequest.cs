namespace RediExpress.Host.Contracts;

public record ConfirmResetPasswordRequest(string Email, string Code, string NewPassword);