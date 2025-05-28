using System.Text.Json;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using RediExpress.GeoService.Extensions;
using RediExpress.Redis.Services;
using RediExpress.WeatherService.Services;
using StackExchange.Redis;
using Order = RediExpress.Core.Model.Order;

namespace RediExpress.Application.Services;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRedisService _orderRedisService;
    private readonly IDatabase _redis;

    public OrderService([FromKeyedServices("orders")] IConnectionMultiplexer redis, IOrderRedisService orderRedisService)
    {
        _orderRedisService = orderRedisService;
        _redis = redis.GetDatabase();
    }

    public async Task<Result<Order>> CreateOrder(Guid userId, Order order, CancellationToken cancellationToken = default)
    {
        var temperature = await WeatherService.Services.WeatherService.GetWeatherAsync("Ekaterinburg", cancellationToken);
        var priceResult = order.SetDeliveryPrice(order.OriginDetails.GeoPoint.GetDistanceTo(order.DestinationDetails.GeoPoint), double.Parse(temperature));
        if (priceResult.IsFailure)
            return Result.Failure<Order>(priceResult.Error);
        
        var setResult = await _orderRedisService.Add(order, userId, cancellationToken);
        if(setResult.IsFailure)
            return Result.Failure<Order>(setResult.Error);
                
        return Result.Success(order);
    }

    public async Task<Result<Order>> ConfirmOrder(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Order>> GetOrder(Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRedisService.Get(userId, cancellationToken);
        if(order.IsFailure)
            return Result.Failure<Order>(order.Error);
        return Result.Success(order.Value);
    }
}