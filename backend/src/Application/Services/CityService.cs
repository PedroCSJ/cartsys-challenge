using Cartsys.Application.Common;
using Cartsys.Application.DTOs.Cities;
using Cartsys.Application.Interfaces;
using Cartsys.Domain.Entities;
using Cartsys.Domain.Interfaces;

namespace Cartsys.Application.Services;

public class CityService(ICityRepository cityRepository, IRepository<State> stateRepository) : ICityService
{
    public async Task<Result<PagedResult<CityDto>>> GetAllAsync(CityFilterRequest filter)
    {
        var (items, total) = await cityRepository.GetPagedWithStateAsync(filter.Page, filter.PageSize, filter);
        var dtos = items.Select(ToDto);
        return Result<PagedResult<CityDto>>.Ok(PagedResult<CityDto>.Create(dtos, total, filter.Page, filter.PageSize));
    }

    public async Task<Result<IEnumerable<CityDto>>> GetByStateAsync(Guid stateId)
    {
        var cities = await cityRepository.GetByStateIdAsync(stateId);
        return Result<IEnumerable<CityDto>>.Ok(cities.Select(ToDto));
    }

    public async Task<Result<CityDto>> GetByIdAsync(Guid id)
    {
        var city = await cityRepository.GetByIdAsync(id);
        if (city is null)
            return Result<CityDto>.NotFound("Cidade não encontrada.");

        return Result<CityDto>.Ok(ToDto(city));
    }

    public async Task<Result<CityDto>> CreateAsync(CreateCityRequest request)
    {
        var stateExists = await stateRepository.ExistsAsync(s => s.Id == request.StateId);
        if (!stateExists)
            return Result<CityDto>.NotFound("Estado não encontrado.");

        var cityExists = await cityRepository.ExistsAsync(
            c => c.Name.ToLower() == request.Name.ToLower() && c.StateId == request.StateId);
        if (cityExists)
            return Result<CityDto>.Conflict("Já existe uma cidade com este nome neste estado.");

        var city = new City { Name = request.Name, StateId = request.StateId };

        await cityRepository.AddAsync(city);
        await cityRepository.SaveChangesAsync();

        var created = await cityRepository.GetByIdAsync(city.Id);
        return Result<CityDto>.Created(ToDto(created!));
    }

    public async Task<Result<CityDto>> UpdateAsync(Guid id, UpdateCityRequest request)
    {
        var city = await cityRepository.GetByIdAsync(id);
        if (city is null)
            return Result<CityDto>.NotFound("Cidade não encontrada.");

        var stateExists = await stateRepository.ExistsAsync(s => s.Id == request.StateId);
        if (!stateExists)
            return Result<CityDto>.NotFound("Estado não encontrado.");

        var nameExists = await cityRepository.ExistsAsync(
            c => c.Name.ToLower() == request.Name.ToLower() && c.StateId == request.StateId && c.Id != id);
        if (nameExists)
            return Result<CityDto>.Conflict("Já existe uma cidade com este nome neste estado.");

        city.Name = request.Name;
        city.StateId = request.StateId;
        city.UpdatedAt = DateTime.UtcNow;

        cityRepository.Update(city);
        await cityRepository.SaveChangesAsync();

        var updated = await cityRepository.GetByIdAsync(id);
        return Result<CityDto>.Ok(ToDto(updated!));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var city = await cityRepository.GetByIdAsync(id);
        if (city is null)
            return Result.NotFound("Cidade não encontrada.");

        cityRepository.SoftDelete(city);
        await cityRepository.SaveChangesAsync();

        return Result.Ok();
    }

    private static CityDto ToDto(City c)
        => new(c.Id, c.Name, c.StateId, c.State?.Name ?? "", c.State?.UF ?? "", c.CreatedAt);
}
