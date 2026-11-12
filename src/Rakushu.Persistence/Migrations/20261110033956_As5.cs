using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class As5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shadowing_role_lines_shadowing_materials_shadowing_material",
                schema: "shadowing",
                table: "shadowing_role_lines");

            migrationBuilder.DropIndex(
                name: "ix_shadowing_role_lines_shadowing_material_id_sort_order",
                schema: "shadowing",
                table: "shadowing_role_lines");

            migrationBuilder.DropIndex(
                name: "ix_shadowing_role_lines_shadowing_role_id",
                schema: "shadowing",
                table: "shadowing_role_lines");

            migrationBuilder.DropIndex(
                name: "ix_shadowing_role_line_transcript_segment_shadowing_material_i",
                schema: "shadowing",
                table: "shadowing_role_line_transcript_segment");

            migrationBuilder.DropColumn(
                name: "shadowing_material_id",
                schema: "shadowing",
                table: "shadowing_role_lines");

            migrationBuilder.DropColumn(
                name: "shadowing_material_id",
                schema: "shadowing",
                table: "shadowing_role_line_transcript_segment");

            migrationBuilder.DropColumn(
                name: "type",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.AddColumn<Guid>(
                name: "end_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "name",
                schema: "shadowing",
                table: "shadowing_materials",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "start_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_role_lines_shadowing_role_id_sort_order",
                schema: "shadowing",
                table: "shadowing_role_lines",
                columns: new[] { "shadowing_role_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_materials_end_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials",
                column: "end_transcript_segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_materials_start_transcript_segment_id_end_transcr",
                schema: "shadowing",
                table: "shadowing_materials",
                columns: new[] { "start_transcript_segment_id", "end_transcript_segment_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_shadowing_materials_transcript_segments_end_transcript_segm",
                schema: "shadowing",
                table: "shadowing_materials",
                column: "end_transcript_segment_id",
                principalSchema: "material",
                principalTable: "transcript_segments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_shadowing_materials_transcript_segments_start_transcript_se",
                schema: "shadowing",
                table: "shadowing_materials",
                column: "start_transcript_segment_id",
                principalSchema: "material",
                principalTable: "transcript_segments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shadowing_materials_transcript_segments_end_transcript_segm",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.DropForeignKey(
                name: "fk_shadowing_materials_transcript_segments_start_transcript_se",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.DropIndex(
                name: "ix_shadowing_role_lines_shadowing_role_id_sort_order",
                schema: "shadowing",
                table: "shadowing_role_lines");

            migrationBuilder.DropIndex(
                name: "ix_shadowing_materials_end_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.DropIndex(
                name: "ix_shadowing_materials_start_transcript_segment_id_end_transcr",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.DropColumn(
                name: "end_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.DropColumn(
                name: "name",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.DropColumn(
                name: "start_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.AddColumn<Guid>(
                name: "shadowing_material_id",
                schema: "shadowing",
                table: "shadowing_role_lines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "shadowing_material_id",
                schema: "shadowing",
                table: "shadowing_role_line_transcript_segment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "type",
                schema: "shadowing",
                table: "shadowing_materials",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_role_lines_shadowing_material_id_sort_order",
                schema: "shadowing",
                table: "shadowing_role_lines",
                columns: new[] { "shadowing_material_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_role_lines_shadowing_role_id",
                schema: "shadowing",
                table: "shadowing_role_lines",
                column: "shadowing_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_role_line_transcript_segment_shadowing_material_i",
                schema: "shadowing",
                table: "shadowing_role_line_transcript_segment",
                columns: new[] { "shadowing_material_id", "transcript_segment_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_shadowing_role_lines_shadowing_materials_shadowing_material",
                schema: "shadowing",
                table: "shadowing_role_lines",
                column: "shadowing_material_id",
                principalSchema: "shadowing",
                principalTable: "shadowing_materials",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
