using System.ComponentModel.DataAnnotations;

namespace Application.Models.Register
{
    public class UpdateRegisterUserDto
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(64, ErrorMessage = "First name cannot be longer than 64 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(64, ErrorMessage = "Last name cannot be longer than 64 characters")]
        public string LastName { get; set; }
    }
}
