namespace Cartsys.Application.DTOs.Users;

public record UserDto(Guid Id, string Name, string Email, DateTime CreatedAt);

public record CreateUserRequest(string Name, string Email, string Password);

public record UpdateUserRequest(string Name, string Email, string? Password);

public record UserFilterRequest(string? Name, string? Email, int Page = 1, int PageSize = 10);
