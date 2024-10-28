using System.ComponentModel.DataAnnotations;

namespace Application.Shared.Classes
{
    public class Paginator
    {
        [Range(0, int.MaxValue)]
        public int Page { get; set; } = 1;
        [Range(0, int.MaxValue)]
        public int PageSize { get; set; } = 10;
    }
}