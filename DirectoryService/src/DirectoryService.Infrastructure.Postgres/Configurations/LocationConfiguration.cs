using DirectoryService.Domain.Entities;
using DirectoryService.Domain.EntityIds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.Id).HasName("pk_locations");

        builder.Property(l => l.Id)
            .HasConversion(
                id => id.Value,
                value => LocationId.Create(value));

        builder.ComplexProperty(l => l.Name, lnb =>
        {
            lnb.Property(ln => ln.Value)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.OwnsOne(l => l.Address, ab =>
        {
            ab.ToJson("address");

            ab.Property(a => a.AddressLines)
                .HasColumnName("address_lines")
                .IsRequired();

            ab.Property(p => p.Locality)
                    .IsRequired()
                    .HasColumnName("locality")
                    .HasMaxLength(100);

            ab.Property(p => p.Region)
                .IsRequired(false)
                .HasColumnName("region")
                .HasMaxLength(100);

            ab.Property(p => p.CountryCode)
                .IsRequired()
                .HasColumnName("country_code")
                .HasMaxLength(2);

            ab.Property(p => p.PostalCode)
                .IsRequired(false)
                .HasColumnName("postal_code")
                .HasMaxLength(20);
        });

        builder.ComplexProperty(l => l.TimeZone, tzb =>
        {
            tzb.Property(tz => tz.Value)
                .HasColumnName("time_zone")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.Property(l => l.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
    }
}
