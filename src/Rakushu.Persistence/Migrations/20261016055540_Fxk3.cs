using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Fxk3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "confidence",
                schema: "material",
                table: "transcript_items",
                type: "numeric(8,5)",
                precision: 8,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(8,7)",
                oldPrecision: 8,
                oldScale: 7);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "confidence",
                schema: "material",
                table: "transcript_items",
                type: "numeric(8,7)",
                precision: 8,
                scale: 7,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(8,5)",
                oldPrecision: 8,
                oldScale: 5);
        }
    }
}
