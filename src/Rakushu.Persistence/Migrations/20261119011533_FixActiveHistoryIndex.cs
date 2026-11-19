using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixActiveHistoryIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_system_configuration_histories_configuration_id",
                schema: "configuration",
                table: "system_configuration_histories");

            migrationBuilder.CreateIndex(
                name: "ux_system_configuration_histories_active",
                schema: "configuration",
                table: "system_configuration_histories",
                column: "configuration_id",
                unique: true,
                filter: "is_active = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_system_configuration_histories_active",
                schema: "configuration",
                table: "system_configuration_histories");

            migrationBuilder.CreateIndex(
                name: "ix_system_configuration_histories_configuration_id",
                schema: "configuration",
                table: "system_configuration_histories",
                column: "configuration_id",
                unique: true);
        }
    }
}
