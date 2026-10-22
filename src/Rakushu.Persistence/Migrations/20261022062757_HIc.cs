using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HIc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                schema: "material",
                table: "transcript_speakers");

            migrationBuilder.DropColumn(
                name: "name",
                schema: "material",
                table: "transcript_speakers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "material",
                table: "transcript_speakers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                schema: "material",
                table: "transcript_speakers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
