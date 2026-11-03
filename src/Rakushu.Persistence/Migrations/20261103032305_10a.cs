using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _10a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.AddColumn<string>(
                name: "status",
                schema: "shadowing",
                table: "shadowing_materials",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "shadowing",
                table: "shadowing_materials",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
