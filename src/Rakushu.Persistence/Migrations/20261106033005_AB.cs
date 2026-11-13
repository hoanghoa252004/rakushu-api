using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_shadowing_role_lines_shadowing_material_id_sort_order",
                schema: "shadowing",
                table: "shadowing_role_lines");

            migrationBuilder.AlterColumn<Guid>(
                name: "shadowing_role_id",
                schema: "shadowing",
                table: "shadowing_session_members",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "shadowing_role_line_transcript_segment",
                schema: "shadowing",
                columns: table => new
                {
                    shadowing_role_line_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transcript_segment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_material_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_role_line_transcript_segment", x => new { x.shadowing_role_line_id, x.transcript_segment_id });
                    table.ForeignKey(
                        name: "fk_shadowing_role_line_transcript_segment_shadowing_role_lines",
                        column: x => x.shadowing_role_line_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_role_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shadowing_role_line_transcript_segment_transcript_segments_",
                        column: x => x.transcript_segment_id,
                        principalSchema: "material",
                        principalTable: "transcript_segments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_role_lines_shadowing_material_id_sort_order",
                schema: "shadowing",
                table: "shadowing_role_lines",
                columns: new[] { "shadowing_material_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_role_line_transcript_segment_shadowing_material_i",
                schema: "shadowing",
                table: "shadowing_role_line_transcript_segment",
                columns: new[] { "shadowing_material_id", "transcript_segment_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_role_line_transcript_segment_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_role_line_transcript_segment",
                column: "transcript_segment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shadowing_role_line_transcript_segment",
                schema: "shadowing");

            migrationBuilder.DropIndex(
                name: "ix_shadowing_role_lines_shadowing_material_id_sort_order",
                schema: "shadowing",
                table: "shadowing_role_lines");

            migrationBuilder.AlterColumn<Guid>(
                name: "shadowing_role_id",
                schema: "shadowing",
                table: "shadowing_session_members",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_role_lines_shadowing_material_id_sort_order",
                schema: "shadowing",
                table: "shadowing_role_lines",
                columns: new[] { "shadowing_material_id", "sort_order" },
                unique: true);
        }
    }
}
