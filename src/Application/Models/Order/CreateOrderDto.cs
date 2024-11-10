using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Application.Models.Address;

namespace Application.Models.Order
{
    public class CreateOrderDto
    {
        [JsonIgnore]
        public int IdUser { get; set; }
        [Required(ErrorMessage = "Products must be specified")]
        [MinLength(1, ErrorMessage = "OrderLines must contain at least 1 productVariant")]
        public ICollection<OrderLineDto> OrderLines { get; set; } = new List<OrderLineDto>();
        [Required(ErrorMessage = "Address must be specified")]
        public CreateAddressDto Address { get; set; }
    }
}