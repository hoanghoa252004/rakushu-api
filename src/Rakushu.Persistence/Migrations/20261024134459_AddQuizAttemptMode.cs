using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizAttemptMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "mode",
                schema: "srs",
                table: "quiz_attempts",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "SrsReview");

            migrationBuilder.AddCheckConstraint(
                name: "ck_quiz_attempts_mode",
                schema: "srs",
                table: "quiz_attempts",
                sql: "mode IN ('SrsReview', 'PracticeAll', 'PracticeMistakes')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_quiz_attempts_mode",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropColumn(
                name: "mode",
                schema: "srs",
                table: "quiz_attempts");
        }
    }
}
