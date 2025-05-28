using CSharpFunctionalExtensions;
using RediExpress.Core.Model;
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
        var orderEntity = new OrderEntity(order.Id)
        {
            Package = package,
            OriginDetails = originEntity,
            DestinationDetails = destinationEntity,
            Status = order.Status,
            TotalCharges = order.TotalCharges,
            CreatedTime = DateTime.Now
        };
        await _context.AddAsync(orderEntity, cancellationToken);
        var result = await _context.SaveChangesAsync(cancellationToken);
        return result <= 0
            ? Result.Failure("Failed to add order")
            : Result.Success();
    }
}