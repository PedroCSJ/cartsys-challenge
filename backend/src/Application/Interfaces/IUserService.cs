using Cartsys.Application.Common;
using Cartsys.Application.DTOs.Users;

namespace Cartsys.Application.Interfaces;

public interface IUserService
{
    Task<Result<PagedResult<UserDto>>> GetAllAsync(UserFilterRequest filter);
    Task<Result<UserDto>> GetByIdAsync(Guid id);
    Task<Result<UserDto>> CreateAsync(CreateUserRequest request);
    Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<Result> DeleteAsync(Guid id);
}
