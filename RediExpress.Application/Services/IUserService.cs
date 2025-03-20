using CSharpFunctionalExtensions;
using RediExpress.Core.Model.ValueObjects;

namespace RediExpress.Application.Services;

public interface IUserService
{
    Task<Result> SignUpAsync(string firstName, string lastName, string? middleName, string emailAddress, string phoneNumber, string password);
    Task<Result<string>> SignInAsync(string email, string password);
}