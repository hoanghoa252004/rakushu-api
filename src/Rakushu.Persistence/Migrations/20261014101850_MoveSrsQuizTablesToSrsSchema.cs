using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveSrsQuizTablesToSrsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "srs");

            migrationBuilder.RenameTable(
                name: "srs_sessions",
                newName: "srs_sessions",
                newSchema: "srs");

            migrationBuilder.RenameTable(
                name: "srs_session_items",
                newName: "srs_session_items",
                newSchema: "srs");

            migrationBuilder.RenameTable(
                name: "srs_review_logs",
                newName: "srs_review_logs",
                newSchema: "srs");

            migrationBuilder.RenameTable(
                name: "srs_notes",
                newName: "srs_notes",
                newSchema: "srs");

            migrationBuilder.RenameTable(
                name: "srs_cards",
                newName: "srs_cards",
                newSchema: "srs");

            migrationBuilder.RenameTable(
                name: "srs_card_states",
                newName: "srs_card_states",
                newSchema: "srs");

            migrationBuilder.RenameTable(
                name: "quiz_attempts",
                newName: "quiz_attempts",
                newSchema: "srs");

            migrationBuilder.RenameTable(
                name: "quiz_attempt_items",
                newName: "quiz_attempt_items",
                newSchema: "srs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "srs_sessions",
                schema: "srs",
                newName: "srs_sessions");

            migrationBuilder.RenameTable(
                name: "srs_session_items",
                schema: "srs",
                newName: "srs_session_items");

            migrationBuilder.RenameTable(
                name: "srs_review_logs",
                schema: "srs",
                newName: "srs_review_logs");

            migrationBuilder.RenameTable(
                name: "srs_notes",
                schema: "srs",
                newName: "srs_notes");

            migrationBuilder.RenameTable(
                name: "srs_cards",
                schema: "srs",
                newName: "srs_cards");

            migrationBuilder.RenameTable(
                name: "srs_card_states",
                schema: "srs",
                newName: "srs_card_states");

            migrationBuilder.RenameTable(
                name: "quiz_attempts",
                schema: "srs",
                newName: "quiz_attempts");

            migrationBuilder.RenameTable(
                name: "quiz_attempt_items",
                schema: "srs",
                newName: "quiz_attempt_items");
        }
    }
}
