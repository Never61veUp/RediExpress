using System.Text.Json;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using RediExpress.Core.Model;
using RediExpress.Core.Model.ValueObjects;
using RediExpress.GeoService.Model;
using RediExpress.Redis.Model;
using StackExchange.Redis;
using Order = RediExpress.Core.Model.Order;

namespace RediExpress.Redis.Services;

public sealed class OrderRedisService : IOrderRedisService
{
    private readonly IDatabase _redis;

    public OrderRedisService([FromKeyedServices("orders")] IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }
    public async Task<Result> Add(Order order, Guid userId,
        CancellationToken cancellationToken = default)
    {
        var orderDto = OrderDto.FromDomain(order);
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, orderDto, cancellationToken: cancellationToken);
        var bytes = stream.ToArray();
        
        var setResult = await _redis.StringSetAsync(userId.ToString(), bytes, expiry: TimeSpan.FromMinutes(120));
        if(!setResult)
            return Result.Failure<Order>("Failed to save order");
                
        return Result.Success(order);
    }
    
    public async Task<Result<Order>> Get(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var bytes = await _redis.StringGetAsync(userId.ToString());
        if (bytes.IsNullOrEmpty)
            return Result.Failure<Order>($"You are trying to confirm an order that doesn't exist.");

        using var stream = new MemoryStream(bytes);
        var orderDto = await JsonSerializer.DeserializeAsync<OrderDto>(stream, cancellationToken: cancellationToken);
        if(orderDto == null)
            return Result.Failure<Order>("Failed to deserialize order");
        var order = ToDomain(orderDto);
        if(order.IsFailure)
            return Result.Failure<Order>($"Failed to deserialize order");
        
        return Result.Success(order.Value);
    }
    
    public Result<Order> ToDomain(OrderDto dto)
    {
        var originGeo = OrderGeo.Create(
            GeoPoint.FromPosString($"{dto.OriginLatitude} {dto.OriginLongitude}"),
            PhoneNumber.Create(dto.OriginPhoneNumber).Value,
            dto.OriginAddress
        );

        var destinationGeo = OrderGeo.Create(
            GeoPoint.FromPosString($"{dto.DestinationLatitude} {dto.DestinationLongitude}"),
            PhoneNumber.Create(dto.DestinationPhoneNumber).Value,
            dto.DestinationAddress
        );

        var package = Package.Create(
            dto.PackageItems, dto.WeightOfItems, dto.WorthOfItems);

        var order =  Order.Create(
            dto.Id,
            package.Value,
            originGeo.Value,
            destinationGeo.Value,
            dto.UserId
        );
        if(order.IsFailure)
            return Result.Failure<Order>($"Failed to create order");
        return Result.Success(order.Value);
    }
    
}