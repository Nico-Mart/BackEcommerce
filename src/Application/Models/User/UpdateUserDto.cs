using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.User
{
    public class UpdateUserDto : CreateUserDto
    {
        [Required(ErrorMessage = "The product id must be specified")]
        [Key]
        public int Id { get; set; }
    }
}
