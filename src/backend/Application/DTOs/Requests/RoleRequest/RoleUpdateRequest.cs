using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests.RoleRequest
{
    public class RoleUpdateRequest
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        [Required]
        public HashSet<string> Permissions;
    }
}
