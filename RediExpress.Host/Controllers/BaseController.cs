using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using RediExpress.Host.Utils;

namespace RediExpress.Host.Controllers;

public class BaseController : Controller
{
    protected IActionResult FromResult<T>(Result<T> result)
    {
        return result.IsSuccess ? Ok(result.Value) : Error(result.Error);
    }
    protected IActionResult FromResult(Result result)
    {
        return result.IsSuccess ? Ok() : Error(result.Error);
    }
    protected bool TryGetUserId(out Guid id)
    {
        id = Guid.Empty;
        var userId = User.FindFirst("userId")?.Value;
        if (userId is null || !Guid.TryParse(userId, out id))
            return false;
        return true;
    }
    protected new IActionResult Ok()
    {
        return base.Ok(Envelope.Ok());
    }

    protected IActionResult Ok<T>(T result)
    {
        return base.Ok(Envelope.Ok(result));
    }

    protected IActionResult Error(string errorMessage)
    {
        return BadRequest(Envelope.Error(errorMessage));
    }
}