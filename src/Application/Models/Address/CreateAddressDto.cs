using System.ComponentModel.DataAnnotations;

namespace Application.Models.Address
{
    public class CreateAddressDto
    {
        [Required]
        [StringLength(64, ErrorMessage = "StreetName cannot be longer than 64 characters")]
        public string StreetName { get; set; }
        [Required]
        [StringLength(8, ErrorMessage = "StreetNumber cannot be longer than 8 characters")]
        public string StreetNumber { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Province cannot be longer than 20 characters")]
        public string Province { get; set; }
        [Required]
        [StringLength(64, ErrorMessage = "Locality cannot be longer than 64 characters")]
        public string Locality { get; set; }
    }
}
