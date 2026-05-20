using Cartsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cartsys.Infrastructure.Data.Configurations;

public class DeveloperConfiguration : IEntityTypeConfiguration<Developer>
{
    public void Configure(EntityTypeBuilder<Developer> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Name).IsRequired().HasMaxLength(150);
        builder.Property(d => d.Email).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Notes).HasMaxLength(500);
        builder.HasIndex(d => d.Email).IsUnique();

        builder.HasMany(d => d.Languages)
            .WithMany(l => l.Developers)
            .UsingEntity(j => j.ToTable("DeveloperLanguages"));
    }
}
