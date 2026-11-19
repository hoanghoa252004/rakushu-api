using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _1xnss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_system_configuration_histories_active",
                table: "system_configuration_histories");

            migrationBuilder.EnsureSchema(
                name: "configuration");

            migrationBuilder.RenameTable(
                name: "system_configurations",
                newName: "system_configurations",
                newSchema: "configuration");

            migrationBuilder.RenameTable(
                name: "system_configuration_histories",
                newName: "system_configuration_histories",
                newSchema: "configuration");

            migrationBuilder.CreateIndex(
                name: "ix_system_configuration_histories_configuration_id",
                schema: "configuration",
                table: "system_configuration_histories",
                column: "configuration_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_system_configuration_histories_configuration_id",
                schema: "configuration",
                table: "system_configuration_histories");

            migrationBuilder.RenameTable(
                name: "system_configurations",
                schema: "configuration",
                newName: "system_configurations");

            migrationBuilder.RenameTable(
                name: "system_configuration_histories",
                schema: "configuration",
                newName: "system_configuration_histories");

            migrationBuilder.CreateIndex(
                name: "ux_system_configuration_histories_active",
                table: "system_configuration_histories",
                column: "configuration_id",
                unique: true,
                filter: "is_active = true");
        }
    }
}
