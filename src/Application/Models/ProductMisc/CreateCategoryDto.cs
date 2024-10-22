using System.ComponentModel.DataAnnotations;

namespace Application.Models.ProductMisc
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "A Category name is required")]
        [StringLength(32)]
        public string Name { get; set; }
        public int? IdParent { get; set; }
    }
}
