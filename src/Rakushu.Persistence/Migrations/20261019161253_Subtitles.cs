using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Subtitles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_subtitle_segments_transcript_segments_transcript_segment_id",
                schema: "material",
                table: "subtitle_segments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_subtitle_segments",
                schema: "material",
                table: "subtitle_segments");

            migrationBuilder.RenameTable(
                name: "subtitle_segments",
                schema: "material",
                newName: "subtitles",
                newSchema: "material");

            migrationBuilder.RenameIndex(
                name: "ix_subtitle_segments_transcript_segment_id",
                schema: "material",
                table: "subtitles",
                newName: "ix_subtitles_transcript_segment_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_subtitles",
                schema: "material",
                table: "subtitles",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_subtitles_transcript_segments_transcript_segment_id",
                schema: "material",
                table: "subtitles",
                column: "transcript_segment_id",
                principalSchema: "material",
                principalTable: "transcript_segments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_subtitles_transcript_segments_transcript_segment_id",
                schema: "material",
                table: "subtitles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_subtitles",
                schema: "material",
                table: "subtitles");

            migrationBuilder.RenameTable(
                name: "subtitles",
                schema: "material",
                newName: "subtitle_segments",
                newSchema: "material");

            migrationBuilder.RenameIndex(
                name: "ix_subtitles_transcript_segment_id",
                schema: "material",
                table: "subtitle_segments",
                newName: "ix_subtitle_segments_transcript_segment_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_subtitle_segments",
                schema: "material",
                table: "subtitle_segments",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_subtitle_segments_transcript_segments_transcript_segment_id",
                schema: "material",
                table: "subtitle_segments",
                column: "transcript_segment_id",
                principalSchema: "material",
                principalTable: "transcript_segments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
