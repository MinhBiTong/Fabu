using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests.UserRequest
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50)]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Email is required")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be more 10 number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool IsActive { get; set; } = true;

        // Relationship
        public virtual ICollection<Customer> Customers { get; set; }
    }
}
