using Microsoft.AspNetCore.Mvc;
using RediExpress.Application.Services;
using RediExpress.GeoService.Model;

namespace RediExpress.Host.Controllers;
[Route("api/geolocation")]
[ApiController]
public class GeoController : BaseController
{
    private readonly IGeoService _geoService;

    public GeoController(IGeoService geoService)
    {
        _geoService = geoService;
    }
    [HttpGet]
    public async Task<IActionResult> GetDistance(string point1, string point2)
    {
        var result = await _geoService.GetDistance(point1, point2);
        return Ok(result);
        
    }
}