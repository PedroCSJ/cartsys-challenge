using Cartsys.Application.Common;
using Cartsys.Application.DTOs.States;

namespace Cartsys.Application.Interfaces;

public interface IStateService
{
    Task<Result<PagedResult<StateDto>>> GetAllAsync(StateFilterRequest filter);
    Task<Result<IEnumerable<StateDto>>> GetAllSimpleAsync();
    Task<Result<StateDto>> GetByIdAsync(Guid id);
    Task<Result<StateDto>> CreateAsync(CreateStateRequest request);
    Task<Result<StateDto>> UpdateAsync(Guid id, UpdateStateRequest request);
    Task<Result> DeleteAsync(Guid id);
}
