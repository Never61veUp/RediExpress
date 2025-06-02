using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using RediExpress.Core.Model;
using RediExpress.PostgreSql.Model;

namespace RediExpress.PostgreSql.Repositories;

public interface IRiderRepository
{
    Task<Result> AddRiderAsync(Rider rider, CancellationToken token = default);
    Task<Result> UpdateRiderAsync(Rider rider, CancellationToken token = default);
    Task<Result<IEnumerable<Rider>>> GetRidersAsync(CancellationToken token = default);
    Task<Result<Rider>> GetRiderByIdAsync(Guid userId, CancellationToken token = default);
    Task<Result<Rider>> GetRiderWithReviewsByIdAsync(Guid userId, CancellationToken token = default);
    Task<Result> UpdateRiderReviewAsync(Rider rider, CancellationToken token = default);
}

public sealed class RiderRepository : IRiderRepository
{
    private readonly RediExpressDbContext _context;

    public RiderRepository(RediExpressDbContext context)
    {
        _context = context;
    }

    public async Task<Result> AddRiderAsync(Rider rider, CancellationToken token = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == rider.UserId, cancellationToken: token);

        if (user is null)
            return Result.Failure("User not found");

        var riderEntity = new RiderEntity()
        {
            Rating = rider.Rating,
            User = user,
            RatingCount = rider.RatingCount
        };

        await _context.AddAsync(riderEntity, token);
        return await _context.SaveChangesAsync(token) > 0 
            ? Result.Success() 
            : Result.Failure("Failed to add Rider");
    }

    public async Task<Result> UpdateRiderAsync(Rider rider, CancellationToken token = default)
    {
        var riderEntity = await _context.Riders
            .FirstOrDefaultAsync(r => r.UserId == rider.UserId, cancellationToken: token);

        if (riderEntity is null)
            return Result.Failure("Rider not found");
        
        riderEntity.Rating = rider.Rating;
        riderEntity.RatingCount = rider.RatingCount;

        riderEntity.Reviews = [];
        
        foreach (var review in rider.Reviews)
        {
            riderEntity.Reviews.Add(Review.Create(
                review.Comment,
                review.Rating,
                review.AuthorUserId,
                review.CreatedAt
            ).Value);
        }
        
        _context.Riders.Update(riderEntity);
        
        return await _context.SaveChangesAsync(token) > 0
            ? Result.Success()
            : Result.Failure("Failed to update Rider");
    }
    public async Task<Result> UpdateRiderReviewAsync(Rider rider, CancellationToken token = default)
    {
        var riderEntity = await _context.Riders
            .Include(r => r.Reviews) // ← Обязательно
            .FirstOrDefaultAsync(r => r.UserId == rider.UserId, cancellationToken: token);

        if (riderEntity is null)
            return Result.Failure("Rider not found");  

        var newReviews = rider.Reviews.Except(riderEntity.Reviews).ToList();

        foreach (var review in newReviews)
        {
            riderEntity.Reviews.Add(review);
        }

        return await _context.SaveChangesAsync(token) > 0
            ? Result.Success()
            : Result.Failure("Failed to update Rider");
    }
    public async Task<Result<IEnumerable<Rider>>> GetRidersAsync(CancellationToken token = default)
    {
        var riderEntities = await _context.Riders.AsNoTracking().ToListAsync(cancellationToken: token);

        var riders = riderEntities.Select(r => Rider.Create(
            r.UserId, r.Rating, r.RatingCount).Value);
        
        return Result.Success(riders);
    }

    public async Task<Result<Rider>> GetRiderByIdAsync(Guid userId, CancellationToken token = default)
    {
        var riderEntity = await _context.Riders
            .FirstOrDefaultAsync(r => r.UserId == userId, cancellationToken: token);
        if (riderEntity is null)
            return Result.Failure<Rider>("Rider not found");
        
        var rider = Rider.Create(riderEntity.UserId, riderEntity.Rating, riderEntity.RatingCount);
        if(rider.IsFailure)
            return Result.Failure<Rider>(rider.Error);
        
        return Result.Success<Rider>(rider.Value);
    }
    
    public async Task<Result<Rider>> GetRiderWithReviewsByIdAsync(Guid userId, CancellationToken token = default)
    {
        var riderEntity = await _context.Riders
            .Include(r => r.Reviews)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.UserId == userId, cancellationToken: token);
        if (riderEntity is null)
            throw new KeyNotFoundException("Rider not found");
        
        var rider = Rider.Create(riderEntity.UserId, riderEntity.Rating, riderEntity.RatingCount, riderEntity.Reviews);
        if(rider.IsFailure)
            return Result.Failure<Rider>(rider.Error);
        
        return Result.Success<Rider>(rider.Value);
    }
    
}