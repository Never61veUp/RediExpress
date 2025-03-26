using CSharpFunctionalExtensions;
using RediExpress.Core.Model.ValueObjects;

namespace RediExpress.PostgreSql.Model;

public class UserEntity(Guid id) : Entity<Guid>(id)
{
    public required FullName FullName { get; set; }
    public required Email Email { get; set; }
    public required PhoneNumber PhoneNumber { get; set; }
    public required string PasswordHash { get; set; }
}