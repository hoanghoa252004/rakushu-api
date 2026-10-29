using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFlashcardFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "flashcard_sessions",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flashcard_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_flashcard_sessions_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "flashcard_session_items",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    flashcard_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    saved_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expression = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    reading = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    meaning = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: true),
                    video_title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: true),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: true),
                    original_sentence = table.Column<string>(type: "text", nullable: true),
                    translated_sentence = table.Column<string>(type: "text", nullable: true),
                    rating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    quality = table.Column<int>(type: "integer", nullable: true),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flashcard_session_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_flashcard_session_items_flashcard_sessions_flashcard_sessio",
                        column: x => x.flashcard_session_id,
                        principalSchema: "srs",
                        principalTable: "flashcard_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_flashcard_session_items_saved_items_saved_item_id",
                        column: x => x.saved_item_id,
                        principalSchema: "srs",
                        principalTable: "saved_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_flashcard_session_items_flashcard_session_id",
                schema: "srs",
                table: "flashcard_session_items",
                column: "flashcard_session_id");

            migrationBuilder.CreateIndex(
                name: "ix_flashcard_session_items_saved_item_id",
                schema: "srs",
                table: "flashcard_session_items",
                column: "saved_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_flashcard_sessions_user_id",
                schema: "srs",
                table: "flashcard_sessions",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "flashcard_session_items",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "flashcard_sessions",
                schema: "srs");
        }
    }
}
