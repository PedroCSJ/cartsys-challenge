using System.Linq.Expressions;
using Cartsys.Domain.Entities;

namespace Cartsys.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<(IEnumerable<T> Items, int Total)> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>>? filter = null);
    Task AddAsync(T entity);
    void Update(T entity);
    void SoftDelete(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
    Task SaveChangesAsync();
}
