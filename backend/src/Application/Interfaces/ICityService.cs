using Cartsys.Application.Common;
using Cartsys.Application.DTOs.Cities;

namespace Cartsys.Application.Interfaces;

public interface ICityService
{
    Task<Result<PagedResult<CityDto>>> GetAllAsync(CityFilterRequest filter);
    Task<Result<IEnumerable<CityDto>>> GetByStateAsync(Guid stateId);
    Task<Result<CityDto>> GetByIdAsync(Guid id);
    Task<Result<CityDto>> CreateAsync(CreateCityRequest request);
    Task<Result<CityDto>> UpdateAsync(Guid id, UpdateCityRequest request);
    Task<Result> DeleteAsync(Guid id);
}
