using System.ComponentModel.DataAnnotations;

namespace Application.Shared.Classes
{
    public class Sorter
    {
        public string SortBy { get; set; } = "id";
        public bool IsDescending { get; set; } = false;
    }
}