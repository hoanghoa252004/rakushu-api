using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoHistoryAndEncounterTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "learning");

            migrationBuilder.CreateTable(
                name: "learner_encountered_learning_units",
                schema: "learning",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    learning_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_meaning_id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    encounter_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    raw_playback_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    encounter_sources_json = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb"),
                    first_encountered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_encountered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_learner_encountered_learning_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_learner_encountered_learning_units_knowledge_meanings_knowl",
                        column: x => x.knowledge_meaning_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_meanings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_learner_encountered_learning_units_knowledge_units_knowledg",
                        column: x => x.knowledge_unit_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_learner_encountered_learning_units_learning_units_learning_",
                        column: x => x.learning_unit_id,
                        principalSchema: "material",
                        principalTable: "learning_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_learner_encountered_learning_units_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_learner_encountered_learning_units_videos_video_id",
                        column: x => x.video_id,
                        principalSchema: "material",
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "learner_video_progresses",
                schema: "learning",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_watched_second = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    total_watched_seconds = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    watched_intervals_json = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb"),
                    completion_rate = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    last_watched_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_learner_video_progresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_learner_video_progresses_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_learner_video_progresses_videos_video_id",
                        column: x => x.video_id,
                        principalSchema: "material",
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_learner_encountered_learning_units_knowledge_meaning_id",
                schema: "learning",
                table: "learner_encountered_learning_units",
                column: "knowledge_meaning_id");

            migrationBuilder.CreateIndex(
                name: "ix_learner_encountered_learning_units_knowledge_unit_id",
                schema: "learning",
                table: "learner_encountered_learning_units",
                column: "knowledge_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_learner_encountered_learning_units_learning_unit_id",
                schema: "learning",
                table: "learner_encountered_learning_units",
                column: "learning_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_learner_encountered_learning_units_user_id_knowledge_meanin",
                schema: "learning",
                table: "learner_encountered_learning_units",
                columns: new[] { "user_id", "knowledge_meaning_id" });

            migrationBuilder.CreateIndex(
                name: "ix_learner_encountered_learning_units_user_id_knowledge_unit_id",
                schema: "learning",
                table: "learner_encountered_learning_units",
                columns: new[] { "user_id", "knowledge_unit_id" });

            migrationBuilder.CreateIndex(
                name: "ix_learner_encountered_learning_units_user_id_learning_unit_id",
                schema: "learning",
                table: "learner_encountered_learning_units",
                columns: new[] { "user_id", "learning_unit_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_learner_encountered_learning_units_user_id_video_id",
                schema: "learning",
                table: "learner_encountered_learning_units",
                columns: new[] { "user_id", "video_id" });

            migrationBuilder.CreateIndex(
                name: "ix_learner_encountered_learning_units_video_id",
                schema: "learning",
                table: "learner_encountered_learning_units",
                column: "video_id");

            migrationBuilder.CreateIndex(
                name: "ix_learner_video_progresses_user_id_last_watched_at",
                schema: "learning",
                table: "learner_video_progresses",
                columns: new[] { "user_id", "last_watched_at" });

            migrationBuilder.CreateIndex(
                name: "ix_learner_video_progresses_user_id_video_id",
                schema: "learning",
                table: "learner_video_progresses",
                columns: new[] { "user_id", "video_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_learner_video_progresses_video_id",
                schema: "learning",
                table: "learner_video_progresses",
                column: "video_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "learner_encountered_learning_units",
                schema: "learning");

            migrationBuilder.DropTable(
                name: "learner_video_progresses",
                schema: "learning");
        }
    }
}
