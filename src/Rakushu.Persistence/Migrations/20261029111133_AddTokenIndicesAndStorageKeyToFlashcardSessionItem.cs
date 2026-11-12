using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTokenIndicesAndStorageKeyToFlashcardSessionItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "end_token_index",
                schema: "srs",
                table: "flashcard_session_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "start_token_index",
                schema: "srs",
                table: "flashcard_session_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "storage_key",
                schema: "srs",
                table: "flashcard_session_items",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "video_source_url",
                schema: "srs",
                table: "flashcard_session_items",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_token_index",
                schema: "srs",
                table: "flashcard_session_items");

            migrationBuilder.DropColumn(
                name: "start_token_index",
                schema: "srs",
                table: "flashcard_session_items");

            migrationBuilder.DropColumn(
                name: "storage_key",
                schema: "srs",
                table: "flashcard_session_items");

            migrationBuilder.DropColumn(
                name: "video_source_url",
                schema: "srs",
                table: "flashcard_session_items");
        }
    }
}
