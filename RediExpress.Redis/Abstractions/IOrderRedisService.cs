using CSharpFunctionalExtensions;
using RediExpress.Core.Model;

namespace RediExpress.Redis.Services;

public interface IOrderRedisService
{
    Task<Result> Add(Order order, Guid userId,
        CancellationToken cancellationToken = default);
    Task<Result<Order>> Get(Guid userId,
        CancellationToken cancellationToken = default);
}