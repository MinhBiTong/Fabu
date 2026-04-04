using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.RoleRequest
{
    public class RoleCreateRequest
    {
        [Required]
        public string Name {  get; set; }
        public string Description { get; set; }
        [Required]
        public HashSet<string> Permissions;
    }
}
