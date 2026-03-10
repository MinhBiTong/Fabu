using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.LoginRequest
{
    public class RefreshRequest
    {
        public int UserId { get; set; } //lay tu claims sau login, frontend luu va gui kem
        public string RefreshToken { get; set; }
    }
}
