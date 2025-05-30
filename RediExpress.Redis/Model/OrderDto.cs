using RediExpress.Core.Model;

namespace RediExpress.Redis.Model;

public sealed class OrderDto
{
    public Guid Id { get; set; }
    public PackageStatus Status { get; set; }

    public string OriginAddress { get; set; }
    public string OriginPhoneNumber { get; set; }
    public double OriginLatitude { get; set; }
    public double OriginLongitude { get; set; }

    public string DestinationAddress { get; set; }
    public string DestinationPhoneNumber { get; set; }
    public double DestinationLatitude { get; set; }
    public double DestinationLongitude { get; set; }

    public string PackageItems { get; set; }
    public float WeightOfItems { get; set; }
    public decimal WorthOfItems { get; set; }

    public double DeliveryCharges { get; set; }
    public double TotalCharges { get; set; }
    public Guid UserId { get; set; }
    public static OrderDto FromDomain(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            Status = order.Status,

            OriginAddress = order.OriginDetails.Address,
            OriginPhoneNumber = order.OriginDetails.PhoneNumber.Number,
            OriginLatitude = order.OriginDetails.GeoPoint.Latitude,
            OriginLongitude = order.OriginDetails.GeoPoint.Longitude,

            DestinationAddress = order.DestinationDetails.Address,
            DestinationPhoneNumber = order.DestinationDetails.PhoneNumber.Number,
            DestinationLatitude = order.DestinationDetails.GeoPoint.Latitude,
            DestinationLongitude = order.DestinationDetails.GeoPoint.Longitude,

            PackageItems = order.Package.PackageItems,
            WeightOfItems = order.Package.WeightOfItems,
            WorthOfItems = order.Package.WorthOfItems,

            DeliveryCharges = order.DeliveryCharges,
            TotalCharges = order.TotalCharges,
            UserId = order.UserId,
        };
    }
}