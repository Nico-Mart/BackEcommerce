using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Data
{
    public class OrderLineRepository : Repository<OrderLine>, IOrderLineRepository 
    {
        public OrderLineRepository(NirvanaContext context) : base(context) 
        {   
        }
    }
}
