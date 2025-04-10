using RediExpress.GeoService.Model;

namespace RediExpress.GeoService.Extensions;

public static class GeoPointExtensions
{
    public static double GetDistanceTo(this GeoPoint point1, GeoPoint point2)
    {
        // ReSharper disable once InconsistentNaming
        const double R = 6371.0;

        var dLat = ToRadians(point2.Latitude - point1.Latitude);
        var dLon = ToRadians(point2.Longitude - point1.Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(point1.Latitude)) * Math.Cos(ToRadians(point2.Latitude)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private static double ToRadians(double deg) => deg * Math.PI / 180.0;
}