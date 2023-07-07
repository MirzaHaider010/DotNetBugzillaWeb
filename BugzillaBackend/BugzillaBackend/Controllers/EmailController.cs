using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;

namespace BugzillaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //public EmailController(IHttpContextAccessor httpContextAccessor) 
        //{
        //    _httpContextAccessor = httpContextAccessor;
        //}

        //[HttpPost]
        //public IActionResult SendVerificationEmail(string verificationToken)
        //{
        //    var verificationUrl = CreateVerificationUrl(verificationToken);

        //    var email = new MimeMessage();
        //    email.From.Add(MailboxAddress.Parse("haider.ejaz@devsinc.com"));
        //    email.To.Add(MailboxAddress.Parse("haider.ejaz@devsinc.com"));
        //    email.Subject = "Verification Email";
        //    email.Body = new TextPart(TextFormat.Html) { Text = verificationUrl };

        //    using var smtp = new SmtpClient();
        //    smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        //    smtp.Authenticate("haider.ejaz@devsinc.com", "bwdwvmxlrsiuvkyb");
        //    smtp.Send(email);
        //    smtp.Disconnect(true);

        //    return Ok();
        //}

        //private string CreateVerificationUrl(string token)
        //{
        //    var appUrl = getApplicationUrl();
        //    return $"{appUrl}/api/auth/verify?token={token}";
        //}

        //private string getApplicationUrl()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    var request = httpContext.Request;
        //    var applicationUrl = $"{request.Scheme}://{request.Host}";
        //    return applicationUrl;
        //}
    }
}
