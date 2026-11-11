using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Vsxx5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transcript_segments_transcript_speakers_transcript_speaker_",
                schema: "material",
                table: "transcript_segments");

            migrationBuilder.DropColumn(
                name: "intensity_similarity_score",
                schema: "shadowing",
                table: "shadowing_assessment_details");

            migrationBuilder.DropColumn(
                name: "timing_score",
                schema: "shadowing",
                table: "shadowing_assessment_details");

            migrationBuilder.AlterColumn<Guid>(
                name: "transcript_speaker_id",
                schema: "material",
                table: "transcript_segments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "accuracy_assessment_response",
                schema: "shadowing",
                table: "shadowing_assessment_details",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pitch_similarity_assessment_response",
                schema: "shadowing",
                table: "shadowing_assessment_details",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_transcript_segments_transcript_speakers_transcript_speaker_",
                schema: "material",
                table: "transcript_segments",
                column: "transcript_speaker_id",
                principalSchema: "material",
                principalTable: "transcript_speakers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transcript_segments_transcript_speakers_transcript_speaker_",
                schema: "material",
                table: "transcript_segments");

            migrationBuilder.DropColumn(
                name: "accuracy_assessment_response",
                schema: "shadowing",
                table: "shadowing_assessment_details");

            migrationBuilder.DropColumn(
                name: "pitch_similarity_assessment_response",
                schema: "shadowing",
                table: "shadowing_assessment_details");

            migrationBuilder.AlterColumn<Guid>(
                name: "transcript_speaker_id",
                schema: "material",
                table: "transcript_segments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "intensity_similarity_score",
                schema: "shadowing",
                table: "shadowing_assessment_details",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "timing_score",
                schema: "shadowing",
                table: "shadowing_assessment_details",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_transcript_segments_transcript_speakers_transcript_speaker_",
                schema: "material",
                table: "transcript_segments",
                column: "transcript_speaker_id",
                principalSchema: "material",
                principalTable: "transcript_speakers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
