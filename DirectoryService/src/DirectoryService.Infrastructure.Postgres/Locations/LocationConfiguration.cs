using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Locations;

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

        builder.ComplexProperty(l => l.Address, ab =>
        {
            ab.Property(a => a.PostalCode)
                .HasColumnName("postal_code")
                .HasMaxLength(6)
                .IsRequired();

            ab.Property(p => p.Region)
                .IsRequired()
                .HasColumnName("region")
                .HasMaxLength(100);

            ab.Property(p => p.City)
                .IsRequired()
                .HasColumnName("city")
                .HasMaxLength(100);

            ab.Property(p => p.District)
                .IsRequired(false)
                .HasColumnName("district")
                .HasMaxLength(150);

            ab.Property(p => p.Street)
                .IsRequired()
                .HasColumnName("street")
                .HasMaxLength(150);

            ab.Property(p => p.House)
                .IsRequired()
                .HasColumnName("house")
                .HasMaxLength(10);

            ab.Property(p => p.Building)
                .IsRequired(false)
                .HasColumnName("building")
                .HasMaxLength(10);

            ab.Property(p => p.Apartment)
                .IsRequired(false)
                .HasColumnName("apartment")
                .HasMaxLength(10);
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