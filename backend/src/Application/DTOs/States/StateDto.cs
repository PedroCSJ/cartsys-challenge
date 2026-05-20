namespace Cartsys.Application.DTOs.States;

public record StateDto(Guid Id, string Name, string UF, DateTime CreatedAt);

public record CreateStateRequest(string Name, string UF);

public record UpdateStateRequest(string Name, string UF);

public record StateFilterRequest(string? Name, string? UF, int Page = 1, int PageSize = 10);
