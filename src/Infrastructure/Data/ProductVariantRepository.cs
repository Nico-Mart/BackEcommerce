using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Data
{
    public class ProductVariantRepository : Repository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(NirvanaContext context) : base(context)
        {
        }
    }
}
