using Cartsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cartsys.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }
    public DbSet<Developer> Developers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Soft delete global filter
        modelBuilder.Entity<User>().HasQueryFilter(u => u.DeletedAt == null);
        modelBuilder.Entity<State>().HasQueryFilter(s => s.DeletedAt == null);
        modelBuilder.Entity<City>().HasQueryFilter(c => c.DeletedAt == null);
        modelBuilder.Entity<ProgrammingLanguage>().HasQueryFilter(l => l.DeletedAt == null);
        modelBuilder.Entity<Developer>().HasQueryFilter(d => d.DeletedAt == null);
    }
}
