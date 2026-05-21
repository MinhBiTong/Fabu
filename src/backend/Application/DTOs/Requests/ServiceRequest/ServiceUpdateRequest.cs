using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.ServiceRequest
{
    public class ServiceUpdateRequest
    {
        public long Id { get; set; }
        public string ServiceName { get; set; }
        public string ServiceCode { get; set; }
        public string Category { get; set; }
        public int DataAmountMB { get; set; }
        public bool IsAutoRenew { get; set; }
        public int MaxActivationsPerMonth { get; set; }
        public decimal Price { get; set; }
        public int? ValidityDays { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
