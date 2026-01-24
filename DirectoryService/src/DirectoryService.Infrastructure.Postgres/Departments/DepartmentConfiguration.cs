using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Path = DirectoryService.Domain.Departments.Path;

namespace DirectoryService.Infrastructure.Postgres.Departments;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.Id)
            .HasName("pk_departments");

        builder.Property(d => d.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => DepartmentId.Create(value));

        builder.Property(d => d.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired()
            .HasConversion(
                name => name.Value,
                value => DepartmentName.Create(value).Value);

        builder.Property(d => d.Identifier)
            .HasColumnName("identifier")
            .HasMaxLength(200)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => Identifier.Create(value).Value);

        builder.OwnsOne(d => d.ParentId, dpb =>
        {
            dpb.Property(d => d.Value)
                .HasColumnName("parent_id");
        });

        builder.Navigation(d => d.ParentId).IsRequired(false);

        builder.Property(x => x.Path)
            .HasColumnName("path")
            .HasColumnType("ltree")
            .IsRequired()
            .HasConversion(
                value => value.Value,
                value => Path.Create(value).Value);

        builder.HasIndex(x => x.Path)
            .HasMethod("gist")
            .HasDatabaseName("ix_departments_path");

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