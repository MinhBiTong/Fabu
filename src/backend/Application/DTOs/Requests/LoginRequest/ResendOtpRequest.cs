using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.LoginRequest
{
    public class ResendOtpRequest
    {
        public long? UserId { get; set; }
        public string PhoneNumber { get; set; }
    }
}
