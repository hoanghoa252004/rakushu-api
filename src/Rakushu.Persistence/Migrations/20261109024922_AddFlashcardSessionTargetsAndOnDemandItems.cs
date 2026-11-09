using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFlashcardSessionTargetsAndOnDemandItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "reviewed_at",
                schema: "srs",
                table: "flashcard_session_items",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "rating",
                schema: "srs",
                table: "flashcard_session_items",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "flashcard_session_targets",
                schema: "srs",
                columns: table => new
                {
                    flashcard_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    saved_item_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flashcard_session_targets", x => new { x.flashcard_session_id, x.saved_item_id });
                    table.ForeignKey(
                        name: "fk_flashcard_session_targets_flashcard_sessions_flashcard_sess",
                        column: x => x.flashcard_session_id,
                        principalSchema: "srs",
                        principalTable: "flashcard_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_flashcard_session_targets_saved_items_saved_item_id",
                        column: x => x.saved_item_id,
                        principalSchema: "srs",
                        principalTable: "saved_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_flashcard_session_targets_saved_item_id",
                schema: "srs",
                table: "flashcard_session_targets",
                column: "saved_item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "flashcard_session_targets",
                schema: "srs");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "reviewed_at",
                schema: "srs",
                table: "flashcard_session_items",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "rating",
                schema: "srs",
                table: "flashcard_session_items",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }
    }
}
