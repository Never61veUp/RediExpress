using CSharpFunctionalExtensions;
using RediExpress.Core.Model.ValueObjects;

namespace RediExpress.PostgreSql.Model;

public class UserEntity(Guid id) : Entity<Guid>(id)
{
    public required FullName FullName { get; init; }
    public required Email Email { get; init; }
    public required PhoneNumber PhoneNumber { get; init; }
    public required string PasswordHash { get; init; }
}