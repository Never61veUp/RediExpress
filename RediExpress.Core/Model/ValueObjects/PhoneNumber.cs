using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace RediExpress.Core.Model.ValueObjects;

public partial class PhoneNumber : ValueObject
{
    private static readonly Regex PhoneRegex =
        MyRegex();
    
    public string Number { get; }

    private PhoneNumber(string number)
    {
        Number = number;
    }

    public static Result<PhoneNumber> Create(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return Result.Failure<PhoneNumber>("Phone number cannot be empty.");

        var normalizedNumber = Normalize(number);

        if (!PhoneRegex.IsMatch(normalizedNumber))
            return Result.Failure<PhoneNumber>("Invalid phone number format.");

        return Result.Success(new PhoneNumber(normalizedNumber));
    }
    
    private static string Normalize(string number)
    {
        return number.Replace(" ", "").Replace("-", "");
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }

    [GeneratedRegex(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}