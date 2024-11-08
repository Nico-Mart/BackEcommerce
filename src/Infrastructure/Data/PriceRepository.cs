using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class PriceRepository : Repository<Price>, IPriceRepository
    {
        public PriceRepository(NirvanaContext context) : base(context)
        {
        }
        public override async Task<Price?> GetByIdAsync<Tid>(Tid id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Prices
                    .Where(p => p.IdProduct == Convert.ToInt32(id))
                    .OrderByDescending(p => p.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving latest price by product ID: ", ex);
            }
        }
    }
}
