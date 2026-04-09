using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Options
{
    public class VNPayConfiguration
    {
        [Required(ErrorMessage = "VNPay TmnCode is required")]
        public string TmnCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "VNPay HashSecret is required")]
        public string HashSecret { get; set; } = string.Empty;

        [Required]
        public string BaseUrl { get; set; } = "https://sandbox.vnpayment.vn";

        [Required]
        public string ReturnUrl { get; set; } = string.Empty;

        public string Version { get; set; } = "2.1.0";
        public string Command { get; set; } = "pay";
        public string CurrCode { get; set; } = "VND";
        public string Locale { get; set; } = "vn";
    }
}
