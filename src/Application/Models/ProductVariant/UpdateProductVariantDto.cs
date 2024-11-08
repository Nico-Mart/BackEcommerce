using System.ComponentModel.DataAnnotations;

namespace Application.Models.ProductVariant
{
    public class UpdateProductVariantDto : CreateProductVariantDto
    {
        [Required(ErrorMessage = "The productVariant id must be specified")]
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "The product id must be specified")]
        public int IdProduct { get; set; }
    }
}