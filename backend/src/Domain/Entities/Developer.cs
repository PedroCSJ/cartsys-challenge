using Cartsys.Domain.Enums;

namespace Cartsys.Domain.Entities;

public class Developer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Seniority Seniority { get; set; }
    public string? Notes { get; set; }

    public Guid CityId { get; set; }
    public City City { get; set; } = null!;

    public ICollection<ProgrammingLanguage> Languages { get; set; } = new List<ProgrammingLanguage>();
}
