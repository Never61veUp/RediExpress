using CSharpFunctionalExtensions;

namespace RediExpress.Core.Model;

public sealed class Review : ValueObject
{
    public string Comment { get; private set; }
    public int Rating { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid AuthorUserId { get; }

    private Review(string comment, int rating, Guid authorUserId)
    {
        Comment = comment;
        Rating = rating;
        CreatedAt = DateTime.UtcNow;
        AuthorUserId = authorUserId;
    }

    public static Result<Review> Create(string comment, int rating, Guid authorUserId)
    {
        if (rating is < 1 or > 5)
            return Result.Failure<Review>("Rating must be between 1 and 5.");
        
        return Result.Success(new Review(comment, rating, authorUserId));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Comment;
        yield return Rating;
        yield return CreatedAt;
        yield return AuthorUserId;
    }
}