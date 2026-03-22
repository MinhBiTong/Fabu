using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.RoleRequest
{
    public class RoleCreateRequest
    {
        public string Name {  get; set; }
        public string Description { get; set; }
        public HashSet<string> Permissions;
    }
}
