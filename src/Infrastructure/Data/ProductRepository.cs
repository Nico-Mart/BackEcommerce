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
    public async override Task<Product?> GetByIdAsync<Tid>(Tid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Products.Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id.Equals(id), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving entity by ID: {ex.Message}", ex);
        }
    }
    public IQueryable<Product> GetAllWithPrices()
    {
        return _context.Products.Include(p => p.ProductVariants).Include(p => p.Prices);
    }
}
