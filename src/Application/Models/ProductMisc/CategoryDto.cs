namespace Application.Models.ProductMisc
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? IdParent { get; set; }
    }
}