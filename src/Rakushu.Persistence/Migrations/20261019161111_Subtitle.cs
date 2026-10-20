using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Subtitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_subtitle_segments_subtitles_subtitle_id",
                schema: "material",
                table: "subtitle_segments");

            migrationBuilder.DropTable(
                name: "bunsetsu_dependency_relationships",
                schema: "material");

            migrationBuilder.DropTable(
                name: "subtitles",
                schema: "material");

            migrationBuilder.DropIndex(
                name: "ix_subtitle_segments_subtitle_id",
                schema: "material",
                table: "subtitle_segments");

            migrationBuilder.DropColumn(
                name: "end_time",
                schema: "material",
                table: "subtitle_segments");

            migrationBuilder.DropColumn(
                name: "original_text",
                schema: "material",
                table: "subtitle_segments");

            migrationBuilder.DropColumn(
                name: "start_time",
                schema: "material",
                table: "subtitle_segments");

            migrationBuilder.DropColumn(
                name: "subtitle_id",
                schema: "material",
                table: "subtitle_segments");

            migrationBuilder.RenameColumn(
                name: "translated_text",
                schema: "material",
                table: "subtitle_segments",
                newName: "text");

            migrationBuilder.AddColumn<string>(
                name: "language_code",
                schema: "material",
                table: "subtitle_segments",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "language_code",
                schema: "material",
                table: "subtitle_segments");

            migrationBuilder.RenameColumn(
                name: "text",
                schema: "material",
                table: "subtitle_segments",
                newName: "translated_text");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "end_time",
                schema: "material",
                table: "subtitle_segments",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "original_text",
                schema: "material",
                table: "subtitle_segments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "start_time",
                schema: "material",
                table: "subtitle_segments",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<Guid>(
                name: "subtitle_id",
                schema: "material",
                table: "subtitle_segments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "bunsetsu_dependency_relationships",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    dependency_relationship_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_bunsetsu_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_bunsetsu_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bunsetsu_dependency_relationships", x => x.id);
                    table.ForeignKey(
                        name: "fk_bunsetsu_dependency_relationships_bunsetsu_from_bunsetsu_id",
                        column: x => x.from_bunsetsu_id,
                        principalSchema: "material",
                        principalTable: "bunsetsu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bunsetsu_dependency_relationships_bunsetsu_to_bunsetsu_id",
                        column: x => x.to_bunsetsu_id,
                        principalSchema: "material",
                        principalTable: "bunsetsu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bunsetsu_dependency_relationships_dependency_relationships_",
                        column: x => x.dependency_relationship_id,
                        principalSchema: "knowledge",
                        principalTable: "dependency_relationships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subtitles",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subtitles", x => x.id);
                    table.ForeignKey(
                        name: "fk_subtitles_videos_video_id",
                        column: x => x.video_id,
                        principalSchema: "material",
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_subtitle_segments_subtitle_id",
                schema: "material",
                table: "subtitle_segments",
                column: "subtitle_id");

            migrationBuilder.CreateIndex(
                name: "ix_bunsetsu_dependency_relationships_dependency_relationship_id",
                schema: "material",
                table: "bunsetsu_dependency_relationships",
                column: "dependency_relationship_id");

            migrationBuilder.CreateIndex(
                name: "ix_bunsetsu_dependency_relationships_from_bunsetsu_id",
                schema: "material",
                table: "bunsetsu_dependency_relationships",
                column: "from_bunsetsu_id");

            migrationBuilder.CreateIndex(
                name: "ix_bunsetsu_dependency_relationships_to_bunsetsu_id",
                schema: "material",
                table: "bunsetsu_dependency_relationships",
                column: "to_bunsetsu_id");

            migrationBuilder.CreateIndex(
                name: "ix_subtitles_code",
                schema: "material",
                table: "subtitles",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subtitles_video_id",
                schema: "material",
                table: "subtitles",
                column: "video_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_subtitle_segments_subtitles_subtitle_id",
                schema: "material",
                table: "subtitle_segments",
                column: "subtitle_id",
                principalSchema: "material",
                principalTable: "subtitles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
