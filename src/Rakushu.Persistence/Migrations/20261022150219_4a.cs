using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _4a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_subtitles_transcript_segments_transcript_segment_id",
                schema: "material",
                table: "subtitles");

            migrationBuilder.AddForeignKey(
                name: "fk_subtitles_transcript_segments_transcript_segment_id",
                schema: "material",
                table: "subtitles",
                column: "transcript_segment_id",
                principalSchema: "material",
                principalTable: "transcript_segments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_subtitles_transcript_segments_transcript_segment_id",
                schema: "material",
                table: "subtitles");

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
    }
}
