using RediExpress.Core.Model.Auth;

namespace RediExpress.Auth.Abstractions;

public interface IJwtProvider
{
    string GenerateToken(User user);
}