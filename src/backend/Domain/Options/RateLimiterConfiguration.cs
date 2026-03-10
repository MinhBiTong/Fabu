using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Configurations
{
    public class RateLimiterConfiguration
    {
        [Range(1, 1000)]
        public int PermitLimit { get; set; }

        [Range(1, 3600)]
        public int WindowInSeconds { get; set; }

        [Range(0, 100)]
        public int QueueLimit { get; set; }
    }
}
