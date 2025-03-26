using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RediExpress.Application.Services;
using RediExpress.Auth.Abstractions;
using RediExpress.Host.Contracts;

namespace RediExpress.Host.Controllers;

[Route("api/password-reset")]
[ApiController]
public class PasswordResetController : ControllerBase
{
    private readonly IPasswordResetService _passwordResetService;
    private readonly IUserService _userManager;
    private readonly IPasswordHasher _passwordHasher;

    public PasswordResetController(IPasswordResetService passwordResetService, IUserService userManager, IPasswordHasher passwordHasher)
    {
        _passwordResetService = passwordResetService;
        _userManager = userManager;
        _passwordHasher = passwordHasher;
    }
    
    [HttpPost("request")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] ResetPasswordRequest request)
    {
        var user = await _userManager.GetUserByEmail(request.Email);
        if(user.IsFailure)
            return BadRequest(user.Error);

        await _passwordResetService.SendResetCodeAsync(request.Email);
        return Ok("Password reset email sent");
    }
    
    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmResetCode([FromBody] ConfirmResetPasswordRequest request)
    {
        var user = await _userManager.GetUserByEmail(request.Email);
        if(user.IsFailure)
            return BadRequest(user.Error);

        if (!_passwordResetService.ValidateResetCode(request.Email, request.Code))
            return BadRequest("Неверный код.");

        var password = _passwordHasher.GenerateHash(request.NewPassword);
        user.Value.ChangePassword(password);
        var result = await _userManager.UpdateUserAsync(user.Value);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok("Password successfully reset");
    }
}