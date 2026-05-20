using Cartsys.Application.DTOs.Languages;
using Cartsys.Domain.Enums;

namespace Cartsys.Application.DTOs.Developers;

public record DeveloperDto(
    Guid Id,
    string Name,
    string Email,
    Seniority Seniority,
    string SeniorityLabel,
    Guid CityId,
    string CityName,
    string StateName,
    string StateUF,
    string? Notes,
    IEnumerable<LanguageDto> Languages,
    DateTime CreatedAt
);

public record CreateDeveloperRequest(
    string Name,
    string Email,
    Seniority Seniority,
    Guid CityId,
    IEnumerable<Guid> LanguageIds,
    string? Notes
);

public record UpdateDeveloperRequest(
    string Name,
    string Email,
    Seniority Seniority,
    Guid CityId,
    IEnumerable<Guid> LanguageIds,
    string? Notes
);

public record DeveloperFilterRequest(
    string? Name,
    string? Email,
    Seniority? Seniority,
    Guid? CityId,
    Guid? LanguageId,
    int Page = 1,
    int PageSize = 10
);
