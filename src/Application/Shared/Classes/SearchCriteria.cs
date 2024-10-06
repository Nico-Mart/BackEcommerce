using Application.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Shared.Classes
{
    public class SearchCriteria
    {
        [Required]
        public string Field { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public FilterOperator Operator { get; set; }
    }
}