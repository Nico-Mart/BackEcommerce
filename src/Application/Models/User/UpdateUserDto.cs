using System.ComponentModel.DataAnnotations;

namespace Application.Models.User
{
    public class UpdateUserDto : CreateUserDto
    {
        [Required(ErrorMessage = "The product id must be specified")]
        [Key]
        public int Id { get; set; }
    }
}
