using Cartsys.Application.DTOs.Developers;
using Cartsys.Domain.Entities;
using Cartsys.Domain.Interfaces;

namespace Cartsys.Application.Interfaces;

public interface IDeveloperRepository : IRepository<Developer>
{
    Task<Developer?> GetByIdWithRelationsAsync(Guid id);
    Task<(IEnumerable<Developer> Items, int Total)> GetPagedWithRelationsAsync(int page, int pageSize, DeveloperFilterRequest filter);
    Task<IEnumerable<Developer>> GetAllWithRelationsAsync();
}
