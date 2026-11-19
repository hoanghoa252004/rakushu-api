using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTargetSnapshotToCuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "target_snapshot",
                schema: "curation",
                table: "reports",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "target_snapshot",
                schema: "curation",
                table: "issue_details",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "target_snapshot",
                schema: "curation",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "target_snapshot",
                schema: "curation",
                table: "issue_details");
        }
    }
}
