using Application.Shared.Enums;
using System.Text.RegularExpressions;

namespace Application.Shared.Classes
{
    public class Options
    {
        public FilterGroup FilterGroup { get; set; }
        public Sorter Sorter { get; set; }
        public Paginator Paginator { get; set; }

        public Options(string? filterString, Sorter? sorter, Paginator? paginator)
        {
            FilterGroup = ParseFilterGroup(filterString);
            Sorter = sorter ?? new Sorter();
            Paginator = paginator ?? new Paginator();
        }

        private FilterGroup ParseFilterGroup(string? filterString)
        {
            if (string.IsNullOrEmpty(filterString))
                return new FilterGroup { LogicalOperator = LogicalOperator.And };

            var rootGroup = new FilterGroup { LogicalOperator = LogicalOperator.And };
            var currentGroup = rootGroup;
            var stack = new Stack<FilterGroup>();

            var criteriaArray = Regex.Split(filterString, @"(\&\&|\|\|)"); // Split by logical operators but keep them
            foreach (var criterion in criteriaArray)
            {
                var trimmedCriterion = criterion.Trim();

                if (trimmedCriterion == "&&" || trimmedCriterion == "||")
                {
                    currentGroup.LogicalOperator = trimmedCriterion == "&&" ? LogicalOperator.And : LogicalOperator.Or;
                    continue;
                }

                if (trimmedCriterion.StartsWith("("))
                {
                    var newGroup = new FilterGroup { LogicalOperator = LogicalOperator.And };
                    currentGroup.ChildGroups.Add(newGroup);
                    stack.Push(currentGroup);
                    currentGroup = newGroup;
                    trimmedCriterion = trimmedCriterion.Substring(1).Trim();
                }

                if (trimmedCriterion.EndsWith(")"))
                {
                    trimmedCriterion = trimmedCriterion.Substring(0, trimmedCriterion.Length - 1).Trim();
                    currentGroup.Filters.Add(ParseFilter(trimmedCriterion));
                    currentGroup = stack.Pop();
                }
                else
                {
                    currentGroup.Filters.Add(ParseFilter(trimmedCriterion));
                }
            }

            return rootGroup;
        }

        private Filter ParseFilter(string criterion)
        {
            var parts = criterion.Split(':');
            if (parts.Length != 3)
                throw new FormatException("Filter format is incorrect");

            return new Filter
            {
                Field = parts[0],
                Value = parts[1],
                Operator = (FilterOperator)Enum.Parse(typeof(FilterOperator), parts[2])
            };
        }
    }
}
