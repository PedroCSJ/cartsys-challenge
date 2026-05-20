using Cartsys.Application.DTOs.Developers;
using Cartsys.Application.Interfaces;
using Cartsys.Domain.Entities;
using Cartsys.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cartsys.Infrastructure.Repositories;

public class DeveloperRepository(AppDbContext context) : GenericRepository<Developer>(context), IDeveloperRepository
{
    public async Task<Developer?> GetByIdWithRelationsAsync(Guid id)
        => await _dbSet
            .Include(d => d.City).ThenInclude(c => c.State)
            .Include(d => d.Languages)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<(IEnumerable<Developer> Items, int Total)> GetPagedWithRelationsAsync(
        int page, int pageSize, DeveloperFilterRequest filter)
    {
        var query = _dbSet
            .Include(d => d.City).ThenInclude(c => c.State)
            .Include(d => d.Languages)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Name))
            query = query.Where(d => d.Name.Contains(filter.Name));

        if (!string.IsNullOrEmpty(filter.Email))
            query = query.Where(d => d.Email.Contains(filter.Email));

        if (filter.Seniority.HasValue)
            query = query.Where(d => d.Seniority == filter.Seniority.Value);

        if (filter.CityId.HasValue)
            query = query.Where(d => d.CityId == filter.CityId.Value);

        if (filter.LanguageId.HasValue)
            query = query.Where(d => d.Languages.Any(l => l.Id == filter.LanguageId.Value));

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(d => d.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IEnumerable<Developer>> GetAllWithRelationsAsync()
        => await _dbSet
            .Include(d => d.City).ThenInclude(c => c.State)
            .Include(d => d.Languages)
            .OrderBy(d => d.Name)
            .ToListAsync();
}
