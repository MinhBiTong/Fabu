using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests.PermissionRequest
{
    public class PermissionUpdateRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
