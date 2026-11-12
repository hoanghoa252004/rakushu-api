using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSrsFlashcardAndSavedItemContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_srs_item_details_srs_schedule_id_reviewed_at",
                schema: "srs",
                table: "srs_item_details");

            migrationBuilder.DropColumn(
                name: "quality",
                schema: "srs",
                table: "flashcard_session_items");

            migrationBuilder.DropColumn(
                name: "video_id",
                schema: "srs",
                table: "flashcard_session_items");

            migrationBuilder.AddColumn<Guid>(
                name: "flashcard_session_item_id",
                schema: "srs",
                table: "srs_item_details",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "transcript_segment_id",
                schema: "srs",
                table: "saved_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_item_details_flashcard_session_item_id",
                schema: "srs",
                table: "srs_item_details",
                column: "flashcard_session_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_srs_item_details_srs_schedule_id",
                schema: "srs",
                table: "srs_item_details",
                column: "srs_schedule_id");

            migrationBuilder.CreateIndex(
                name: "ix_saved_items_transcript_segment_id",
                schema: "srs",
                table: "saved_items",
                column: "transcript_segment_id");

            migrationBuilder.AddForeignKey(
                name: "fk_saved_items_transcript_segments_transcript_segment_id",
                schema: "srs",
                table: "saved_items",
                column: "transcript_segment_id",
                principalSchema: "material",
                principalTable: "transcript_segments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_srs_item_details_flashcard_session_items_flashcard_session_",
                schema: "srs",
                table: "srs_item_details",
                column: "flashcard_session_item_id",
                principalSchema: "srs",
                principalTable: "flashcard_session_items",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_saved_items_transcript_segments_transcript_segment_id",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.DropForeignKey(
                name: "fk_srs_item_details_flashcard_session_items_flashcard_session_",
                schema: "srs",
                table: "srs_item_details");

            migrationBuilder.DropIndex(
                name: "ix_srs_item_details_flashcard_session_item_id",
                schema: "srs",
                table: "srs_item_details");

            migrationBuilder.DropIndex(
                name: "ix_srs_item_details_srs_schedule_id",
                schema: "srs",
                table: "srs_item_details");

            migrationBuilder.DropIndex(
                name: "ix_saved_items_transcript_segment_id",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.DropColumn(
                name: "flashcard_session_item_id",
                schema: "srs",
                table: "srs_item_details");

            migrationBuilder.DropColumn(
                name: "transcript_segment_id",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.AddColumn<int>(
                name: "quality",
                schema: "srs",
                table: "flashcard_session_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "video_id",
                schema: "srs",
                table: "flashcard_session_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_item_details_srs_schedule_id_reviewed_at",
                schema: "srs",
                table: "srs_item_details",
                columns: new[] { "srs_schedule_id", "reviewed_at" });
        }
    }
}
