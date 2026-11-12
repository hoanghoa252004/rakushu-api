using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Appp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_shadowing_materials_start_transcript_segment_id_end_transcr",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_materials_start_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials",
                column: "start_transcript_segment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_shadowing_materials_start_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_materials_start_transcript_segment_id_end_transcr",
                schema: "shadowing",
                table: "shadowing_materials",
                columns: new[] { "start_transcript_segment_id", "end_transcript_segment_id" },
                unique: true);
        }
    }
}
