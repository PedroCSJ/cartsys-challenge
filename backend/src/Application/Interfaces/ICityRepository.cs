using Cartsys.Application.DTOs.Cities;
using Cartsys.Application.Common;
using Cartsys.Domain.Entities;
using Cartsys.Domain.Interfaces;

namespace Cartsys.Application.Interfaces;

public interface ICityRepository : IRepository<City>
{
    Task<(IEnumerable<City> Items, int Total)> GetPagedWithStateAsync(int page, int pageSize, CityFilterRequest filter);
    Task<IEnumerable<City>> GetByStateIdAsync(Guid stateId);
}
