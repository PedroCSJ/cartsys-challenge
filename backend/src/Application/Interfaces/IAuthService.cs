using Cartsys.Application.Common;
using Cartsys.Application.DTOs.Auth;

namespace Cartsys.Application.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
}
