using CSharpFunctionalExtensions;
using RediExpress.Core.Model.Auth;
using RediExpress.Core.Model.ValueObjects;

namespace RediExpress.Application.Services;

public interface IUserService
{
    Task<Result> SignUpAsync(string firstName, string lastName, string? middleName, string emailAddress, string phoneNumber, string password);
    Task<Result<string>> SignInAsync(string email, string password);
    Task<Result<User>> GetUserByEmail(string email);
    Task<Result> UpdateUserAsync(User user);
    Task<Result<bool>> CheckUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}