using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSrsQuizSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quiz_attempts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_attempt_id = table.Column<Guid>(type: "uuid", nullable: true),
                    template_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "WORD_TO_MEANING_MCQ"),
                    attempt_mode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Practice"),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Created"),
                    total_items = table.Column<int>(type: "integer", nullable: false),
                    answered_items = table.Column<int>(type: "integer", nullable: false),
                    correct_items = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    elapsed_ms = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quiz_attempts", x => x.id);
                    table.CheckConstraint("ck_quiz_attempts_completed_at", "status <> 'Completed' OR completed_at IS NOT NULL");
                    table.CheckConstraint("ck_quiz_attempts_counts", "total_items >= 0 AND answered_items >= 0 AND correct_items >= 0 AND correct_items <= answered_items AND answered_items <= total_items");
                    table.CheckConstraint("ck_quiz_attempts_elapsed_ms", "elapsed_ms IS NULL OR elapsed_ms >= 0");
                    table.CheckConstraint("ck_quiz_attempts_mode", "attempt_mode IN ('SrsReview', 'Practice')");
                    table.CheckConstraint("ck_quiz_attempts_score", "score IS NULL OR (score >= 0 AND score <= 100)");
                    table.CheckConstraint("ck_quiz_attempts_status", "status IN ('Created', 'InProgress', 'Completed', 'Abandoned')");
                    table.ForeignKey(
                        name: "fk_quiz_attempts_quiz_attempts_source_attempt_id",
                        column: x => x.source_attempt_id,
                        principalTable: "quiz_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_quiz_attempts_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "srs_notes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    learning_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    archived_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_notes", x => x.id);
                    table.ForeignKey(
                        name: "fk_srs_notes_learning_units_learning_unit_id",
                        column: x => x.learning_unit_id,
                        principalSchema: "material",
                        principalTable: "learning_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_srs_notes_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "srs_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "InProgress"),
                    total_items = table.Column<int>(type: "integer", nullable: false),
                    reviewed_items = table.Column<int>(type: "integer", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_sessions", x => x.id);
                    table.CheckConstraint("ck_srs_sessions_completed_at", "status <> 'Completed' OR completed_at IS NOT NULL");
                    table.CheckConstraint("ck_srs_sessions_counts", "reviewed_items >= 0 AND total_items >= 0 AND reviewed_items <= total_items");
                    table.CheckConstraint("ck_srs_sessions_status", "status IN ('InProgress', 'Completed', 'Abandoned')");
                    table.ForeignKey(
                        name: "fk_srs_sessions_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "srs_cards",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    srs_note_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "New"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_cards", x => x.id);
                    table.CheckConstraint("ck_srs_cards_status", "status IN ('New', 'Learning', 'Review', 'Suspended', 'Archived')");
                    table.ForeignKey(
                        name: "fk_srs_cards_srs_notes_srs_note_id",
                        column: x => x.srs_note_id,
                        principalTable: "srs_notes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_attempt_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quiz_attempt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    srs_card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    question_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "WORD_TO_MEANING_MCQ"),
                    question_snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    choices_snapshot = table.Column<string>(type: "jsonb", nullable: true),
                    correct_answer_snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    selected_answer = table.Column<string>(type: "jsonb", nullable: true),
                    is_correct = table.Column<bool>(type: "boolean", nullable: true),
                    feedback_snapshot = table.Column<string>(type: "jsonb", nullable: true),
                    answered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    elapsed_ms = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quiz_attempt_items", x => x.id);
                    table.CheckConstraint("ck_quiz_attempt_items_elapsed_ms", "elapsed_ms IS NULL OR elapsed_ms >= 0");
                    table.CheckConstraint("ck_quiz_attempt_items_position", "position > 0");
                    table.ForeignKey(
                        name: "fk_quiz_attempt_items_quiz_attempts_quiz_attempt_id",
                        column: x => x.quiz_attempt_id,
                        principalTable: "quiz_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quiz_attempt_items_srs_cards_srs_card_id",
                        column: x => x.srs_card_id,
                        principalTable: "srs_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "srs_card_states",
                columns: table => new
                {
                    srs_card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_review_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    repetitions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    lapses = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    easiness_factor = table.Column<double>(type: "double precision", nullable: false, defaultValue: 2.5),
                    interval_days = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_card_states", x => x.srs_card_id);
                    table.CheckConstraint("ck_srs_card_states_easiness_factor", "easiness_factor >= 1.3");
                    table.CheckConstraint("ck_srs_card_states_interval_days", "interval_days >= 0");
                    table.CheckConstraint("ck_srs_card_states_lapses", "lapses >= 0");
                    table.CheckConstraint("ck_srs_card_states_repetitions", "repetitions >= 0");
                    table.ForeignKey(
                        name: "fk_srs_card_states_srs_cards_srs_card_id",
                        column: x => x.srs_card_id,
                        principalTable: "srs_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "srs_session_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    srs_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    srs_card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    shown_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    elapsed_ms = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_session_items", x => x.id);
                    table.CheckConstraint("ck_srs_session_items_elapsed_ms", "elapsed_ms IS NULL OR elapsed_ms >= 0");
                    table.CheckConstraint("ck_srs_session_items_position", "position > 0");
                    table.ForeignKey(
                        name: "fk_srs_session_items_srs_cards_srs_card_id",
                        column: x => x.srs_card_id,
                        principalTable: "srs_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_srs_session_items_srs_sessions_srs_session_id",
                        column: x => x.srs_session_id,
                        principalTable: "srs_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "srs_review_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    srs_card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    srs_session_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    quiz_attempt_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    elapsed_ms = table.Column<int>(type: "integer", nullable: true),
                    previous_due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    next_due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    previous_interval_days = table.Column<int>(type: "integer", nullable: true),
                    next_interval_days = table.Column<int>(type: "integer", nullable: true),
                    previous_easiness_factor = table.Column<double>(type: "double precision", nullable: true),
                    next_easiness_factor = table.Column<double>(type: "double precision", nullable: true),
                    previous_repetitions = table.Column<int>(type: "integer", nullable: true),
                    next_repetitions = table.Column<int>(type: "integer", nullable: true),
                    scheduler_version = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "SM2-v1"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_review_logs", x => x.id);
                    table.CheckConstraint("ck_srs_review_logs_elapsed_ms", "elapsed_ms IS NULL OR elapsed_ms >= 0");
                    table.CheckConstraint("ck_srs_review_logs_single_source", "(srs_session_item_id IS NOT NULL AND quiz_attempt_item_id IS NULL) OR (srs_session_item_id IS NULL AND quiz_attempt_item_id IS NOT NULL)");
                    table.ForeignKey(
                        name: "fk_srs_review_logs_quiz_attempt_items_quiz_attempt_item_id",
                        column: x => x.quiz_attempt_item_id,
                        principalTable: "quiz_attempt_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_srs_review_logs_srs_cards_srs_card_id",
                        column: x => x.srs_card_id,
                        principalTable: "srs_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_srs_review_logs_srs_session_items_srs_session_item_id",
                        column: x => x.srs_session_item_id,
                        principalTable: "srs_session_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempt_items_quiz_attempt_id_position",
                table: "quiz_attempt_items",
                columns: new[] { "quiz_attempt_id", "position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempt_items_srs_card_id",
                table: "quiz_attempt_items",
                column: "srs_card_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_source_attempt_id",
                table: "quiz_attempts",
                column: "source_attempt_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_user_id_started_at",
                table: "quiz_attempts",
                columns: new[] { "user_id", "started_at" });

            migrationBuilder.CreateIndex(
                name: "ix_srs_card_states_due_at",
                table: "srs_card_states",
                column: "due_at");

            migrationBuilder.CreateIndex(
                name: "ix_srs_cards_srs_note_id",
                table: "srs_cards",
                column: "srs_note_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_cards_status",
                table: "srs_cards",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_srs_notes_learning_unit_id",
                table: "srs_notes",
                column: "learning_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_srs_notes_user_id_created_at",
                table: "srs_notes",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_srs_notes_user_id_learning_unit_id",
                table: "srs_notes",
                columns: new[] { "user_id", "learning_unit_id" });

            migrationBuilder.CreateIndex(
                name: "ix_srs_review_logs_quiz_attempt_item_id",
                table: "srs_review_logs",
                column: "quiz_attempt_item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_review_logs_srs_card_id_reviewed_at",
                table: "srs_review_logs",
                columns: new[] { "srs_card_id", "reviewed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_srs_review_logs_srs_session_item_id",
                table: "srs_review_logs",
                column: "srs_session_item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_session_items_srs_card_id",
                table: "srs_session_items",
                column: "srs_card_id");

            migrationBuilder.CreateIndex(
                name: "ix_srs_session_items_srs_session_id_position",
                table: "srs_session_items",
                columns: new[] { "srs_session_id", "position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_sessions_user_id_started_at",
                table: "srs_sessions",
                columns: new[] { "user_id", "started_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "srs_card_states");

            migrationBuilder.DropTable(
                name: "srs_review_logs");

            migrationBuilder.DropTable(
                name: "quiz_attempt_items");

            migrationBuilder.DropTable(
                name: "srs_session_items");

            migrationBuilder.DropTable(
                name: "quiz_attempts");

            migrationBuilder.DropTable(
                name: "srs_cards");

            migrationBuilder.DropTable(
                name: "srs_sessions");

            migrationBuilder.DropTable(
                name: "srs_notes");
        }
    }
}
