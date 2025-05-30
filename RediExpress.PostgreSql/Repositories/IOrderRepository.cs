using CSharpFunctionalExtensions;
using RediExpress.Core.Model;

namespace RediExpress.PostgreSql.Repositories;

public interface IOrderRepository
{
    Task<Result> AddOrderAsync(Order order, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Order>>> GetOrders(Guid userId, CancellationToken cancellationToken = default);
}