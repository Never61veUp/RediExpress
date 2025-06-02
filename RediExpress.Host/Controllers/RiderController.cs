using Microsoft.AspNetCore.Mvc;
using RediExpress.Application.Services;
using RediExpress.Core.Model;

namespace RediExpress.Host.Controllers;
[ApiController]
[Route("/api/[controller]")]
public sealed class RiderController : BaseController
{
    private readonly IRiderService _riderService;
    private readonly IUserService _userService;

    public RiderController(IRiderService riderService, IUserService userService)
    {
        _riderService = riderService;
        _userService = userService;
    }
    
    [HttpPost("add-rider")]
    public async Task<IActionResult> AddRider(CancellationToken token = default)
    {
        TryGetUserId(out var userId);
        if (userId == Guid.Empty)
            return Unauthorized("Unauthorized");

        var result = await _userService.CheckUserByIdAsync(userId, token);
        if(result.IsFailure)
            return FromResult(result);
        
        var rider = Rider.Create(userId, 0, 0);
        if(rider.IsFailure)
            return FromResult(rider);
        
        return FromResult(await _riderService.AddRiderAsync(rider.Value, token));
    }
    [HttpPost("add-rating")]
    public async Task<IActionResult> AddRating(Guid riderId, int rating, CancellationToken token = default)
    {
        return FromResult(await _riderService.AddRatingByIdAsync(riderId, rating,  token));
    }
    [HttpGet("get-all-riders")]
    public async Task<IActionResult> GetRiders(CancellationToken token = default)
    {
        return FromResult(await _riderService.GetRidersAsync(token));
    }
    [HttpGet("get-all-riders-with-fullName")]
    public async Task<IActionResult> GetRidersWithFullNameAsync(CancellationToken token = default)
    {
        return FromResult(await _riderService.GetRidersWithFullNameAsync(token));
    }
    [HttpGet("get-rider-with-fullName")]
    public async Task<IActionResult> GetRiderWithFullNameAsync(Guid userId, CancellationToken token = default)
    {
        return FromResult(await _riderService.GetRiderWithFullNameAsync(userId, token));
    }
    [HttpPost("add-review")]
    public async Task<IActionResult> AddReview(Guid riderId, string comment ,int rating, CancellationToken token = default)
    {
        TryGetUserId(out var userId);
        return FromResult(await _riderService.AddReviewAsync(riderId, comment, rating, userId, token));
    }
}