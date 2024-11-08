using System.ComponentModel.DataAnnotations;

namespace Application.Models.Order
{
    public class UpdateOrderDto : CreateOrderDto
    {
        [Required(ErrorMessage = "The product id must be specified")]
        [Key]
        public int Id { get; set; }
    }
}