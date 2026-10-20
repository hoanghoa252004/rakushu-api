using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class KnowledgeElement1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "knowledge_hash",
                schema: "knowledge",
                table: "knowledge_unit_elements");

            migrationBuilder.AddColumn<string>(
                name: "knowledge_hash",
                schema: "knowledge",
                table: "knowledge_units",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "knowledge_hash",
                schema: "knowledge",
                table: "knowledge_units");

            migrationBuilder.AddColumn<string>(
                name: "knowledge_hash",
                schema: "knowledge",
                table: "knowledge_unit_elements",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
