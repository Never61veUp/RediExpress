using CSharpFunctionalExtensions;
using RediExpress.Core.Model.Auth;
using RediExpress.Core.Model.ValueObjects;

namespace RediExpress.Core.Model;

public sealed class Rider
{
    public float Rating { get; private set; }
    public int RatingCount { get; private set; }
    public Guid UserId { get; }
    private readonly List<Review> _reviews = [];
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();
    private Rider(Guid userId, float rating, int ratingCount) 
    {
        UserId = userId;
        Rating = rating;
        RatingCount = ratingCount;
    }

    public static Result<Rider> Create(Guid userId, float rating, int ratingCount)
    {
        return new Rider(userId, rating, ratingCount);
    }

    public Result SetRating(int rating)
    {
        if (rating is < 1 or > 5)
            return Result.Failure("Rating must be between 1 and 5.");

        Rating = (Rating * RatingCount + rating) / (RatingCount + 1);
        RatingCount++;
        return Result.Success();
    }
    
    public Result<Review> AddReview(string comment, int rating, Guid authorUserId)
    {
        if (rating is < 1 or > 5)
            return Result.Failure<Review>("Rating must be between 1 and 5.");

        Rating = (Rating * RatingCount + rating) / (RatingCount + 1);
        RatingCount++;

        var review = Review.Create(comment, rating, authorUserId);
        if(review.IsFailure)
            return Result.Failure<Review>(review.Error);
        
        _reviews.Add(review.Value);

        return Result.Success(review.Value);
    }
    
}