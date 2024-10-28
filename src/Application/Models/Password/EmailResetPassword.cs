using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Password
{
    public class EmailResetPassword
    {
        [Required(ErrorMessage = "Email is required")]
        [StringLength(128, ErrorMessage = "Email cannot be longer than 128 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; }
    }
}
