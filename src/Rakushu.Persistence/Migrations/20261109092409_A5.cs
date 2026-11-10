using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class A5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_skipped",
                schema: "shadowing",
                table: "shadowing_attempt_details",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_skipped",
                schema: "shadowing",
                table: "shadowing_attempt_details");
        }
    }
}
