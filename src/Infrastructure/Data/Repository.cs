using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly NirvanaContext _context;
        protected Repository(NirvanaContext context)
        {
            _context = context;
        }
        public virtual async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            var createEntry = await _context.Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await SaveChangesAsync(cancellationToken); 
            return createEntry.Entity; 
        }
        public virtual async Task CreateRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
            await SaveChangesAsync(cancellationToken);
        }
        public virtual IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }
        public virtual async Task<ICollection<T>> ToListAsync(IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            return await query.ToListAsync();
        }
        public virtual async Task<T?> GetByIdAsync<Tid>(Tid id, CancellationToken cancellationToken = default) where Tid : notnull
        {
            return await _context.Set<T>().FindAsync(id, cancellationToken).ConfigureAwait(false);
        }
        //public virtual async Task<ICollection<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        //{
        //    return await _context.Set<T>().Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
        //}
        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Update(entity);
            await SaveChangesAsync(cancellationToken);
        }
        public virtual async Task<int> UpdateRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().UpdateRange(entities);
            return await SaveChangesAsync(cancellationToken);
        }
        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Remove(entity);
            await SaveChangesAsync(cancellationToken);
        }
        public virtual async Task<int> DeleteRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().RemoveRange(entities);
            return await SaveChangesAsync(cancellationToken);
        }
        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
