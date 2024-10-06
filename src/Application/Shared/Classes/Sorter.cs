using System.ComponentModel.DataAnnotations;

namespace Application.Shared.Classes
{
    public class Sorter
    {
        [Required]
        public string SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
    }
}