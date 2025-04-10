namespace RediExpress.GeoService.Model;

public class GeoPoint
{
    public double Latitude { get; }
    public double Longitude { get; }

    public GeoPoint(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    
    public static GeoPoint FromPosString(string pos)
    {
        var parts = pos.Split(' ');
        if (parts.Length != 2)
            throw new ArgumentException("Invalid POS format. Expected 'longitude latitude'");

        double longitude = double.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
        double latitude = double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);

        return new GeoPoint(latitude, longitude);
    }
}