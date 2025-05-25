using System.Text.Json;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Distributed;
using RediExpress.Core.Model;
using RediExpress.GeoService.Extensions;
using RediExpress.WeatherService.Services;

namespace RediExpress.Application.Services;

public sealed class OrderService : IOrderService
{
    private readonly IDistributedCache _cache;

    public OrderService(IDistributedCache cache)
    {
        _cache = cache;
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
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(120)
        };
        
        await _cache.SetAsync(userId.ToString(), bytes, options, cancellationToken);
        
        return Result.Success(order);
    }

    public async Task<Result<Order>> ConfirmOrder(Guid userId, CancellationToken cancellationToken = default)
    {
        var bytes = await _cache.GetAsync(userId.ToString(), cancellationToken);
        if (bytes is null)
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
        var bytes = await _cache.GetAsync(userId.ToString(), cancellationToken);
        if (bytes is null)
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