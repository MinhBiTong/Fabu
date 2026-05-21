using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.CustomerServiceRequest
{
    public class CustomerServiceUpdateRequest
    {
        public long Id { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsAutoRenewed { get; set; }
    }
}
