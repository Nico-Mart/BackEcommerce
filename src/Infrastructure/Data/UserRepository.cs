using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class UserRepository : Repository<User> , IUserRepository
    {
        public UserRepository(NirvanaContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
        }


    }

}
