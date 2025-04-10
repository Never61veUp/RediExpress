using CSharpFunctionalExtensions;

namespace RediExpress.GeoService.Services;

public interface IGeoService
{
    Task<Result<double>> GetDistance(string point1, string point2);
}