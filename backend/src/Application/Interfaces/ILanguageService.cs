using Cartsys.Application.Common;
using Cartsys.Application.DTOs.Languages;

namespace Cartsys.Application.Interfaces;

public interface ILanguageService
{
    Task<Result<PagedResult<LanguageDto>>> GetAllAsync(LanguageFilterRequest filter);
    Task<Result<IEnumerable<LanguageDto>>> GetAllSimpleAsync();
    Task<Result<LanguageDto>> GetByIdAsync(Guid id);
    Task<Result<LanguageDto>> CreateAsync(CreateLanguageRequest request);
    Task<Result<LanguageDto>> UpdateAsync(Guid id, UpdateLanguageRequest request);
    Task<Result> DeleteAsync(Guid id);
}
