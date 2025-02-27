using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Database.Entities.TypeConfigurations;

/// <summary>
/// Represents the entity type configuration for the <see cref="TelephoneConnectionEntity"/> entity.
/// </summary>
internal sealed class TelephoneConnectionEntityConfiguration : IEntityTypeConfiguration<TelephoneConnectionEntity>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<TelephoneConnectionEntity> builder)
    {
        builder.ToTable("TelephoneConnection");

        builder.HasKey(tc => tc.Id);

        builder.Property(tc => tc.Id)
            .HasColumnName("id");

        builder.Property(tc => tc.PhoneNumber)
            .HasColumnName("telephoneNumber")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(tc => tc.PersonId)
            .HasColumnName("persId")
            .IsRequired();

        builder.HasOne(tc => tc.Person)
            .WithMany(p => p.TelephoneConnections)
            .HasForeignKey(tc => tc.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
