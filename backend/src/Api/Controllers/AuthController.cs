using Cartsys.Api.Extensions;
using Cartsys.Application.DTOs.Auth;
using Cartsys.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cartsys.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        if (!result.IsSuccess)
            return Unauthorized(new { message = result.ErrorMessage });

        return Ok(result.Value);
    }
}
