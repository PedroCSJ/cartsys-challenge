using Cartsys.Application.Common;
using Cartsys.Application.DTOs.Users;
using Cartsys.Application.Interfaces;
using Cartsys.Domain.Entities;
using Cartsys.Domain.Interfaces;

namespace Cartsys.Application.Services;

public class UserService(IRepository<User> repository, IPasswordHasher passwordHasher) : IUserService
{
    public async Task<Result<PagedResult<UserDto>>> GetAllAsync(UserFilterRequest filter)
    {
        var (items, total) = await repository.GetPagedAsync(
            filter.Page,
            filter.PageSize,
            u => (string.IsNullOrEmpty(filter.Name) || u.Name.Contains(filter.Name)) &&
                 (string.IsNullOrEmpty(filter.Email) || u.Email.Contains(filter.Email))
        );

        var dtos = items.Select(ToDto);
        return Result<PagedResult<UserDto>>.Ok(PagedResult<UserDto>.Create(dtos, total, filter.Page, filter.PageSize));
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null)
            return Result<UserDto>.NotFound("Usuário não encontrado.");

        return Result<UserDto>.Ok(ToDto(user));
    }

    public async Task<Result<UserDto>> CreateAsync(CreateUserRequest request)
    {
        var emailExists = await repository.ExistsAsync(u => u.Email == request.Email);
        if (emailExists)
            return Result<UserDto>.Conflict("Já existe um usuário com este e-mail.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password)
        };

        await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        return Result<UserDto>.Created(ToDto(user));
    }

    public async Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null)
            return Result<UserDto>.NotFound("Usuário não encontrado.");

        var emailExists = await repository.ExistsAsync(u => u.Email == request.Email && u.Id != id);
        if (emailExists)
            return Result<UserDto>.Conflict("Já existe um usuário com este e-mail.");

        user.Name = request.Name;
        user.Email = request.Email;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(request.Password))
            user.PasswordHash = passwordHasher.Hash(request.Password);

        repository.Update(user);
        await repository.SaveChangesAsync();

        return Result<UserDto>.Ok(ToDto(user));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null)
            return Result.NotFound("Usuário não encontrado.");

        repository.SoftDelete(user);
        await repository.SaveChangesAsync();

        return Result.Ok();
    }

    private static UserDto ToDto(User u)
        => new(u.Id, u.Name, u.Email, u.CreatedAt);
}
