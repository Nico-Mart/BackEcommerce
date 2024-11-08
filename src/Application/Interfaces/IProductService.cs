using Application.Models.Product;
using Application.Models.ProductVariant;

namespace Application.Interfaces
{
    public interface IProductService : IService<CreateProductDto, ReadProductDto, UpdateProductDto>
    {
    }
    public interface IProductVariantService : IService<CreateProductVariantDto, ReadProductVariantDto, UpdateProductVariantDto>
    {
        Task<ICollection<ReadProductVariantDto>> GetAll(int id);
    }
}
