using CSharpFunctionalExtensions;

namespace RediExpress.GeoService.Services;

public interface IGeoService
{
    Task<Result<double>> GetDistance(string point1, string point2);
    Task<string> GetCoordinatesAsync(string parameter);
    Task<string> GetFormattedAddressAsync(string parameter);
}