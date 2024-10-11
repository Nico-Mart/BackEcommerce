using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(NirvanaContext context) : base(context)
    {
    }
    public override IQueryable<Product> GetAll()
    {
        return _context.Products.Include(p => p.ProductVariants);
    }
    public IQueryable<Product> GetAllWithPrices()
    {
        return _context.Products.Include(p => p.ProductVariants).Include(p => p.Prices);
    }
}
