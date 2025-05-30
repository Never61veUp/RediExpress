﻿using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;

namespace RediExpress.Core.Model;

public enum PackageStatus
{
    Pending = 0,
    Shipped = 1,
}
public sealed class Order : Entity<Guid>
{
    public OrderGeo OriginDetails { get; private set; }
    public OrderGeo DestinationDetails { get; private set; }
    public Package Package { get; private set; }
    public PackageStatus Status { get; private set; }
    private const double ServiceCharges = 300;
    public double DeliveryCharges { get; private set;}
    public double TotalCharges => DeliveryCharges + ServiceCharges;
    public Guid UserId { get; private set; }
    
    private Order(Guid id, Package package, OrderGeo originDetails, OrderGeo destinationDetails, Guid userId)
    {
        Id = id;
        Package = package;
        OriginDetails = originDetails;
        DestinationDetails = destinationDetails;
        Status = PackageStatus.Pending;
        UserId = userId;
    }

    public static Result<Order> Create(Guid id, Package package, OrderGeo originDetails, OrderGeo destinationDetails, Guid userId)
    {
        //TODO: validation
        return new Order(id, package, originDetails, destinationDetails, userId);
    }

    public Result ConfirmOrder()
    {
        if(Status is PackageStatus.Shipped)
            return Result.Failure("Order already shipped");
        
        Status = PackageStatus.Shipped;
        return Result.Success("Order confirmed");
    }

    public Result SetDeliveryPrice(double distance, double temperature)
    {
        if (distance <= 0)
            return Result.Failure("Distance must be greater than 0");

        const double baseRate = 20.0;
        var price = distance * baseRate;
        
        if (temperature < -20)
        {
            price *= 1.2;
        }
        else if (temperature < -10)
        {
            price *= 1.1;
        }
        
        const double minimumPrice = 100;
        if (price < minimumPrice)
            price = minimumPrice;

        const double maximumPrice = 500.0;
        if (distance > 30)
        {
            if (price > maximumPrice)
                price = maximumPrice;
        }
        
        DeliveryCharges = price;
        return Result.Success(price);
    }
}