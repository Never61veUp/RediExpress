using CSharpFunctionalExtensions;

namespace RediExpress.Core.Model;

public sealed class Review : ValueObject
{
    public string Comment { get; }
    public int Rating { get; }
    public DateTime CreatedAt { get; }
    public Guid AuthorUserId { get; }

    private Review(string comment, int rating, Guid authorUserId, DateTime createdAt)
    {
        Comment = comment;
        Rating = rating;
        CreatedAt = createdAt;
        AuthorUserId = authorUserId;
    }

    public static Result<Review> Create(string comment, int rating, Guid authorUserId, DateTime createdAt)
    {
        if (rating is < 1 or > 5)
            return Result.Failure<Review>("Rating must be between 1 and 5.");
        
        return Result.Success(new Review(comment, rating, authorUserId, createdAt));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Comment;
        yield return Rating;
        yield return CreatedAt;
        yield return AuthorUserId;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Review other &&
               AuthorUserId == other.AuthorUserId &&
               Comment == other.Comment &&
               Rating == other.Rating &&
               CreatedAt == other.CreatedAt;
    }
    public override int GetHashCode() =>
        HashCode.Combine(AuthorUserId, Comment, Rating, CreatedAt);
}