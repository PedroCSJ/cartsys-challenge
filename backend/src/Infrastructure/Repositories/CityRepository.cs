using Cartsys.Application.DTOs.Cities;
using Cartsys.Application.Interfaces;
using Cartsys.Domain.Entities;
using Cartsys.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cartsys.Infrastructure.Repositories;

public class CityRepository(AppDbContext context) : GenericRepository<City>(context), ICityRepository
{
    public async Task<(IEnumerable<City> Items, int Total)> GetPagedWithStateAsync(
        int page, int pageSize, CityFilterRequest filter)
    {
        var query = _dbSet
            .Include(c => c.State)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Name))
            query = query.Where(c => c.Name.Contains(filter.Name));

        if (filter.StateId.HasValue)
            query = query.Where(c => c.StateId == filter.StateId.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IEnumerable<City>> GetByStateIdAsync(Guid stateId)
        => await _dbSet
            .Include(c => c.State)
            .Where(c => c.StateId == stateId)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public override async Task<City?> GetByIdAsync(Guid id)
        => await _dbSet
            .Include(c => c.State)
            .FirstOrDefaultAsync(c => c.Id == id);
}
