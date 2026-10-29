using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeTranscriptSegmentIdRequiredInSavedItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_saved_items_transcript_segments_transcript_segment_id",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.AlterColumn<Guid>(
                name: "transcript_segment_id",
                schema: "srs",
                table: "saved_items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_saved_items_transcript_segments_transcript_segment_id",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.AlterColumn<Guid>(
                name: "transcript_segment_id",
                schema: "srs",
                table: "saved_items",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "fk_saved_items_transcript_segments_transcript_segment_id",
                schema: "srs",
                table: "saved_items",
                column: "transcript_segment_id",
                principalSchema: "material",
                principalTable: "transcript_segments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
