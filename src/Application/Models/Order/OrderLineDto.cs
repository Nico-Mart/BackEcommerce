using System.ComponentModel.DataAnnotations;

namespace Application.Models.Order
{
    public class OrderLineDto
    {
        [Required(ErrorMessage = "IdProductVariant must be specified")]
        [Range(0, int.MaxValue, ErrorMessage = "IdProductVariant must be a positive value")]
        public int IdProductVariant { get; set; }
        [Required(ErrorMessage = "Amount must be specified")]
        [Range(0, int.MaxValue, ErrorMessage = "Amount must be a positive value")]
        public int Amount { get; set; }
    }
}