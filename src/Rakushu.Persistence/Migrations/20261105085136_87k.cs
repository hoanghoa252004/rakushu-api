using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _87k : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shadowing_role_lines_shadowing_materials_shadowing_material",
                schema: "shadowing",
                table: "shadowing_role_lines");

            migrationBuilder.AddForeignKey(
                name: "fk_shadowing_role_lines_shadowing_materials_shadowing_material",
                schema: "shadowing",
                table: "shadowing_role_lines",
                column: "shadowing_material_id",
                principalSchema: "shadowing",
                principalTable: "shadowing_materials",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shadowing_role_lines_shadowing_materials_shadowing_material",
                schema: "shadowing",
                table: "shadowing_role_lines");

            migrationBuilder.AddForeignKey(
                name: "fk_shadowing_role_lines_shadowing_materials_shadowing_material",
                schema: "shadowing",
                table: "shadowing_role_lines",
                column: "shadowing_material_id",
                principalSchema: "shadowing",
                principalTable: "shadowing_materials",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
