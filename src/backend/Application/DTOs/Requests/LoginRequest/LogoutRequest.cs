using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.LoginRequest
{
    public class LogoutRequest
    {
        public int UserId { get; set; } //lay tu claims frontend luu
        public string AccessToken { get; set; } = string.Empty; //gui AT de blacklist ngaylap tuc
    }
}
