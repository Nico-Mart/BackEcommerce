using System.ComponentModel.DataAnnotations;

namespace Application.Shared.Classes
{
    public class Paginator
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Page { get; set; } = 1;
        [Required]
        [Range(0, int.MaxValue)]
        public int PageSize { get; set; } = 10;
    }
}