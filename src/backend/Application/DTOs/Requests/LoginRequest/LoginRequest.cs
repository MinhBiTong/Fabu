using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.LoginRequest
{
    public class LoginRequest
    {
        [Required]
        [StringLength(256)]
        public string Password { get; set; }


        [EmailAddress(ErrorMessage = "Email is required")]
        [StringLength(100)]
        public string Email { get; set; }
    }
}
