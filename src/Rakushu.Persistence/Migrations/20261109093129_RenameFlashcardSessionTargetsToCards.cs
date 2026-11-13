using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameFlashcardSessionTargetsToCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "flashcard_session_targets",
                schema: "srs",
                newName: "flashcard_session_cards");

            migrationBuilder.RenameIndex(
                name: "ix_flashcard_session_targets_saved_item_id",
                schema: "srs",
                table: "flashcard_session_cards",
                newName: "ix_flashcard_session_cards_saved_item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "flashcard_session_cards",
                schema: "srs",
                newName: "flashcard_session_targets");

            migrationBuilder.RenameIndex(
                name: "ix_flashcard_session_cards_saved_item_id",
                schema: "srs",
                table: "flashcard_session_targets",
                newName: "ix_flashcard_session_targets_saved_item_id");
        }
    }
}
