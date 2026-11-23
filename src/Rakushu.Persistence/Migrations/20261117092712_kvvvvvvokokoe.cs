using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class kvvvvvvokokoe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ai_request_payload",
                schema: "assistant",
                table: "messages",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ai_request_payload",
                schema: "assistant",
                table: "messages");
        }
    }
}
