using CSharpFunctionalExtensions;

namespace RediExpress.Core.Model.ValueObjects;

public class FullName : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }
    public string? MiddleName { get; }
    
    private FullName(string firstName, string lastName, string? middleName = null)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
    }
    
    public static Result<FullName> Create(string firstName, string lastName, string? middleName = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<FullName>("First name cannot be empty");

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<FullName>("Last name cannot be empty");

        return new FullName(firstName, lastName, middleName);
    }
    
    public override string ToString()
    {
        var middleInitial = string.IsNullOrWhiteSpace(MiddleName) ? "" : $" {MiddleName[0]}.";
        return $"{LastName} {FirstName[0]}.{middleInitial}";
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        if (MiddleName != null) yield return MiddleName;
    }
}