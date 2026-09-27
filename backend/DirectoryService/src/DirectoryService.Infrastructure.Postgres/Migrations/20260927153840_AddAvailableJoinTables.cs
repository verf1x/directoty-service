using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailableJoinTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 CREATE VIEW available.department_locations AS
                                 SELECT dl.*
                                 FROM public.department_locations dl
                                 JOIN available.departments d 
                                     ON dl.department_id = d.id
                                 JOIN available.locations l 
                                     ON dl.location_id = l.id
                                 """);

            migrationBuilder.Sql("""
                                 CREATE VIEW available.department_positions AS
                                 SELECT dp.*
                                 FROM public.department_positions dp
                                 JOIN available.departments d
                                     ON d.id = dp.department_id
                                 JOIN available.positions p
                                     ON p.id = dp.position_id;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS available.department_locations;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS available.department_positions;");
        }
    }
}
