using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorQuizAttemptErd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "started_at",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "expires_at",
                schema: "srs",
                table: "quiz_attempts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now() + interval '1 hour'");

            migrationBuilder.AddColumn<bool>(
                name: "is_evaluated",
                schema: "srs",
                table: "quiz_attempts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.RenameColumn(
                name: "position",
                schema: "srs",
                table: "quiz_attempt_items",
                newName: "sequence");

            migrationBuilder.RenameColumn(
                name: "elapsed_ms",
                schema: "srs",
                table: "quiz_attempt_items",
                newName: "time_spent_ms");

            migrationBuilder.RenameColumn(
                name: "selected_option_id",
                schema: "srs",
                table: "quiz_attempt_items",
                newName: "selected_answer");

            migrationBuilder.AlterColumn<string>(
                name: "selected_answer",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "srs",
                table: "quiz_attempt_items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.DropColumn(
                name: "expires_at",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropColumn(
                name: "is_evaluated",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "started_at",
                schema: "srs",
                table: "quiz_attempts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "selected_answer",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.RenameColumn(
                name: "selected_answer",
                schema: "srs",
                table: "quiz_attempt_items",
                newName: "selected_option_id");

            migrationBuilder.RenameColumn(
                name: "time_spent_ms",
                schema: "srs",
                table: "quiz_attempt_items",
                newName: "elapsed_ms");

            migrationBuilder.RenameColumn(
                name: "sequence",
                schema: "srs",
                table: "quiz_attempt_items",
                newName: "position");
        }
    }
}
