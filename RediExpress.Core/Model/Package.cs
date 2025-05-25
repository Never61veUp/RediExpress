using CSharpFunctionalExtensions;

namespace RediExpress.Core.Model;

public sealed class Package
{
    public string PackageItems { get; }
    public float WeightOfItems { get; }
    public decimal WorthOfItems { get; }

    private Package(string packageItems, float weightOfItems, decimal worthOfItems)
    {
        PackageItems = packageItems;
        WeightOfItems = weightOfItems;
        WorthOfItems = worthOfItems;
    }

    public static Result<Package> Create(string packageItems, float weightOfItems, decimal worthOfItems)
    {
        if(string.IsNullOrEmpty(packageItems))
            return Result.Failure<Package>("Package items cannot be null or empty.");
        if(packageItems.Length is >= 200 or < 10)
            return Result.Failure<Package>("Package items length must be between 10 and 200.");
        if(weightOfItems is <= 0 or < 100)
            return Result.Failure<Package>("Weight of items must be between 0 and 100.");
        if(worthOfItems is <= 10 or > 10000)
            return Result.Failure<Package>("WorthOfItems of items must be between 10 and 10000.");
        
        return Result.Success(new Package(packageItems, weightOfItems, worthOfItems));
    }
}