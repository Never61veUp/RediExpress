using System.Text.Json;
using CSharpFunctionalExtensions;
using RediExpress.GeoService.Extensions;
using RediExpress.GeoService.Model;

namespace RediExpress.GeoService.Services;

public class GeoService(HttpClient httpClient) : IGeoService
{
    public async Task<Result<double>> GetDistance(string point1, string point2)
    {
        var geo1 = await GetExternalDataAsync(point1);
        var geo2 = await GetExternalDataAsync(point2);
        var geoPoint1 = GeoPoint.FromPosString(geo1);
        var geoPoint2 = GeoPoint.FromPosString(geo2);

        var result = geoPoint1.GetDistanceTo(geoPoint2);
        return result;
    }

    private async Task<string> GetExternalDataAsync(string parameter)
    {
        var response =
            await httpClient.GetAsync(
                $"https://geocode-maps.yandex.ru/v1/?apikey=57ab92b0-665a-48e2-89b8-bed586766afa&geocode={parameter}&format=json\n");
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseData);
    
        var pos = doc.RootElement
            .GetProperty("response")
            .GetProperty("GeoObjectCollection")
            .GetProperty("featureMember")[0]
            .GetProperty("GeoObject")
            .GetProperty("Point")
            .GetProperty("pos")
            .GetString();
        if (!string.IsNullOrEmpty(pos))
        {
            var parts = pos.Split(' '); // [0] — долгота, [1] — широта

            if (parts.Length == 2)
            {
                string longitude = parts[0];
                string latitude = parts[1];
                string coordinates = $"{longitude} {latitude}";
                return coordinates;
            }
        }
        return null;
    }
}