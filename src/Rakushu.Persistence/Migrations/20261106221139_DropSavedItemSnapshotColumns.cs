using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DropSavedItemSnapshotColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "expression",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.DropColumn(
                name: "meaning",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.DropColumn(
                name: "reading",
                schema: "srs",
                table: "saved_items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "expression",
                schema: "srs",
                table: "saved_items",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "meaning",
                schema: "srs",
                table: "saved_items",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "reading",
                schema: "srs",
                table: "saved_items",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
