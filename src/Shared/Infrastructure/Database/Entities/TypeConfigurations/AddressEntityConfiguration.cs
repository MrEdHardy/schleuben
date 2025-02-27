using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Database.Entities.TypeConfigurations;

/// <summary>
/// Represents the entity type configuration for the <see cref="AddressEntity"/> entity.
/// </summary>
internal class AddressEntityConfiguration : IEntityTypeConfiguration<AddressEntity>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<AddressEntity> builder)
    {
        builder.ToTable("Address");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id");

        builder.Property(a => a.Street)
            .HasColumnName("street")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.HouseNumber)
            .HasColumnName("houseNumber")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(a => a.City)
            .HasColumnName("city")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.ZipCode)
            .HasColumnName("postCode")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(a => a.AdditionalInfo)
            .HasColumnName("additionalInfo")
            .HasMaxLength(100);

        builder.Property(a => a.PersonId)
            .HasColumnName("persId")
            .IsRequired();

        builder.HasOne(a => a.Person)
            .WithMany(p => p.Addresses)
            .HasForeignKey(a => a.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
