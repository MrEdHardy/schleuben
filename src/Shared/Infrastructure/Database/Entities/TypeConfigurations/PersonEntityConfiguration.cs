using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Shared.Infrastructure.Database.Entities.TypeConfigurations;

/// <summary>
/// Represents the entity type configuration for the <see cref="PersonEntity"/> entity.
/// </summary>
internal class PersonEntityConfiguration : IEntityTypeConfiguration<PersonEntity>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<PersonEntity> builder)
    {
        builder.ToTable("Person");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id");

        builder.Property(p => p.FirstName)
            .HasColumnName("firstName")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.LastName)
            .HasColumnName("lastName")
            .HasMaxLength(100)
            .IsRequired();

        var dateConverter = new ValueConverter<DateOnly?, string?>(
            v => v.HasValue ? v.Value.ToString("yyyy-MM-dd") : null,
            v => v != null ? DateOnly.Parse(v) : null);

        builder.Property(p => p.BirthDate)
            .HasColumnName("birthDate")
            .HasConversion(dateConverter);
    }
}
