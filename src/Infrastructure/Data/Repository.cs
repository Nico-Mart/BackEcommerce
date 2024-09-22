using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly NirvanaContext _context;
        public Repository(NirvanaContext context)
        {
            _context = context;
        }
        public virtual async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            return (await _context.Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false)).Entity;
        }
        public virtual async Task CreateRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        }
        public virtual async Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        public virtual async Task<T?> GetByIdAsync<Tid>(Tid id, CancellationToken cancellationToken = default) where Tid : notnull
        {
            return await _context.Set<T>().FindAsync(id, cancellationToken).ConfigureAwait(false);
        }
        //public virtual async Task<ICollection<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        //{
        //    return await _context.Set<T>().Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
        //}
        public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }
        public virtual Task UpdateRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().UpdateRange(entities);
            return Task.CompletedTask;
        }
        public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }
        public virtual Task DeleteRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().RemoveRange(entities);
            return Task.CompletedTask;
        }
        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
