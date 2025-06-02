using CSharpFunctionalExtensions;
using RediExpress.Auth.Abstractions;
using RediExpress.Core.Model.Auth;
using RediExpress.Core.Model.ValueObjects;
using RediExpress.PostgreSql.Repositories;

namespace RediExpress.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result> SignUpAsync(string firstName, string lastName, string? middleName, string emailAddress, string phoneNumber, string password)
    {
        var fullName = FullName.Create(firstName, lastName, middleName);
        if(fullName.IsFailure)
            return Result.Failure(fullName.Error);

        var email = Email.Create(emailAddress);
        if (email.IsFailure)
            return Result.Failure(email.Error);
        
        var phone = PhoneNumber.Create(phoneNumber);
        if (phone.IsFailure)
            return Result.Failure(phone.Error);
        
        var phoneNumbers = await _userRepository.GetPhoneNumbers();
        var emails = await _userRepository.GetEmails();
        
        var hashedPassword = _passwordHasher.GenerateHash(password);
        
        var user = User.Create(Guid.NewGuid(), fullName.Value, email.Value, phone.Value, hashedPassword, emails, phoneNumbers);
        if(user.IsFailure)
            return Result.Failure(user.Error);
        
        var task = await _userRepository.AddUserAsync(user.Value);
        if(task.IsFailure)
            return Result.Failure(task.Error);
        
        return Result.Success("User created");
    }

    public async Task<Result<string>> SignInAsync(string email, string password)
    {
        var emailAddress = Email.Create(email);
        if (emailAddress.IsFailure)
            return Result.Failure<string>(emailAddress.Error);
        
        var user = await _userRepository.GetUserByEmailAsync(emailAddress.Value);
        if(user.IsFailure)
            return Result.Failure<string>(user.Error);
        
        var result = _passwordHasher.VerifyHash(password, user.Value.PasswordHash);
        if(!result)
            return Result.Failure<string>("Password or Email is invalid");
        
        var token = _jwtProvider.GenerateToken(user.Value);
        
        return Result.Success(token);
    }

    public async Task<Result<User>> GetUserByEmail(string email)
    {
        var emailAddress = Email.Create(email);
        if (emailAddress.IsFailure)
            return Result.Failure<User>(emailAddress.Error);
        
        return await _userRepository.GetUserByEmailAsync(emailAddress.Value);
    }

    public async Task<Result> UpdateUserAsync(User user)
    {
        return await _userRepository.UpdateUserAsync(user);
    }
    
    public async Task<Result<bool>> CheckUserByIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _userRepository.TryCheckUserById(userId, cancellationToken);
}