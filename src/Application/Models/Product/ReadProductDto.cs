using Application.Models.ProductVariant;

namespace Application.Models.Product
{
    public class ReadProductDto
    {
        public int Id { get; set; }
        public int IdCategory { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal Price { get; set; }
        public ICollection<ReadProductVariantDto> ProductVariants { get; set; } = new List<ReadProductVariantDto>();
    }
}
