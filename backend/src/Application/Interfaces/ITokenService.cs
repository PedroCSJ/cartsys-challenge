using Cartsys.Domain.Entities;

namespace Cartsys.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
