using DirectoryService.Domain.Entities;
using DirectoryService.Domain.EntityIds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.Id).HasName("pk_departments");

        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => DepartmentId.Create(value));

        builder.ComplexProperty(d => d.Name, dnb =>
        {
            dnb.Property(dn => dn.Value)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.ComplexProperty(d => d.Identifier, dib =>
        {
            dib.Property(i => i.Value)
                .HasColumnName("identifier")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.OwnsOne(d => d.ParentId, dpb =>
        {
            dpb.Property(d => d.Value)
                .HasColumnName("parent_id");
        });

        builder.Navigation(d => d.ParentId).IsRequired(false);

        builder.ComplexProperty(d => d.Path, dpb =>
        {
            dpb.Property(d => d.Value)
                .HasColumnName("path")
                .HasMaxLength(500)
                .IsRequired();
        });

        builder.Property(d => d.Depth)
            .HasColumnName("depth")
            .IsRequired();

        builder.Property(d => d.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
    }
}