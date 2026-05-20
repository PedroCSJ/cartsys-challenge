using Cartsys.Domain.Enums;

namespace Cartsys.Domain.Entities;

public class ProgrammingLanguage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public LanguageType Type { get; set; }

    public ICollection<Developer> Developers { get; set; } = new List<Developer>();
}
