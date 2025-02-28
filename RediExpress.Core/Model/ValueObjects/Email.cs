using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace RediExpress.Core.Model.ValueObjects;

public partial class Email : ValueObject
{
    private static readonly Regex EmailRegex =
        MyRegex();
    public string EmailAddress { get; }

    private Email(string emailAddress)
    {
        EmailAddress = emailAddress;
    }

    public static Result<Email> Create(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            return Result.Failure<Email>("Email cannot be empty.");

        if (!EmailRegex.IsMatch(emailAddress))
            return Result.Failure<Email>("Invalid email format.");

        return Result.Success(new Email(emailAddress));
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EmailAddress;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}