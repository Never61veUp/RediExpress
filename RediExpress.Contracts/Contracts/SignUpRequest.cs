using RediExpress.Core.Model.ValueObjects;

namespace RediExpress.Host.Contracts;

public record SignUpRequest(string firstName, string lastName, string? middleName, string email, string phoneNumber, string password);