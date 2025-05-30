using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using RediExpress.Application.Services;
using RediExpress.Core.Model;
using RediExpress.Core.Model.ValueObjects;
using RediExpress.GeoService.Model;
using RediExpress.GeoService.Services;

namespace RediExpress.Host.Controllers;
[Route("api/[controller]")]
[ApiController]
public sealed class OrderController : BaseController
{
    private readonly IOrderService _orderService;
    private readonly IGeoService _geoService;

    public OrderController(IOrderService orderService, IGeoService geoService)
    {
        _orderService = orderService;
        _geoService = geoService;
    }

    [HttpPost("CreateOrder")]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest createOrderRequest, CancellationToken cancellationToken)
    {
        TryGetUserId(out var userId);
        
        var package = Package.Create(createOrderRequest.PackageItems, createOrderRequest.WeightOfItems, createOrderRequest.WorthOfItems);
        if(package.IsFailure)
            return FromResult(package);
        
        var originPhoneNumber = PhoneNumber.Create(createOrderRequest.OriginPhoneNumber);
        if (originPhoneNumber.IsFailure)
            return FromResult(originPhoneNumber);

        var originDetails = OrderGeo.Create(GeoPoint.FromPosString(await _geoService.GetCoordinatesAsync(createOrderRequest.OriginAddress)) ,
            originPhoneNumber.Value, await _geoService.GetFormattedAddressAsync(createOrderRequest.OriginAddress));
        
        var destinationPhoneNumber = PhoneNumber.Create(createOrderRequest.DestinationPhoneNumber);
        if (destinationPhoneNumber.IsFailure)
            return FromResult(destinationPhoneNumber);

        var destinationDetails = OrderGeo.Create(GeoPoint.FromPosString(await _geoService.GetCoordinatesAsync(createOrderRequest.DestinationAddress)) ,
            destinationPhoneNumber.Value, await _geoService.GetFormattedAddressAsync(createOrderRequest.DestinationAddress));
        if (destinationDetails.IsFailure)
            return FromResult(destinationDetails);
        
        var order = Order.Create(Guid.NewGuid(), package.Value, originDetails.Value, destinationDetails.Value, userId);
        if (order.IsFailure)
            return FromResult(order);
        
        
        var result = await _orderService.CreateOrder(userId, order.Value, cancellationToken);
        return FromResult(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetOrder(CancellationToken cancellationToken)
    {
        TryGetUserId(out var userId);
        var order = await _orderService.GetOrder(userId, cancellationToken);
        return FromResult(order);
    }
    [HttpPost("ConfirmOrder")]
    public async Task<IActionResult> ConfirmOrder(CancellationToken cancellationToken)
    {
        TryGetUserId(out var userId);
        var order = await _orderService.ConfirmOrder(userId, cancellationToken);
        return FromResult(order);
    }

    [HttpGet("GetOrders")]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
    {
        TryGetUserId(out var userId);
        
        var orders = await _orderService.GetOrders(userId, cancellationToken);
        return FromResult(orders);
    }
    private bool TryGetUserId(out Guid id)
    {
        id = Guid.Empty;
        var userId = User.FindFirst("userId")?.Value;
        if (userId is null || !Guid.TryParse(userId, out id))
            return false;

        return true;
    }
}