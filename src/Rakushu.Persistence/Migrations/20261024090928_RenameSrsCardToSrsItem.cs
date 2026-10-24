using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameSrsCardToSrsItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_questions_srs_cards_srs_card_id",
                schema: "srs",
                table: "questions");

            migrationBuilder.DropForeignKey(
                name: "fk_quiz_attempt_items_srs_cards_srs_card_id",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropTable(
                name: "srs_cards",
                schema: "srs");

            migrationBuilder.RenameColumn(
                name: "srs_card_id",
                schema: "srs",
                table: "quiz_attempt_items",
                newName: "srs_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_quiz_attempt_items_srs_card_id",
                schema: "srs",
                table: "quiz_attempt_items",
                newName: "ix_quiz_attempt_items_srs_item_id");

            migrationBuilder.RenameColumn(
                name: "srs_card_id",
                schema: "srs",
                table: "questions",
                newName: "srs_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_questions_srs_card_id",
                schema: "srs",
                table: "questions",
                newName: "ix_questions_srs_item_id");

            migrationBuilder.CreateTable(
                name: "srs_items",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_meaning_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_learning_unit_id = table.Column<Guid>(type: "uuid", nullable: true),
                    expression = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    reading = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    meaning = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "New"),
                    due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    interval_days = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    easiness_factor = table.Column<double>(type: "double precision", nullable: false, defaultValue: 2.5),
                    repetitions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    lapses = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    last_review_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_items", x => x.id);
                    table.CheckConstraint("ck_srs_items_status", "status IN ('New', 'Learning', 'Review', 'Suspended', 'Ignored')");
                    table.ForeignKey(
                        name: "fk_srs_items_knowledge_meanings_knowledge_meaning_id",
                        column: x => x.knowledge_meaning_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_meanings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_srs_items_learning_units_source_learning_unit_id",
                        column: x => x.source_learning_unit_id,
                        principalSchema: "material",
                        principalTable: "learning_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_srs_items_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_srs_items_due_at",
                schema: "srs",
                table: "srs_items",
                column: "due_at");

            migrationBuilder.CreateIndex(
                name: "ix_srs_items_knowledge_meaning_id",
                schema: "srs",
                table: "srs_items",
                column: "knowledge_meaning_id");

            migrationBuilder.CreateIndex(
                name: "ix_srs_items_source_learning_unit_id",
                schema: "srs",
                table: "srs_items",
                column: "source_learning_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_srs_items_user_id_knowledge_meaning_id",
                schema: "srs",
                table: "srs_items",
                columns: new[] { "user_id", "knowledge_meaning_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_items_user_id_status_due_at",
                schema: "srs",
                table: "srs_items",
                columns: new[] { "user_id", "status", "due_at" });

            migrationBuilder.AddForeignKey(
                name: "fk_questions_srs_items_srs_item_id",
                schema: "srs",
                table: "questions",
                column: "srs_item_id",
                principalSchema: "srs",
                principalTable: "srs_items",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_quiz_attempt_items_srs_items_srs_item_id",
                schema: "srs",
                table: "quiz_attempt_items",
                column: "srs_item_id",
                principalSchema: "srs",
                principalTable: "srs_items",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_questions_srs_items_srs_item_id",
                schema: "srs",
                table: "questions");

            migrationBuilder.DropForeignKey(
                name: "fk_quiz_attempt_items_srs_items_srs_item_id",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropTable(
                name: "srs_items",
                schema: "srs");

            migrationBuilder.RenameColumn(
                name: "srs_item_id",
                schema: "srs",
                table: "quiz_attempt_items",
                newName: "srs_card_id");

            migrationBuilder.RenameIndex(
                name: "ix_quiz_attempt_items_srs_item_id",
                schema: "srs",
                table: "quiz_attempt_items",
                newName: "ix_quiz_attempt_items_srs_card_id");

            migrationBuilder.RenameColumn(
                name: "srs_item_id",
                schema: "srs",
                table: "questions",
                newName: "srs_card_id");

            migrationBuilder.RenameIndex(
                name: "ix_questions_srs_item_id",
                schema: "srs",
                table: "questions",
                newName: "ix_questions_srs_card_id");

            migrationBuilder.CreateTable(
                name: "srs_cards",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_meaning_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_learning_unit_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    easiness_factor = table.Column<double>(type: "double precision", nullable: false, defaultValue: 2.5),
                    expression = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    interval_days = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    lapses = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    last_review_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    meaning = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    reading = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    repetitions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "New"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_cards", x => x.id);
                    table.CheckConstraint("ck_srs_cards_status", "status IN ('New', 'Learning', 'Review', 'Suspended', 'Ignored')");
                    table.ForeignKey(
                        name: "fk_srs_cards_knowledge_meanings_knowledge_meaning_id",
                        column: x => x.knowledge_meaning_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_meanings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_srs_cards_learning_units_source_learning_unit_id",
                        column: x => x.source_learning_unit_id,
                        principalSchema: "material",
                        principalTable: "learning_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_srs_cards_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_srs_cards_due_at",
                schema: "srs",
                table: "srs_cards",
                column: "due_at");

            migrationBuilder.CreateIndex(
                name: "ix_srs_cards_knowledge_meaning_id",
                schema: "srs",
                table: "srs_cards",
                column: "knowledge_meaning_id");

            migrationBuilder.CreateIndex(
                name: "ix_srs_cards_source_learning_unit_id",
                schema: "srs",
                table: "srs_cards",
                column: "source_learning_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_srs_cards_user_id_knowledge_meaning_id",
                schema: "srs",
                table: "srs_cards",
                columns: new[] { "user_id", "knowledge_meaning_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_cards_user_id_status_due_at",
                schema: "srs",
                table: "srs_cards",
                columns: new[] { "user_id", "status", "due_at" });

            migrationBuilder.AddForeignKey(
                name: "fk_questions_srs_cards_srs_card_id",
                schema: "srs",
                table: "questions",
                column: "srs_card_id",
                principalSchema: "srs",
                principalTable: "srs_cards",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_quiz_attempt_items_srs_cards_srs_card_id",
                schema: "srs",
                table: "quiz_attempt_items",
                column: "srs_card_id",
                principalSchema: "srs",
                principalTable: "srs_cards",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
