namespace Application.Models.ProductVariant
{
    public class ProductVariantDto
    {
        public int IdProduct { get; set; }
        public int Stock { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
    }
}