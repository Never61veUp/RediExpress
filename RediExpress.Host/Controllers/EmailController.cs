using Microsoft.AspNetCore.Mvc;
using RediExpress.EmailService.Model;
using RediExpress.EmailService.Services;

namespace RediExpress.Host.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailController : BaseController
{
    private readonly IEmailService _mail;

    public EmailController(IEmailService mail)
    {
        _mail = mail;
    }

    [HttpPost("sendmail")]
    public async Task<IActionResult> SendMailAsync(MailData mailData)
    {
        bool result = await _mail.SendAsync(mailData);
        return result 
            ? StatusCode(StatusCodes.Status200OK, "Mail has successfully been sent.") 
            : StatusCode(StatusCodes.Status500InternalServerError, "An error occured. The Mail could not be sent.");
    }
}