using CSharpFunctionalExtensions;
using RediExpress.Core.Model.ValueObjects;

namespace RediExpress.Core.Model.Auth;

public class User : Entity<Guid>, IAggregateRoot
{
    public FullName FullName { get; }
    public Email Email { get; }
    public PhoneNumber PhoneNumber { get; }
    public string PasswordHash { get; }
    
    private User(Guid id, FullName fullName, Email email, PhoneNumber phoneNumber, string passwordHash) : base(id)
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
    }
    
    public static Result<User> Create(Guid id, FullName fullName, Email email, PhoneNumber phoneNumber, string passwordHash)
    {
        return new User(id, fullName, email, phoneNumber, passwordHash);
    }
    public static Result<User> Create(Guid id, FullName fullName, Email email, PhoneNumber phoneNumber, string passwordHash, ICollection<Email> emails, ICollection<PhoneNumber> phoneNumbers)
    {
        if(emails.Contains(email))
            return Result.Failure<User>("Email is already registered");
        
        if(phoneNumbers.Contains(phoneNumber))
            return Result.Failure<User>("Phone number is already registered");
        
        return new User(id, fullName, email, phoneNumber, passwordHash);
    }
}