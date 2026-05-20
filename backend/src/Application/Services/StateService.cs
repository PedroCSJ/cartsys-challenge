using Cartsys.Application.Common;
using Cartsys.Application.DTOs.States;
using Cartsys.Application.Interfaces;
using Cartsys.Domain.Entities;
using Cartsys.Domain.Interfaces;

namespace Cartsys.Application.Services;

public class StateService(IRepository<State> repository) : IStateService
{
    public async Task<Result<PagedResult<StateDto>>> GetAllAsync(StateFilterRequest filter)
    {
        var (items, total) = await repository.GetPagedAsync(
            filter.Page,
            filter.PageSize,
            s => (string.IsNullOrEmpty(filter.Name) || s.Name.Contains(filter.Name)) &&
                 (string.IsNullOrEmpty(filter.UF) || s.UF.Contains(filter.UF))
        );

        var dtos = items.Select(ToDto);
        return Result<PagedResult<StateDto>>.Ok(PagedResult<StateDto>.Create(dtos, total, filter.Page, filter.PageSize));
    }

    public async Task<Result<IEnumerable<StateDto>>> GetAllSimpleAsync()
    {
        var states = await repository.GetAllAsync();
        return Result<IEnumerable<StateDto>>.Ok(states.Select(ToDto));
    }

    public async Task<Result<StateDto>> GetByIdAsync(Guid id)
    {
        var state = await repository.GetByIdAsync(id);
        if (state is null)
            return Result<StateDto>.NotFound("Estado não encontrado.");

        return Result<StateDto>.Ok(ToDto(state));
    }

    public async Task<Result<StateDto>> CreateAsync(CreateStateRequest request)
    {
        var exists = await repository.ExistsAsync(s => s.UF == request.UF.ToUpper());
        if (exists)
            return Result<StateDto>.Conflict($"Já existe um estado com a UF '{request.UF}'.");

        var state = new State
        {
            Name = request.Name,
            UF = request.UF.ToUpper()
        };

        await repository.AddAsync(state);
        await repository.SaveChangesAsync();

        return Result<StateDto>.Created(ToDto(state));
    }

    public async Task<Result<StateDto>> UpdateAsync(Guid id, UpdateStateRequest request)
    {
        var state = await repository.GetByIdAsync(id);
        if (state is null)
            return Result<StateDto>.NotFound("Estado não encontrado.");

        var ufExists = await repository.ExistsAsync(s => s.UF == request.UF.ToUpper() && s.Id != id);
        if (ufExists)
            return Result<StateDto>.Conflict($"Já existe um estado com a UF '{request.UF}'.");

        state.Name = request.Name;
        state.UF = request.UF.ToUpper();
        state.UpdatedAt = DateTime.UtcNow;

        repository.Update(state);
        await repository.SaveChangesAsync();

        return Result<StateDto>.Ok(ToDto(state));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var state = await repository.GetByIdAsync(id);
        if (state is null)
            return Result.NotFound("Estado não encontrado.");

        repository.SoftDelete(state);
        await repository.SaveChangesAsync();

        return Result.Ok();
    }

    private static StateDto ToDto(State s)
        => new(s.Id, s.Name, s.UF, s.CreatedAt);
}
