using Cartsys.Application.Common;
using Cartsys.Application.DTOs.Developers;
using Cartsys.Application.DTOs.Languages;
using Cartsys.Application.Interfaces;
using Cartsys.Domain.Entities;
using Cartsys.Domain.Interfaces;

namespace Cartsys.Application.Services;

public class DeveloperService(
    IDeveloperRepository developerRepository,
    IRepository<ProgrammingLanguage> languageRepository,
    IRepository<City> cityRepository) : IDeveloperService
{
    public async Task<Result<PagedResult<DeveloperDto>>> GetAllAsync(DeveloperFilterRequest filter)
    {
        var (items, total) = await developerRepository.GetPagedWithRelationsAsync(filter.Page, filter.PageSize, filter);
        var dtos = items.Select(ToDto);
        return Result<PagedResult<DeveloperDto>>.Ok(PagedResult<DeveloperDto>.Create(dtos, total, filter.Page, filter.PageSize));
    }

    public async Task<Result<DeveloperDto>> GetByIdAsync(Guid id)
    {
        var dev = await developerRepository.GetByIdWithRelationsAsync(id);
        if (dev is null)
            return Result<DeveloperDto>.NotFound("Desenvolvedor não encontrado.");

        return Result<DeveloperDto>.Ok(ToDto(dev));
    }

    public async Task<Result<DeveloperDto>> CreateAsync(CreateDeveloperRequest request)
    {
        if (!request.LanguageIds.Any())
            return Result<DeveloperDto>.Fail("O desenvolvedor deve possuir ao menos uma linguagem.");

        var emailExists = await developerRepository.ExistsAsync(d => d.Email == request.Email);
        if (emailExists)
            return Result<DeveloperDto>.Conflict("Já existe um desenvolvedor com este e-mail.");

        var cityExists = await cityRepository.ExistsAsync(c => c.Id == request.CityId);
        if (!cityExists)
            return Result<DeveloperDto>.NotFound("Cidade não encontrada.");

        var languages = new List<ProgrammingLanguage>();
        foreach (var langId in request.LanguageIds.Distinct())
        {
            var lang = await languageRepository.GetByIdAsync(langId);
            if (lang is null)
                return Result<DeveloperDto>.NotFound($"Linguagem com ID '{langId}' não encontrada.");

            languages.Add(lang);
        }

        var developer = new Developer
        {
            Name = request.Name,
            Email = request.Email,
            Seniority = request.Seniority,
            CityId = request.CityId,
            Notes = request.Notes,
            Languages = languages
        };

        await developerRepository.AddAsync(developer);
        await developerRepository.SaveChangesAsync();

        var created = await developerRepository.GetByIdWithRelationsAsync(developer.Id);
        return Result<DeveloperDto>.Created(ToDto(created!));
    }

    public async Task<Result<DeveloperDto>> UpdateAsync(Guid id, UpdateDeveloperRequest request)
    {
        if (!request.LanguageIds.Any())
            return Result<DeveloperDto>.Fail("O desenvolvedor deve possuir ao menos uma linguagem.");

        var dev = await developerRepository.GetByIdWithRelationsAsync(id);
        if (dev is null)
            return Result<DeveloperDto>.NotFound("Desenvolvedor não encontrado.");

        var emailExists = await developerRepository.ExistsAsync(d => d.Email == request.Email && d.Id != id);
        if (emailExists)
            return Result<DeveloperDto>.Conflict("Já existe um desenvolvedor com este e-mail.");

        var cityExists = await cityRepository.ExistsAsync(c => c.Id == request.CityId);
        if (!cityExists)
            return Result<DeveloperDto>.NotFound("Cidade não encontrada.");

        var languages = new List<ProgrammingLanguage>();
        foreach (var langId in request.LanguageIds.Distinct())
        {
            var lang = await languageRepository.GetByIdAsync(langId);
            if (lang is null)
                return Result<DeveloperDto>.NotFound($"Linguagem com ID '{langId}' não encontrada.");

            languages.Add(lang);
        }

        dev.Name = request.Name;
        dev.Email = request.Email;
        dev.Seniority = request.Seniority;
        dev.CityId = request.CityId;
        dev.Notes = request.Notes;
        dev.UpdatedAt = DateTime.UtcNow;
        dev.Languages = languages;

        developerRepository.Update(dev);
        await developerRepository.SaveChangesAsync();

        var updated = await developerRepository.GetByIdWithRelationsAsync(id);
        return Result<DeveloperDto>.Ok(ToDto(updated!));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var dev = await developerRepository.GetByIdAsync(id);
        if (dev is null)
            return Result.NotFound("Desenvolvedor não encontrado.");

        developerRepository.SoftDelete(dev);
        await developerRepository.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result<IEnumerable<DeveloperDto>>> GetAllForReportAsync()
    {
        var devs = await developerRepository.GetAllWithRelationsAsync();
        return Result<IEnumerable<DeveloperDto>>.Ok(devs.Select(ToDto));
    }

    private static DeveloperDto ToDto(Developer d) => new(
        d.Id,
        d.Name,
        d.Email,
        d.Seniority,
        d.Seniority.ToString(),
        d.CityId,
        d.City?.Name ?? "",
        d.City?.State?.Name ?? "",
        d.City?.State?.UF ?? "",
        d.Notes,
        d.Languages.Select(l => new LanguageDto(l.Id, l.Name, l.Type, l.Type.ToString(), l.CreatedAt)),
        d.CreatedAt
    );
}
