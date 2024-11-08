using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class OrderRepository : Repository<Order>, IOrderRepository 
    {
        public OrderRepository(NirvanaContext context) : base(context) 
        {   
        }
        public override IQueryable<Order> GetAll()
        {
            return _context.Orders.Include(o => o.OrderLines).ThenInclude(ol => ol.IdProductVariantNavigation);
        }
    }
}
