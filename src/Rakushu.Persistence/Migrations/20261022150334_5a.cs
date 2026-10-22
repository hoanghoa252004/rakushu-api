using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _5a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_learning_units_knowledge_meanings_knowledge_meaning_id",
                schema: "material",
                table: "learning_units");

            migrationBuilder.DropForeignKey(
                name: "fk_learning_units_transcript_segments_transcript_segment_id",
                schema: "material",
                table: "learning_units");

            migrationBuilder.AddForeignKey(
                name: "fk_learning_units_knowledge_meanings_knowledge_meaning_id",
                schema: "material",
                table: "learning_units",
                column: "knowledge_meaning_id",
                principalSchema: "knowledge",
                principalTable: "knowledge_meanings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_learning_units_transcript_segments_transcript_segment_id",
                schema: "material",
                table: "learning_units",
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
                name: "fk_learning_units_knowledge_meanings_knowledge_meaning_id",
                schema: "material",
                table: "learning_units");

            migrationBuilder.DropForeignKey(
                name: "fk_learning_units_transcript_segments_transcript_segment_id",
                schema: "material",
                table: "learning_units");

            migrationBuilder.AddForeignKey(
                name: "fk_learning_units_knowledge_meanings_knowledge_meaning_id",
                schema: "material",
                table: "learning_units",
                column: "knowledge_meaning_id",
                principalSchema: "knowledge",
                principalTable: "knowledge_meanings",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_learning_units_transcript_segments_transcript_segment_id",
                schema: "material",
                table: "learning_units",
                column: "transcript_segment_id",
                principalSchema: "material",
                principalTable: "transcript_segments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
