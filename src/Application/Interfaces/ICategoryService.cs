using Application.Models.ProductMisc;

namespace Application.Interfaces
{
    public interface ICategoryService : IService<CreateCategoryDto, CategoryDto, CategoryDto>
    {
    }
}
