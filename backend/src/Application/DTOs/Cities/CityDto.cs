namespace Cartsys.Application.DTOs.Cities;

public record CityDto(Guid Id, string Name, Guid StateId, string StateName, string StateUF, DateTime CreatedAt);

public record CreateCityRequest(string Name, Guid StateId);

public record UpdateCityRequest(string Name, Guid StateId);

public record CityFilterRequest(string? Name, Guid? StateId, int Page = 1, int PageSize = 10);
