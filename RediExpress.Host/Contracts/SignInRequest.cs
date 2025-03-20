using RediExpress.Core.Model.ValueObjects;

namespace RediExpress.Host.Contracts;

public record SignInRequest(string email, string password);