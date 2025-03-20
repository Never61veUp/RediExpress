using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using RediExpress.Core.Model.Auth;
using RediExpress.Core.Model.ValueObjects;
using RediExpress.PostgreSql.Model;

namespace RediExpress.PostgreSql.Repositories;

public class UserRepository : IUserRepository
{
    private readonly RediExpressDbContext _context;

    public UserRepository(RediExpressDbContext context)
    {
        _context = context;
    }

    public async Task<Result> AddUserAsync(User user)
    {
        var userEntity = new UserEntity(user.Id)
        {
            Email = user.Email,
            FullName = user.FullName,
            PasswordHash = user.PasswordHash,
            PhoneNumber = user.PhoneNumber,
        };
        
        await _context.Users.AddAsync(userEntity);
        var result = await _context.SaveChangesAsync();
        return result <= 0
            ? Result.Failure("Failed to add user")
            : Result.Success();
    }
    public async Task<Result<User>> GetUserByEmailAsync(Email email)
    {
        var userEntity = await _context.Users
            .SingleOrDefaultAsync(u => u.Email == email);
        if(userEntity == null)
            return Result.Failure<User>($"User with email {email} does not exist");

        var user = User.Create(
            userEntity.Id,
            userEntity.FullName,
            userEntity.Email,
            userEntity.PhoneNumber,
            userEntity.PasswordHash
        );
        if(user.IsFailure)
            return Result.Failure<User>(user.Error);
        return Result.Success(user.Value);
    }

    public async Task<ICollection<Email>> GetEmails() => 
        await _context.Users.AsNoTracking().Select(x => x.Email).ToListAsync();
    
    public async Task<ICollection<PhoneNumber>> GetPhoneNumbers() => 
        await _context.Users.AsNoTracking().Select(x => x.PhoneNumber).ToListAsync();
    
    
}