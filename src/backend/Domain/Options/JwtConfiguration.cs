using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Configurations
{
    public class JwtConfiguration
    {
        [Required]
        public string Key { get; set; } = default!;

        [Required]
        public string Issuer { get; set; } = default!;

        [Required]
        public string Audience { get; set; } = default!;

        [Range(1, 1440)]
        public int ExpiryMinutes { get; set; }
    }
}
