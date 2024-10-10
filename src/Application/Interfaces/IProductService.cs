using Application.Models.Product;

namespace Application.Interfaces
{
    public interface IProductService : IService<CreateProductDto, ReadProductDto, UpdateProductDto>
    {
    }
}
