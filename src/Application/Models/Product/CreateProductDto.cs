using Application.Models.ProductMisc;
using Application.Models.ProductVariant;
using System.ComponentModel.DataAnnotations;

namespace Application.Models.Product
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "The product category must be specified")]
        [Range(0, int.MaxValue, ErrorMessage = "IdCategory must be a positive value")]
        public int IdCategory { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(64, ErrorMessage = "Name cannot be longer than 64 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [StringLength(255, ErrorMessage = "Description cannot be longer than 255 characters")]
        public string Description { get; set; }
        [StringLength(512, ErrorMessage = "ImageUrl cannot be longer than 512 characters")]
        public string? ImageUrl { get; set; }
        [Required]
        public CreatePriceDto Price { get; set; }
        public ICollection<CreateProductVariantDto>? ProductVariants { get; set; } = new List<CreateProductVariantDto>();
    }
}
