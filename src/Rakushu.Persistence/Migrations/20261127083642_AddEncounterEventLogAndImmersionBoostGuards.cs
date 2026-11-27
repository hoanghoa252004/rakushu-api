using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEncounterEventLogAndImmersionBoostGuards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "consecutive_passive_boosts",
                schema: "srs",
                table: "srs_schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_immersion_boost_at",
                schema: "srs",
                table: "srs_schedules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "learner_encounter_events",
                schema: "learning",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    learning_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_meaning_id = table.Column<Guid>(type: "uuid", nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    encountered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_learner_encounter_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_learner_encounter_events_knowledge_meanings_knowledge_meani",
                        column: x => x.knowledge_meaning_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_meanings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_learner_encounter_events_learning_units_learning_unit_id",
                        column: x => x.learning_unit_id,
                        principalSchema: "material",
                        principalTable: "learning_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_learner_encounter_events_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_learner_encounter_events_videos_video_id",
                        column: x => x.video_id,
                        principalSchema: "material",
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_learner_encounter_events_knowledge_meaning_id",
                schema: "learning",
                table: "learner_encounter_events",
                column: "knowledge_meaning_id");

            migrationBuilder.CreateIndex(
                name: "ix_learner_encounter_events_learning_unit_id",
                schema: "learning",
                table: "learner_encounter_events",
                column: "learning_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_learner_encounter_events_user_id_knowledge_meaning_id_encou",
                schema: "learning",
                table: "learner_encounter_events",
                columns: new[] { "user_id", "knowledge_meaning_id", "encountered_at" });

            migrationBuilder.CreateIndex(
                name: "ix_learner_encounter_events_user_id_video_id",
                schema: "learning",
                table: "learner_encounter_events",
                columns: new[] { "user_id", "video_id" });

            migrationBuilder.CreateIndex(
                name: "ix_learner_encounter_events_video_id",
                schema: "learning",
                table: "learner_encounter_events",
                column: "video_id");

            // Backfill one event per recently encountered context so active 7-day badges stay populated after upgrade.
            migrationBuilder.Sql("""
                INSERT INTO learning.learner_encounter_events (
                    id,
                    user_id,
                    learning_unit_id,
                    knowledge_meaning_id,
                    video_id,
                    encountered_at
                )
                SELECT
                    gen_random_uuid(),
                    user_id,
                    learning_unit_id,
                    knowledge_meaning_id,
                    video_id,
                    last_encountered_at
                FROM learning.learner_encountered_learning_units
                WHERE last_encountered_at >= NOW() - INTERVAL '7 days';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "learner_encounter_events",
                schema: "learning");

            migrationBuilder.DropColumn(
                name: "consecutive_passive_boosts",
                schema: "srs",
                table: "srs_schedules");

            migrationBuilder.DropColumn(
                name: "last_immersion_boost_at",
                schema: "srs",
                table: "srs_schedules");
        }
    }
}
