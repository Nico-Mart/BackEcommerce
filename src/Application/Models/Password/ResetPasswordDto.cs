using System.ComponentModel.DataAnnotations;

namespace Application.Models.Password
{
    public class ResetPasswordDto
    {

        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(255, ErrorMessage = "Password cannot be longer than 255 characters")]
        public string NewPassword { get; set; }

    }
}
