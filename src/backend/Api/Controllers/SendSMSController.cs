using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SendSMSController : ControllerBase
    {
        //http://localhost:5055/api/v1/SendSMS

        private readonly ISmsService _sms;

        public SendSMSController(ISmsService sms)
        {
            _sms = sms;
        }

        [HttpGet]
        public async Task<IActionResult> Send()
        {
            var otp = new Random().Next(100000, 999999);
            string phone = "84377843050";
            var message = $"OTP {otp}";
            //await _sms.SendSmsAsync("84377843050", $"Welcome to you! We're Fabu, your OTP code: {otp}");
            await _sms.SendSmsAsync(phone, message);

            return Ok("Sent");
        }
    }
}