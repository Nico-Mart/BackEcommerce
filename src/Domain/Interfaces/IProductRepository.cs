using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        IQueryable<Product> GetAllWithPrices();
    }
}
