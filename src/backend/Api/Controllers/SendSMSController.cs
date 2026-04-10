using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SendSMSController : ControllerBase
    {
        //http://localhost:5000/api/v1/SendSMS

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
            var message = $"Welcome to you! We're Fabu, your OTP code: {otp}";
            var result = await _sms.SendSmsAsync(phone, message);

            //var message = "Cam on quy khach da su dung dich vu cua chung toi. Chuc quy khach mot ngay tot lanh!";
            return Ok(result);
        }
    }
}