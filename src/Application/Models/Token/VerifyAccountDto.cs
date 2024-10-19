using System.ComponentModel.DataAnnotations;

namespace Application.Models.Token
{
    public class VerifyAccountDto
    {
        [Required]
        public string Token { get; set; }
    }
}
