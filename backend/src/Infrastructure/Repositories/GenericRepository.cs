using System.Linq.Expressions;
using Cartsys.Domain.Entities;
using Cartsys.Domain.Interfaces;
using Cartsys.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cartsys.Infrastructure.Repositories;

public class GenericRepository<T>(AppDbContext context) : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public virtual async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id);

    public virtual async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public virtual async Task<(IEnumerable<T> Items, int Total)> GetPagedAsync(
        int page, int pageSize, Expression<Func<T, bool>>? filter = null)
    {
        var query = _dbSet.AsQueryable();

        if (filter != null)
            query = query.Where(filter);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);

    public void Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void SoftDelete(T entity)
    {
        entity.DeletedAt = DateTime.UtcNow;
        Update(entity);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.AnyAsync(predicate);

    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.FirstOrDefaultAsync(predicate);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
