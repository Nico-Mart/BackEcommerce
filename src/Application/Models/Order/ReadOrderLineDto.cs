namespace Application.Models.Order
{
    public class ReadOrderLineDto
    {
        public OrderLineProductVariant ProductVariant { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
    }
    public class OrderLineProductVariant
    {
        public int Id { get; set; }
        public int IdProduct { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
    }
}