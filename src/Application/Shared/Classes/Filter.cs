using Application.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Shared.Classes
{
    public class FilterGroup
    {
        public List<Filter> Filters { get; set; } = new List<Filter>();
        public List<FilterGroup> ChildGroups { get; set; } = new List<FilterGroup>();
        public LogicalOperator LogicalOperator { get; set; }
    }

    public class Filter
    {
        [Required]
        public string Field { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public FilterOperator Operator { get; set; }
    }

    public enum LogicalOperator
    {
        And,
        Or
    }
}