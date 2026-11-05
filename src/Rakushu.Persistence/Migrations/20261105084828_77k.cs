using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _77k : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shadowing_materials_shadowing_material_requests_shadowing_m",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.DropTable(
                name: "shadowing_material_requests",
                schema: "shadowing");

            migrationBuilder.DropIndex(
                name: "ix_shadowing_materials_shadowing_material_request_id",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.DropColumn(
                name: "shadowing_material_request_id",
                schema: "shadowing",
                table: "shadowing_materials");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "shadowing_material_request_id",
                schema: "shadowing",
                table: "shadowing_materials",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "shadowing_material_requests",
                schema: "shadowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    learner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    reject_reason = table.Column<string>(type: "text", nullable: true),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_material_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_shadowing_material_requests_users_learner_id",
                        column: x => x.learner_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shadowing_material_requests_videos_video_id",
                        column: x => x.video_id,
                        principalSchema: "material",
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_materials_shadowing_material_request_id",
                schema: "shadowing",
                table: "shadowing_materials",
                column: "shadowing_material_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_material_requests_code",
                schema: "shadowing",
                table: "shadowing_material_requests",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_material_requests_learner_id",
                schema: "shadowing",
                table: "shadowing_material_requests",
                column: "learner_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_material_requests_video_id",
                schema: "shadowing",
                table: "shadowing_material_requests",
                column: "video_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_shadowing_materials_shadowing_material_requests_shadowing_m",
                schema: "shadowing",
                table: "shadowing_materials",
                column: "shadowing_material_request_id",
                principalSchema: "shadowing",
                principalTable: "shadowing_material_requests",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
