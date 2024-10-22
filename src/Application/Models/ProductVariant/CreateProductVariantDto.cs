using System.ComponentModel.DataAnnotations;

namespace Application.Models.ProductVariant
{
    public class CreateProductVariantDto
    {
        [Required(ErrorMessage = "A ProductVariant Stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a positive value")]
        public int Stock { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
    }
}