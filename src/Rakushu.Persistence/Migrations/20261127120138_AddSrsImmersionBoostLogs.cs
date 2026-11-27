using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSrsImmersionBoostLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "srs_immersion_boost_logs",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    srs_schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    adjustment_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    bonus_days = table.Column<int>(type: "integer", nullable: false),
                    previous_due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    new_due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    boost_number = table.Column<int>(type: "integer", nullable: false),
                    encounter_count_in_window = table.Column<int>(type: "integer", nullable: false),
                    boosted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_immersion_boost_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_srs_immersion_boost_logs_srs_schedules_srs_schedule_id",
                        column: x => x.srs_schedule_id,
                        principalSchema: "srs",
                        principalTable: "srs_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_srs_immersion_boost_logs_srs_schedule_id_boosted_at",
                schema: "srs",
                table: "srs_immersion_boost_logs",
                columns: new[] { "srs_schedule_id", "boosted_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "srs_immersion_boost_logs",
                schema: "srs");
        }
    }
}
