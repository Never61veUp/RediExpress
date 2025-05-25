using System.Text.Json;

namespace RediExpress.WeatherService.Services;


public sealed class WeatherService
{
    private static readonly string apiKey = "f102afe716c5964592cfd5c76010dd70";

    public static async Task<string> GetWeatherAsync(string city, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        var url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

        var response = await client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(content);

        if (doc.RootElement.TryGetProperty("main", out var mainElement) &&
            mainElement.TryGetProperty("temp", out var tempElement))
        {
            return tempElement.GetDouble().ToString("F1");
        }

        return null;
    }
}