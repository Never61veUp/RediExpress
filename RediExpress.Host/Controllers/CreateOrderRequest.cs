namespace RediExpress.Host.Controllers;

public sealed record CreateOrderRequest(string OriginAddress, string OriginPhoneNumber, string DestinationAddress, string DestinationPhoneNumber,
    string PackageItems, float WeightOfItems, decimal WorthOfItems
);