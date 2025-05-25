using CSharpFunctionalExtensions;
using RediExpress.Core.Model.ValueObjects;
using RediExpress.GeoService.Model;

namespace RediExpress.Core.Model;

public sealed class OrderGeo
{
    public GeoPoint GeoPoint { get; }
    public string Address { get; }
    public PhoneNumber PhoneNumber { get; }
    
    private OrderGeo(GeoPoint geoPoint, PhoneNumber phoneNumber, string address)
    {
        GeoPoint = geoPoint;
        PhoneNumber = phoneNumber;
        Address = address;
    }

    public static Result<OrderGeo> Create(GeoPoint geoPoint, PhoneNumber phoneNumber, string address)
    {
        //TODO: validation
        return new OrderGeo(geoPoint, phoneNumber, address);
    }
}