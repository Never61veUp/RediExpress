using System.Text.Json;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using RediExpress.GeoService.Extensions;
using RediExpress.WeatherService.Services;
using StackExchange.Redis;
using Order = RediExpress.Core.Model.Order;

namespace RediExpress.Application.Services;

public sealed class OrderService : IOrderService
{
    private readonly IDatabase _redis;

    public OrderService([FromKeyedServices("orders")] IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public async Task<Result<Order>> CreateOrder(Guid userId, Order order, CancellationToken cancellationToken = default)
    {
        var temperature = await WeatherService.Services.WeatherService.GetWeatherAsync("Ekaterinburg", cancellationToken);
        var priceResult = order.SetDeliveryPrice(order.OriginDetails.GeoPoint.GetDistanceTo(order.DestinationDetails.GeoPoint), double.Parse(temperature));
        if (priceResult.IsFailure)
            return Result.Failure<Order>(priceResult.Error);
        
        
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, order, cancellationToken: cancellationToken);
        var bytes = stream.ToArray();
        
        var setResult = await _redis.StringSetAsync(userId.ToString(), bytes, expiry: TimeSpan.FromMinutes(120));
        if(!setResult)
            return Result.Failure<Order>("Failed to create order");
                
        return Result.Success(order);
    }

    public async Task<Result<Order>> ConfirmOrder(Guid userId, CancellationToken cancellationToken = default)
    {
        var bytes = await _redis.StringGetAsync(userId.ToString());
        if (bytes.IsNullOrEmpty)
        {
            return Result.Failure<Order>($"You are trying to confirm an order that doesn't exist.");
        }

        using var stream = new MemoryStream(bytes);
        var order = await JsonSerializer.DeserializeAsync<Order>(stream, cancellationToken: cancellationToken);
        if(order is null)
            return Result.Failure<Order>($"You are trying to confirm an order that doesn't exist.");
        return Result.Success(order);
    }

    public async Task<Result<Order>> GetOrder(Guid userId, CancellationToken cancellationToken = default)
    {
        var bytes = await _redis.StringGetAsync(userId.ToString());
        if (bytes.IsNullOrEmpty)
        {
            return Result.Failure<Order>($"You are trying to get an order that doesn't exist.");
        }
        using var stream = new MemoryStream(bytes);
        var order = await JsonSerializer.DeserializeAsync<Order>(stream, cancellationToken: cancellationToken);
        if(order is null)
            return Result.Failure<Order>($"You are trying to get an order that doesn't exist.");
        return Result.Success(order);
    }
}