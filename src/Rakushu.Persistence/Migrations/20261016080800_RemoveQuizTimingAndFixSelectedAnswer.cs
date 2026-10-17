using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveQuizTimingAndFixSelectedAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_quiz_attempts_elapsed_ms",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropCheckConstraint(
                name: "ck_quiz_attempt_items_elapsed_ms",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropColumn(
                name: "elapsed_ms",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropColumn(
                name: "elapsed_ms",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.AlterColumn<string>(
                name: "selected_answer",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_session_items_srs_session_id_srs_card_id",
                schema: "srs",
                table: "srs_session_items",
                columns: new[] { "srs_session_id", "srs_card_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_srs_session_items_srs_session_id_srs_card_id",
                schema: "srs",
                table: "srs_session_items");

            migrationBuilder.AddColumn<long>(
                name: "elapsed_ms",
                schema: "srs",
                table: "quiz_attempts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "selected_answer",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "elapsed_ms",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_quiz_attempts_elapsed_ms",
                schema: "srs",
                table: "quiz_attempts",
                sql: "elapsed_ms IS NULL OR elapsed_ms >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_quiz_attempt_items_elapsed_ms",
                schema: "srs",
                table: "quiz_attempt_items",
                sql: "elapsed_ms IS NULL OR elapsed_ms >= 0");
        }
    }
}
