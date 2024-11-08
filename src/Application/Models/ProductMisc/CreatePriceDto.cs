using System.ComponentModel.DataAnnotations;

namespace Application.Models.ProductMisc
{
    public class CreatePriceDto
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        [Required(ErrorMessage = "A price value is required")]
        [Range(0, 999999.99, ErrorMessage = "Price must be a positive value")]
        public decimal Value { get; set; }
    }
}
