using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.CouponUsageRequest
{
    public class CouponUsageUpdateRequest
    {
        public long Id { get; set; }
        public string Status { get; set; } = "Success";
    }
}
