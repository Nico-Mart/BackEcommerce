using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Data
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository 
    {
        public CategoryRepository(NirvanaContext context) : base(context) 
        {   
        }
    }
}
