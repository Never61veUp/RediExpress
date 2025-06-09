using Microsoft.AspNetCore.Mvc;
using RediExpress.Application.Services;
using CSharpFunctionalExtensions;
using RediExpress.Host.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace RediExpress.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseController
{
    private const string COOKIE_KEY = "tasty-cookies";
    
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    /// <summary>
    /// Регистрация
    /// </summary>
    /// <param name="signUpRequest">Входные данные</param>
    /// <returns></returns>
    [HttpPost("signUp")]
    [SwaggerOperation(Summary = "Регистрация")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest signUpRequest)
    {
        var signUpResult = await _userService.SignUpAsync(signUpRequest.firstName, signUpRequest.middleName,
            signUpRequest.lastName, signUpRequest.email, signUpRequest.phoneNumber, signUpRequest.password);
        
        return FromResult(signUpResult);
    }
    
    [HttpPost("signIn")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest signInRequest)
    {
        var token = await _userService.SignInAsync(signInRequest.email, signInRequest.password);
        if (token.IsFailure)
            return FromResult(token);
        
        HttpContext.Response.Cookies.Append(COOKIE_KEY, token.Value, new CookieOptions
        {
            HttpOnly = true,
            Secure = true
        });

        return FromResult(token);
    }

    [HttpPost("signOut")]
    public new void SignOut() => 
        HttpContext.Response.Cookies.Delete(COOKIE_KEY);
    
    [HttpGet("isLoggedIn")]
    public IActionResult IsLoggedIn() => 
        Ok(HttpContext.Request.Cookies.ContainsKey(COOKIE_KEY));
}