using Application.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SendEmailController : ControllerBase
    {
        //http://localhost:5055/api/v1/SendEmail/send
        //http://localhost:5055/hangfire
        private readonly IEmailService _emailService;

        public SendEmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet("send")]
        public async Task<IActionResult> Send()
        {
            string toEmail = "nguyenhuyhien2k5@gmail.com";
            string subject = "Fabu's email service to customer";
            string email_message = "< h1 > We're Fabu - solution recharge mobile </ h1 >";

            BackgroundJob.Enqueue<IEmailService>(x =>
                x.SendEmailAsync(toEmail, subject, email_message));
            return Ok("Sent email");
        }
    }
}
