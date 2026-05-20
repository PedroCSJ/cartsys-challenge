using Cartsys.Application.Common;
using Cartsys.Application.DTOs.Auth;
using Cartsys.Application.Interfaces;
using Cartsys.Domain.Entities;
using Cartsys.Domain.Interfaces;

namespace Cartsys.Application.Services;

public class AuthService(IRepository<User> userRepository, ITokenService tokenService, IPasswordHasher passwordHasher) : IAuthService
{
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.FindAsync(u => u.Email == request.Email);
        if (user is null)
            return Result<LoginResponse>.Fail("E-mail ou senha inválidos.", 401);

        var passwordValid = passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!passwordValid)
            return Result<LoginResponse>.Fail("E-mail ou senha inválidos.", 401);

        var expiresAt = DateTime.UtcNow.AddHours(8);
        var token = tokenService.GenerateToken(user);

        return Result<LoginResponse>.Ok(new LoginResponse(token, user.Name, user.Email, expiresAt));
    }
}
