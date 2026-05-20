using Cartsys.Application.Common;
using Cartsys.Application.DTOs.Languages;
using Cartsys.Application.Interfaces;
using Cartsys.Domain.Entities;
using Cartsys.Domain.Interfaces;

namespace Cartsys.Application.Services;

public class LanguageService(IRepository<ProgrammingLanguage> repository) : ILanguageService
{
    public async Task<Result<PagedResult<LanguageDto>>> GetAllAsync(LanguageFilterRequest filter)
    {
        var (items, total) = await repository.GetPagedAsync(
            filter.Page,
            filter.PageSize,
            l => (string.IsNullOrEmpty(filter.Name) || l.Name.Contains(filter.Name)) &&
                 (filter.Type == null || l.Type == filter.Type)
        );

        var dtos = items.Select(ToDto);
        return Result<PagedResult<LanguageDto>>.Ok(PagedResult<LanguageDto>.Create(dtos, total, filter.Page, filter.PageSize));
    }

    public async Task<Result<IEnumerable<LanguageDto>>> GetAllSimpleAsync()
    {
        var langs = await repository.GetAllAsync();
        return Result<IEnumerable<LanguageDto>>.Ok(langs.Select(ToDto));
    }

    public async Task<Result<LanguageDto>> GetByIdAsync(Guid id)
    {
        var lang = await repository.GetByIdAsync(id);
        if (lang is null)
            return Result<LanguageDto>.NotFound("Linguagem não encontrada.");

        return Result<LanguageDto>.Ok(ToDto(lang));
    }

    public async Task<Result<LanguageDto>> CreateAsync(CreateLanguageRequest request)
    {
        var exists = await repository.ExistsAsync(l => l.Name.ToLower() == request.Name.ToLower());
        if (exists)
            return Result<LanguageDto>.Conflict("Já existe uma linguagem com este nome.");

        var lang = new ProgrammingLanguage
        {
            Name = request.Name,
            Type = request.Type
        };

        await repository.AddAsync(lang);
        await repository.SaveChangesAsync();

        return Result<LanguageDto>.Created(ToDto(lang));
    }

    public async Task<Result<LanguageDto>> UpdateAsync(Guid id, UpdateLanguageRequest request)
    {
        var lang = await repository.GetByIdAsync(id);
        if (lang is null)
            return Result<LanguageDto>.NotFound("Linguagem não encontrada.");

        var nameExists = await repository.ExistsAsync(l => l.Name.ToLower() == request.Name.ToLower() && l.Id != id);
        if (nameExists)
            return Result<LanguageDto>.Conflict("Já existe uma linguagem com este nome.");

        lang.Name = request.Name;
        lang.Type = request.Type;
        lang.UpdatedAt = DateTime.UtcNow;

        repository.Update(lang);
        await repository.SaveChangesAsync();

        return Result<LanguageDto>.Ok(ToDto(lang));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var lang = await repository.GetByIdAsync(id);
        if (lang is null)
            return Result.NotFound("Linguagem não encontrada.");

        repository.SoftDelete(lang);
        await repository.SaveChangesAsync();

        return Result.Ok();
    }

    private static LanguageDto ToDto(ProgrammingLanguage l)
        => new(l.Id, l.Name, l.Type, l.Type.ToString(), l.CreatedAt);
}
