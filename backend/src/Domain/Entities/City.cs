namespace Cartsys.Domain.Entities;

public class City : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid StateId { get; set; }
    public State State { get; set; } = null!;

    public ICollection<Developer> Developers { get; set; } = new List<Developer>();
}
