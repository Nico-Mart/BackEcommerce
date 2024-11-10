using Application.Models.Address;

namespace Application.Models.Order
{
    public class ReadOrderDto
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public ICollection<ReadOrderLineDto> OrderLines { get; set; } = new List<ReadOrderLineDto>();
        public ReadAddressDto Address { get; set; }
    }
}