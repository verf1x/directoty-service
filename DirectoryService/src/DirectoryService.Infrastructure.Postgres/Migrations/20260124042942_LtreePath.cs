using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class LtreePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "positions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "locations",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "departments",
                newName: "id");

            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS ltree;");

            migrationBuilder.Sql("""
                                 ALTER TABLE departments 
                                 ALTER COLUMN path TYPE ltree 
                                 USING path::ltree;
                                 """);

            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS ix_departments_path ON departments USING GIST (path);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_departments_path",
                table: "departments");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "positions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "locations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "departments",
                newName: "Id");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:ltree", ",,");

            migrationBuilder.Sql(@"ALTER TABLE departments ALTER COLUMN path TYPE text USING path::text;");
        }
    }
}
