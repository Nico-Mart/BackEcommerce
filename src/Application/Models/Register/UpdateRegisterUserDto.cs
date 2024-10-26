
using System.ComponentModel.DataAnnotations;

namespace Application.Models.Register
{
    public class UpdateRegisterUserDto : CreateRegisterUserDto
    {
        [Required(ErrorMessage = "The product id must be specified")]
        [Key]
        public int Id { get; set; }
    }
}
