using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailableViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "available");

            migrationBuilder.Sql("""
                                 CREATE VIEW available.locations AS
                                 SELECT *
                                 FROM public.locations
                                 WHERE deleted_at IS NULL
                                   AND is_active = true;
                                 """);

            migrationBuilder.Sql("""
                                 CREATE VIEW available.departments AS
                                 SELECT *
                                 FROM public.departments
                                 WHERE deleted_at IS NULL
                                   AND is_active = true;
                                 """);

            migrationBuilder.Sql("""
                                 CREATE VIEW available.positions AS
                                 SELECT *
                                 FROM public.positions
                                 WHERE deleted_at IS NULL
                                   AND is_active = true;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS available.locations;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS available.departments;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS available.positions;");
            migrationBuilder.Sql("DROP SCHEMA IF EXISTS available;");
        }
    }
}
