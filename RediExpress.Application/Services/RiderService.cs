using CSharpFunctionalExtensions;
using RediExpress.Core.Model;
using RediExpress.Core.Model.ValueObjects;
using RediExpress.Host.Contracts;
using RediExpress.PostgreSql.Repositories;

namespace RediExpress.Application.Services;

public sealed class RiderService : IRiderService
{
    private readonly IRiderRepository _riderRepository;
    private readonly IUserRepository _userRepository;

    public RiderService(IRiderRepository riderRepository, IUserRepository userRepository)
    {
        _riderRepository = riderRepository;
        _userRepository = userRepository;
    }
    public async Task<Result<IEnumerable<Rider>>> GetRidersAsync(CancellationToken token = default)
    {
        return await _riderRepository.GetRidersAsync(token);
    }

    public async Task<Result> AddRatingAsync(Rider rider, int rating, CancellationToken token = default)
    {
        rider.SetRating(rating);
        return await _riderRepository.UpdateRiderAsync(rider, token);
    }

    public async Task<Result> AddRiderAsync(Rider rider, CancellationToken token = default)
    {
        return await _riderRepository.AddRiderAsync(rider, token);
    }

    public async Task<Result<Rider>> GetRiderAsync(Guid userId, CancellationToken token = default)
    {
        return await _riderRepository.GetRiderByIdAsync(userId, token);
    }
    
    public async Task<Result> AddRatingByIdAsync(Guid userId, int rating, CancellationToken token = default)
    {
        var rider = await GetRiderAsync(userId, token);
        if(rider.IsFailure)
            return Result.Failure(rider.Error);
                
        rider.Value.SetRating(rating);
        
        return await _riderRepository.UpdateRiderAsync(rider.Value, token);
    }
    
    public async Task<Result<RidersResponse>> GetRiderWithFullNameAsync(Guid userId, CancellationToken token = default)
    {
        var rider = await _riderRepository.GetRiderByIdAsync(userId, token);
        if(rider.IsFailure)
            return Result.Failure<RidersResponse>(rider.Error);
        
        var user = await _userRepository.GetUserByIdAsync(userId, token);
        if(user.IsFailure)
            return Result.Failure<RidersResponse>(user.Error);
        
        return new RidersResponse(user.Value.FullName, rider.Value.Rating, user.Value.Id);
    }
    
    public async Task<Result<IEnumerable<RidersResponse>>> GetRidersWithFullNameAsync(CancellationToken token = default)
    {
        var riders =  await _riderRepository.GetRidersAsync(token);
        var users = await _userRepository.GetUsersAsync(token);
        
        var usersById = users.Value.ToDictionary(u => u.Id);
        
        var responses = riders.Value
            .Where(r => usersById.ContainsKey(r.UserId))
            .Select(r =>
            {
                var user = usersById[r.UserId];
                var fullName = user.FullName;
                return new RidersResponse(fullName, r.Rating, r.UserId);
            })
            .ToList();
        return responses;
    }

    public async Task<Result> AddReviewAsync(Guid riderId, string comment, int rating, Guid authorUserId, CancellationToken token = default)
    {
        var rider = await GetRiderWithReviewsByIdAsync(riderId, token);
        if(rider.IsFailure)
            return Result.Failure(rider.Error);
        
        rider.Value.AddReview(comment, rating, authorUserId, DateTime.UtcNow);
        return await _riderRepository.UpdateRiderReviewAsync(rider.Value, token);
    }

    public async Task<Result<Rider>> GetRiderWithReviewsByIdAsync(Guid riderId,  CancellationToken token = default)
    {
        return await _riderRepository.GetRiderWithReviewsByIdAsync(riderId, token);
    }
}