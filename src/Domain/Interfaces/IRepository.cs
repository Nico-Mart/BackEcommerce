
namespace Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
        Task CreateRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default);
        IQueryable<T> GetAll();
        Task<ICollection<T>> ToListAsync(IQueryable<T> query, CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync<Tid>(Tid id, CancellationToken cancellationToken = default) where Tid : notnull;
        //Task<ICollection<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
