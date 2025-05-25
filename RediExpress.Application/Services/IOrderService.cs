using CSharpFunctionalExtensions;
using RediExpress.Core.Model;

namespace RediExpress.Application.Services;

public interface IOrderService
{
    Task<Result<Order>> CreateOrder(Guid userId, Order order, CancellationToken cancellationToken = default);
    Task<Result<Order>> ConfirmOrder(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<Order>> GetOrder(Guid userId, CancellationToken cancellationToken = default);
}