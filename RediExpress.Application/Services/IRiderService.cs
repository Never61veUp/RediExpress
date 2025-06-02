using CSharpFunctionalExtensions;
using RediExpress.Core.Model;
using RediExpress.Host.Contracts;

namespace RediExpress.Application.Services;

public interface IRiderService
{
    Task<Result<IEnumerable<Rider>>> GetRidersAsync(CancellationToken token = default);
    Task<Result> AddRatingAsync(Rider rider, int rating, CancellationToken token = default);
    Task<Result> AddRiderAsync(Rider rider, CancellationToken token = default);
    Task<Result<Rider>> GetRiderAsync(Guid userId, CancellationToken token = default);
    Task<Result> AddRatingByIdAsync(Guid userId, int rating, CancellationToken token = default);
    Task<Result<RidersResponse>> GetRiderWithFullNameAsync(Guid userId, CancellationToken token = default);
    Task<Result<IEnumerable<RidersResponse>>> GetRidersWithFullNameAsync(CancellationToken token = default);
    Task<Result> AddReviewAsync(Guid riderId, string comment, int rating, Guid authorUserId,
        CancellationToken token = default);
}