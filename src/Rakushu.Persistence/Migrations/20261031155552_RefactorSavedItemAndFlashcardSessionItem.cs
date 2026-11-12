using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSavedItemAndFlashcardSessionItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_saved_items_transcript_segments_transcript_segment_id",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.DropIndex(
                name: "ix_saved_items_transcript_segment_id",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.DropColumn(
                name: "transcript_segment_id",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.DropColumn(
                name: "end_time",
                schema: "srs",
                table: "flashcard_session_items");

            migrationBuilder.DropColumn(
                name: "end_token_index",
                schema: "srs",
                table: "flashcard_session_items");

            migrationBuilder.DropColumn(
                name: "expression",
                schema: "srs",
                table: "flashcard_session_items");

            migrationBuilder.DropColumn(
                name: "meaning",
                schema: "srs",
                table: "flashcard_session_items");

            migrationBuilder.DropColumn(
                name: "original_sentence",
                schema: "srs",
                table: "flashcard_session_items");

            migrationBuilder.DropColumn(
                name: "reading",
                schema: "srs",
                table: "flashcard_session_items");

            migrationBuilder.DropColumn(
                name: "start_time",
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
                name: "translated_sentence",
                schema: "srs",
                table: "flashcard_session_items");

            migrationBuilder.DropColumn(
                name: "video_source_url",
                schema: "srs",
                table: "flashcard_session_items");

            migrationBuilder.DropColumn(
                name: "video_title",
                schema: "srs",
                table: "flashcard_session_items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "transcript_segment_id",
                schema: "srs",
                table: "saved_items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "end_time",
                schema: "srs",
                table: "flashcard_session_items",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "end_token_index",
                schema: "srs",
                table: "flashcard_session_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "expression",
                schema: "srs",
                table: "flashcard_session_items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "meaning",
                schema: "srs",
                table: "flashcard_session_items",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "original_sentence",
                schema: "srs",
                table: "flashcard_session_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reading",
                schema: "srs",
                table: "flashcard_session_items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "start_time",
                schema: "srs",
                table: "flashcard_session_items",
                type: "interval",
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
                name: "translated_sentence",
                schema: "srs",
                table: "flashcard_session_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "video_source_url",
                schema: "srs",
                table: "flashcard_session_items",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "video_title",
                schema: "srs",
                table: "flashcard_session_items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

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
                onDelete: ReferentialAction.Cascade);
        }
    }
}
