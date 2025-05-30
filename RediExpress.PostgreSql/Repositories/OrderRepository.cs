using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using RediExpress.Core.Model;
using RediExpress.GeoService.Model;
using RediExpress.PostgreSql.Model;

namespace RediExpress.PostgreSql.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly RediExpressDbContext _context;

    public OrderRepository(RediExpressDbContext context)
    {
        _context = context;
    }

    public async Task<Result> AddOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        var destinationGeoPoint = new GeoPointEntity()
        {
            Latitude = order.DestinationDetails.GeoPoint.Latitude,
            Longitude = order.DestinationDetails.GeoPoint.Longitude
        };
        var destinationEntity = new OrderGeoEntity()
        {
            GeoPoint = destinationGeoPoint,
            Address = order.DestinationDetails.Address,
            PhoneNumber = order.DestinationDetails.PhoneNumber,
        };
        var originGeoPoint = new GeoPointEntity()
        {
            Latitude = order.OriginDetails.GeoPoint.Latitude,
            Longitude = order.OriginDetails.GeoPoint.Longitude
        };
        var originEntity = new OrderGeoEntity()
        {
            GeoPoint = originGeoPoint,
            Address = order.OriginDetails.Address,
            PhoneNumber = order.OriginDetails.PhoneNumber,
        };
        var package = new PackageEntity()
        {
            PackageItems = order.Package.PackageItems,
            WeightOfItems = order.Package.WeightOfItems,
            WorthOfItems = order.Package.WorthOfItems,
        };
        
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == order.UserId, cancellationToken: cancellationToken);
        if(user is null)
            return Result.Failure<Order>("User not found");
        
        var orderEntity = new OrderEntity(order.Id)
        {
            Package = package,
            OriginDetails = originEntity,
            DestinationDetails = destinationEntity,
            Status = order.Status,
            TotalCharges = order.TotalCharges,
            CreatedTime = DateTime.UtcNow,
            User = user
        };
        
        await _context.AddAsync(orderEntity, cancellationToken);
        var result = await _context.SaveChangesAsync(cancellationToken);
        return result <= 0
            ? Result.Failure("Failed to add order")
            : Result.Success();
    }

    public async Task<Result<IEnumerable<Order>>> GetOrders(Guid userId, CancellationToken cancellationToken = default)
    {
        var orderEntities = await _context.Orders.Where(o => o.User.Id == userId)
            .Include(orderEntity => orderEntity.User).ToListAsync(cancellationToken);
        var orderResults = orderEntities.Select(o => Order.Create(
            o.Id, Package.Create(o.Package.PackageItems, o.Package.WeightOfItems, o.Package.WorthOfItems).Value,
            OrderGeo.Create(
                GeoPoint.FromPosString($"{o.OriginDetails.GeoPoint.Latitude} {o.OriginDetails.GeoPoint.Longitude}"),
                o.OriginDetails.PhoneNumber, o.OriginDetails.Address).Value, OrderGeo.Create(
                GeoPoint.FromPosString(
                    $"{o.DestinationDetails.GeoPoint.Latitude} {o.DestinationDetails.GeoPoint.Longitude}"),
                o.DestinationDetails.PhoneNumber, o.DestinationDetails.Address).Value, o.User.Id));
        var firstFailure = orderResults.FirstOrDefault(r => r.IsFailure);
        if (firstFailure.IsFailure)
            return Result.Failure<IEnumerable<Order>>(firstFailure.Error);
        var orders = orderResults.Select(r => r.Value);
        return Result.Success(orders);
    }
}