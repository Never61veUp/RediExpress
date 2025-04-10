namespace RediExpress.Application.Services;

public interface IGeoService
{
    Task<double> GetDistance(string point1, string point2);
    Task<string> GetExternalDataAsync(string parameter);
}