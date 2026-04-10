using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests.PermissionRequest
{
    public class PermissionCreateRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
