using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Positions;

public sealed class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(p => p.Id).HasName("pk_positions");

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => PositionId.Create(value));

        builder.ComplexProperty(p => p.Name, pnb =>
        {
            pnb.Property(pn => pn.Value)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.ComplexProperty(p => p.Description, pdb =>
        {
            pdb.Property(pd => pd.Value)
                .HasColumnName("description")
                .HasMaxLength(5000)
                .IsRequired(false);
        });

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
    }
}