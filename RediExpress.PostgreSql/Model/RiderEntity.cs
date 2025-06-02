using RediExpress.Core.Model;

namespace RediExpress.PostgreSql.Model;

public sealed class RiderEntity
{
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    public float Rating { get; set; }
    public int RatingCount { get; set; }
    public List<Review> Reviews { get; set; }
}