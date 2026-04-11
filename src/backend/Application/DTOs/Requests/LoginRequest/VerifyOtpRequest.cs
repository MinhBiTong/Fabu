using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.LoginRequest
{
    public class VerifyOtpRequest
    {
        public long UserId { get; set; }
        public string Otp { get; set; } = string.Empty;
    }
}
