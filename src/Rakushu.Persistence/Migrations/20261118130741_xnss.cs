using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class xnss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "system_configurations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    config_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    data_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    config_value = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    version_in_used = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_configurations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "system_configuration_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    configuration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    config_value = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_configuration_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_system_configuration_histories_system_configurations_config",
                        column: x => x.configuration_id,
                        principalTable: "system_configurations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_system_configuration_histories_configuration_id_version",
                table: "system_configuration_histories",
                columns: new[] { "configuration_id", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_system_configuration_histories_active",
                table: "system_configuration_histories",
                column: "configuration_id",
                unique: true,
                filter: "is_active = true");

            migrationBuilder.CreateIndex(
                name: "ix_system_configurations_config_key",
                table: "system_configurations",
                column: "config_key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "system_configuration_histories");

            migrationBuilder.DropTable(
                name: "system_configurations");
        }
    }
}
