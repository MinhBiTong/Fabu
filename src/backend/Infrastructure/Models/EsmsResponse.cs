using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class EsmsResponse
    {
        public string CodeResult { get; set; }
        public int CountRegenerate { get; set; }
        public string? SMSID { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
