using Microsoft.AspNetCore.Mvc;
using RediExpress.Application.Services;
using RediExpress.GeoService.Model;
using RediExpress.GeoService.Services;

namespace RediExpress.Host.Controllers;
[Route("api/geolocation")]
[ApiController]
public class GeoController(IGeoService geoService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetDistance(string point1, string point2)
    {
        var distance = await geoService.GetDistance(point1, point2);
        return FromResult(distance);
    }
}