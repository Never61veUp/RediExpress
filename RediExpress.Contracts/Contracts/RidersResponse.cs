using RediExpress.Core.Model.ValueObjects;

namespace RediExpress.Host.Contracts;

public record RidersResponse(
    FullName FullName, float Rating, Guid UserId);