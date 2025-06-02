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

    public async Task<Result> UpdateUserAsync(User user)
    {
        var userEntity = await _context.Users.FindAsync(user.Id);
    
        if (userEntity == null)
        {
            return Result.Failure("User not found");
        }
        
        userEntity.Email = user.Email;
        userEntity.FullName = user.FullName;
        userEntity.PasswordHash = user.PasswordHash;
        userEntity.PhoneNumber = user.PhoneNumber;

        var result = await _context.SaveChangesAsync();

        return result > 0 ? 
            Result.Success() : 
            Result.Failure("Failed to update user");
        
    }

    public async Task<Result<bool>> TryCheckUserById(Guid userId, CancellationToken cancellationToken) => 
        await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken: cancellationToken);
    
    public async Task<Result<User>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userEntity = await _context.Users
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);
        if(userEntity == null)
            return Result.Failure<User>($"User with email {userId} does not exist");

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

    public async Task<Result<IEnumerable<User>>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var userEntities = await _context.Users.AsNoTracking().ToListAsync(cancellationToken);
        var users = userEntities.Select(u => User.Create(
            u.Id,
            u.FullName,
            u.Email,
            u.PhoneNumber,
            u.PasswordHash).Value);
        return Result.Success(users);
    }
    
    
}