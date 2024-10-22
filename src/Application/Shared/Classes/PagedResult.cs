namespace Application.Shared.Classes
{
    public class PagedResult<T>(ICollection<T> data, int pageCount)
    {
        public ICollection<T> Data { get; set; } = data;
        public int PageCount { get; set; } = pageCount;
    }
}
