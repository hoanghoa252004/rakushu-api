using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaFieldsToQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "correct_answer",
                schema: "srs",
                table: "quiz_items",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "media_metadata",
                schema: "srs",
                table: "quiz_items",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "media_type",
                schema: "srs",
                table: "quiz_items",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "NONE");

            migrationBuilder.AddColumn<string>(
                name: "media_url",
                schema: "srs",
                table: "quiz_items",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "correct_answer",
                schema: "srs",
                table: "questions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "media_metadata",
                schema: "srs",
                table: "questions",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "media_type",
                schema: "srs",
                table: "questions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "NONE");

            migrationBuilder.AddColumn<string>(
                name: "media_url",
                schema: "srs",
                table: "questions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "media_metadata",
                schema: "srs",
                table: "quiz_items");

            migrationBuilder.DropColumn(
                name: "media_type",
                schema: "srs",
                table: "quiz_items");

            migrationBuilder.DropColumn(
                name: "media_url",
                schema: "srs",
                table: "quiz_items");

            migrationBuilder.DropColumn(
                name: "media_metadata",
                schema: "srs",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "media_type",
                schema: "srs",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "media_url",
                schema: "srs",
                table: "questions");

            migrationBuilder.AlterColumn<string>(
                name: "correct_answer",
                schema: "srs",
                table: "quiz_items",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "correct_answer",
                schema: "srs",
                table: "questions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);
        }
    }
}
