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
            try
            {
                var createEntry = await _context.Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
                await SaveChangesAsync(cancellationToken);
                return createEntry.Entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating entity: {ex.Message}", ex);
            }
        }
        public virtual async Task CreateRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Set<T>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating entity range: {ex.Message}", ex);
            }
        }
        public virtual IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }
        public virtual async Task<ICollection<T>> ToListAsync(IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            try
            {
                return await query.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving entities list: {ex.Message}", ex);
            }
        }
        public virtual async Task<T?> GetByIdAsync<Tid>(Tid id, CancellationToken cancellationToken = default) where Tid : notnull
        {
            try
            {
                return await _context.Set<T>().FindAsync(id, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving entity by ID: {ex.Message}", ex);
            }
        }
        /*public virtual async Task<ICollection<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Set<T>().Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving entity by expression: ", ex);
            }
        }*/
        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Set<T>().Update(entity);
                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating entity: {ex.Message}", ex);
            }
        }
        public virtual async Task<int> UpdateRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Set<T>().UpdateRange(entities);
                return await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating entity range: {ex.Message}", ex);
            }
        }
        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            try 
            {
                _context.Set<T>().Remove(entity);
                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting entity: {ex.Message}", ex);
            }
        }
        public virtual async Task<int> DeleteRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Set<T>().RemoveRange(entities);
                return await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting entity range: {ex.Message}", ex);
            }
        }
        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving changes: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }
    }
}
