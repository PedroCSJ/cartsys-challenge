using Cartsys.Application.Common;
using Cartsys.Application.DTOs.Developers;

namespace Cartsys.Application.Interfaces;

public interface IDeveloperService
{
    Task<Result<PagedResult<DeveloperDto>>> GetAllAsync(DeveloperFilterRequest filter);
    Task<Result<DeveloperDto>> GetByIdAsync(Guid id);
    Task<Result<DeveloperDto>> CreateAsync(CreateDeveloperRequest request);
    Task<Result<DeveloperDto>> UpdateAsync(Guid id, UpdateDeveloperRequest request);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<IEnumerable<DeveloperDto>>> GetAllForReportAsync();
}
