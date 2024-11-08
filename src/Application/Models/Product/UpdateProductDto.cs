using Application.Models.ProductVariant;
using System.ComponentModel.DataAnnotations;

namespace Application.Models.Product
{
    public class UpdateProductDto : CreateProductDto
    {
        [Required(ErrorMessage = "The product id must be specified")]
        [Key]
        public int Id { get; set; }
        public new ICollection<UpdateProductVariantDto>? ProductVariants { get; set; } = new List<UpdateProductVariantDto>();
    }
}
