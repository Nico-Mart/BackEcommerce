using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Models.User
{
    public class UpdateUserDto 
    {
        [Required(ErrorMessage = "The product id must be specified")]
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(64, ErrorMessage = "First name cannot be longer than 64 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(64, ErrorMessage = "Last name cannot be longer than 64 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(128, ErrorMessage = "Email cannot be longer than 128 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public Role Role { get; set; }

        [Required(ErrorMessage = "Active status must be specified")]
        public sbyte IsActive { get; set; }
    }
}
