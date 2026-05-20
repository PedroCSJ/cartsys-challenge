using Cartsys.Domain.Enums;

namespace Cartsys.Application.DTOs.Languages;

public record LanguageDto(Guid Id, string Name, LanguageType Type, string TypeLabel, DateTime CreatedAt);

public record CreateLanguageRequest(string Name, LanguageType Type);

public record UpdateLanguageRequest(string Name, LanguageType Type);

public record LanguageFilterRequest(string? Name, LanguageType? Type, int Page = 1, int PageSize = 10);
