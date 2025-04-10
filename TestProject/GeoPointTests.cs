using Moq;
using RediExpress.Application.Services;
using RediExpress.GeoService.Extensions;
using RediExpress.GeoService.Model;

namespace TestProject;

public class GeoPointTests
{
    [Fact]
    public void FromPosString_ParsesCorrectly()
    {
        // arrange
        var input = "55.274247 25.19718";

        // act
        var point = GeoPoint.FromPosString(input);

        // assert
        Assert.Equal(25.19718, point.Latitude, 5);
        Assert.Equal(55.274247, point.Longitude, 5);
    }

    [Fact]
    public void GetDistanceTo_ReturnsCorrectDistance()
    {
        // arrange
        var dubai = GeoPoint.FromPosString("55.274247 25.19718");
        var yekaterinburg = new GeoPoint(56.8389, 60.6057);

        // act
        var distance = dubai.GetDistanceTo(yekaterinburg);

        // assert
        Assert.InRange(distance, 3530, 3550); // погрешность допустима
    }

    [Fact]
    public void GetDistanceTo_SamePoints_ReturnsZero()
    {
        // arrange
        var point1 = new GeoPoint(50.0, 50.0);
        var point2 = new GeoPoint(50.0, 50.0);

        // act
        var distance = point1.GetDistanceTo(point2);

        // assert
        Assert.Equal(0.0, distance, 5);
    }

    [Fact]
    public void FromPosString_InvalidFormat_ThrowsException()
    {
        // arrange
        var input = "55.274247"; // только одно число

        // act & assert
        Assert.Throws<ArgumentException>(() => GeoPoint.FromPosString(input));
    }
    
    [Fact]
    public void GetDistanceTo_ReturnsCorrectDistance_2()
    {
        // arrange
        var dubai = GeoPoint.FromPosString("55.274247 25.19718");
        var yekaterinburg = new GeoPoint(56.8389, 60.6057);

        // act
        var distance = dubai.GetDistanceTo(yekaterinburg);

        // assert
        Assert.InRange(distance, 3530, 3550);
    }
    
    [Fact]
    public void GetDistanceTo_ReturnsCorrectDistance_3()
    {
        // arrange
        var point1 = GeoPoint.FromPosString("60.630326 56.820681");
        var point2 = new GeoPoint(56.838661, 60.610024);

        // act
        var distance = point1.GetDistanceTo(point2);

        // assert
        Assert.InRange(distance, 2.25, 2.45);
    }
}