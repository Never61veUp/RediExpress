using CSharpFunctionalExtensions;
using RediExpress.Core.Model;
using RediExpress.Core.Model.ValueObjects;

namespace RediExpress.PostgreSql.Model;

public sealed class OrderEntity(Guid id) : Entity<Guid>(id)
{
    public PackageEntity Package { get; set; }
    public OrderGeoEntity OriginDetails { get; set; }
    public OrderGeoEntity DestinationDetails { get; set; }
    public PackageStatus Status { get; set; }
    private const double ServiceCharges = 300;
    public double DeliveryCharges { get; set;}
    public double TotalCharges => DeliveryCharges + ServiceCharges;
    public DateTime CreatedTime { get; set; }
}
public sealed class PackageEntity
{
    public string PackageItems { get; set; }
    public float WeightOfItems { get; set; }
    public decimal WorthOfItems { get; set; }
}
public sealed class OrderGeoEntity
{
    public GeoPointEntity GeoPoint { get; }
    public string Address { get; }
    public PhoneNumber PhoneNumber { get; }
}
public sealed class GeoPointEntity
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}