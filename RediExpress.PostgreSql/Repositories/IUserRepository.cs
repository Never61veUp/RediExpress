using CSharpFunctionalExtensions;
using RediExpress.Core.Model.Auth;
using RediExpress.Core.Model.ValueObjects;

namespace RediExpress.PostgreSql.Repositories;

public interface IUserRepository
{
    Task<Result> AddUserAsync(User user);
    Task<Result<User>> GetUserByEmailAsync(Email email);
    Task<ICollection<Email>> GetEmails();
    Task<ICollection<PhoneNumber>> GetPhoneNumbers();
    Task<Result> UpdateUserAsync(User user);
}