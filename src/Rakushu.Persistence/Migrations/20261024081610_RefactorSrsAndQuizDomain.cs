using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSrsAndQuizDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_quiz_attempts_quiz_attempts_source_attempt_id",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropForeignKey(
                name: "fk_quiz_attempts_users_user_id",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropForeignKey(
                name: "fk_srs_cards_srs_notes_srs_note_id",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropTable(
                name: "srs_card_states",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "srs_notes",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "srs_review_logs",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "srs_session_items",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "srs_sessions",
                schema: "srs");

            migrationBuilder.DropIndex(
                name: "ix_srs_cards_srs_note_id",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropIndex(
                name: "ix_srs_cards_status",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropCheckConstraint(
                name: "ck_srs_cards_status",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropIndex(
                name: "ix_quiz_attempts_source_attempt_id",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropCheckConstraint(
                name: "ck_quiz_attempts_mode",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropColumn(
                name: "attempt_mode",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropColumn(
                name: "source_attempt_id",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropColumn(
                name: "template_code",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropColumn(
                name: "choices_snapshot",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropColumn(
                name: "correct_answer_snapshot",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropColumn(
                name: "feedback_snapshot",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropColumn(
                name: "question_snapshot",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropColumn(
                name: "question_type",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.RenameColumn(
                name: "srs_note_id",
                schema: "srs",
                table: "srs_cards",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "selected_answer",
                schema: "srs",
                table: "quiz_attempt_items",
                newName: "feedback");

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "srs",
                table: "srs_cards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "due_at",
                schema: "srs",
                table: "srs_cards",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<double>(
                name: "easiness_factor",
                schema: "srs",
                table: "srs_cards",
                type: "double precision",
                nullable: false,
                defaultValue: 2.5);

            migrationBuilder.AddColumn<string>(
                name: "expression",
                schema: "srs",
                table: "srs_cards",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "interval_days",
                schema: "srs",
                table: "srs_cards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "knowledge_meaning_id",
                schema: "srs",
                table: "srs_cards",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "lapses",
                schema: "srs",
                table: "srs_cards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_review_at",
                schema: "srs",
                table: "srs_cards",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "meaning",
                schema: "srs",
                table: "srs_cards",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "reading",
                schema: "srs",
                table: "srs_cards",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "repetitions",
                schema: "srs",
                table: "srs_cards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "source_learning_unit_id",
                schema: "srs",
                table: "srs_cards",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "srs",
                table: "quiz_attempts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "InProgress",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Created");

            migrationBuilder.AddColumn<Guid>(
                name: "quiz_id",
                schema: "srs",
                table: "quiz_attempts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "elapsed_ms",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "quality",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "question_id",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "selected_option_id",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "quizzes",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_questions = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quizzes", x => x.id);
                    table.ForeignKey(
                        name: "fk_quizzes_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quiz_id = table.Column<Guid>(type: "uuid", nullable: false),
                    srs_card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    question_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    prompt = table.Column<string>(type: "text", nullable: false),
                    reading = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    choices = table.Column<string>(type: "jsonb", nullable: false),
                    correct_option_id = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_questions_quizzes_quiz_id",
                        column: x => x.quiz_id,
                        principalSchema: "srs",
                        principalTable: "quizzes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_questions_srs_cards_srs_card_id",
                        column: x => x.srs_card_id,
                        principalSchema: "srs",
                        principalTable: "srs_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.AddCheckConstraint(
                name: "ck_srs_cards_status",
                schema: "srs",
                table: "srs_cards",
                sql: "status IN ('New', 'Learning', 'Review', 'Suspended', 'Ignored')");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_quiz_id",
                schema: "srs",
                table: "quiz_attempts",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempt_items_question_id",
                schema: "srs",
                table: "quiz_attempt_items",
                column: "question_id");

            migrationBuilder.AddCheckConstraint(
                name: "ck_quiz_attempt_items_quality",
                schema: "srs",
                table: "quiz_attempt_items",
                sql: "quality IS NULL OR (quality >= 0 AND quality <= 5)");

            migrationBuilder.CreateIndex(
                name: "ix_questions_quiz_id_position",
                schema: "srs",
                table: "questions",
                columns: new[] { "quiz_id", "position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_questions_srs_card_id",
                schema: "srs",
                table: "questions",
                column: "srs_card_id");

            migrationBuilder.CreateIndex(
                name: "ix_quizzes_user_id",
                schema: "srs",
                table: "quizzes",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_quiz_attempt_items_questions_question_id",
                schema: "srs",
                table: "quiz_attempt_items",
                column: "question_id",
                principalSchema: "srs",
                principalTable: "questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_quiz_attempts_quizzes_quiz_id",
                schema: "srs",
                table: "quiz_attempts",
                column: "quiz_id",
                principalSchema: "srs",
                principalTable: "quizzes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_quiz_attempts_users_user_id",
                schema: "srs",
                table: "quiz_attempts",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_srs_cards_knowledge_meanings_knowledge_meaning_id",
                schema: "srs",
                table: "srs_cards",
                column: "knowledge_meaning_id",
                principalSchema: "knowledge",
                principalTable: "knowledge_meanings",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_srs_cards_learning_units_source_learning_unit_id",
                schema: "srs",
                table: "srs_cards",
                column: "source_learning_unit_id",
                principalSchema: "material",
                principalTable: "learning_units",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_srs_cards_users_user_id",
                schema: "srs",
                table: "srs_cards",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_quiz_attempt_items_questions_question_id",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropForeignKey(
                name: "fk_quiz_attempts_quizzes_quiz_id",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropForeignKey(
                name: "fk_quiz_attempts_users_user_id",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropForeignKey(
                name: "fk_srs_cards_knowledge_meanings_knowledge_meaning_id",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropForeignKey(
                name: "fk_srs_cards_learning_units_source_learning_unit_id",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropForeignKey(
                name: "fk_srs_cards_users_user_id",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropTable(
                name: "questions",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "quizzes",
                schema: "srs");

            migrationBuilder.DropIndex(
                name: "ix_srs_cards_due_at",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropIndex(
                name: "ix_srs_cards_knowledge_meaning_id",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropIndex(
                name: "ix_srs_cards_source_learning_unit_id",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropIndex(
                name: "ix_srs_cards_user_id_knowledge_meaning_id",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropIndex(
                name: "ix_srs_cards_user_id_status_due_at",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropCheckConstraint(
                name: "ck_srs_cards_status",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropIndex(
                name: "ix_quiz_attempts_quiz_id",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropIndex(
                name: "ix_quiz_attempt_items_question_id",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropCheckConstraint(
                name: "ck_quiz_attempt_items_quality",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropColumn(
                name: "due_at",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropColumn(
                name: "easiness_factor",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropColumn(
                name: "expression",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropColumn(
                name: "interval_days",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropColumn(
                name: "knowledge_meaning_id",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropColumn(
                name: "lapses",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropColumn(
                name: "last_review_at",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropColumn(
                name: "meaning",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropColumn(
                name: "reading",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropColumn(
                name: "repetitions",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropColumn(
                name: "source_learning_unit_id",
                schema: "srs",
                table: "srs_cards");

            migrationBuilder.DropColumn(
                name: "quiz_id",
                schema: "srs",
                table: "quiz_attempts");

            migrationBuilder.DropColumn(
                name: "elapsed_ms",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropColumn(
                name: "quality",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropColumn(
                name: "question_id",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropColumn(
                name: "selected_option_id",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "srs",
                table: "srs_cards",
                newName: "srs_note_id");

            migrationBuilder.RenameColumn(
                name: "feedback",
                schema: "srs",
                table: "quiz_attempt_items",
                newName: "selected_answer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "srs",
                table: "quiz_attempts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Created",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "InProgress");

            migrationBuilder.AddColumn<string>(
                name: "attempt_mode",
                schema: "srs",
                table: "quiz_attempts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Practice");

            migrationBuilder.AddColumn<Guid>(
                name: "source_attempt_id",
                schema: "srs",
                table: "quiz_attempts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "template_code",
                schema: "srs",
                table: "quiz_attempts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "WORD_TO_MEANING_MCQ");

            migrationBuilder.AddColumn<string>(
                name: "choices_snapshot",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "correct_answer_snapshot",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "feedback_snapshot",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "question_snapshot",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "question_type",
                schema: "srs",
                table: "quiz_attempt_items",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "WORD_TO_MEANING_MCQ");

            migrationBuilder.CreateTable(
                name: "srs_card_states",
                schema: "srs",
                columns: table => new
                {
                    srs_card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    easiness_factor = table.Column<double>(type: "double precision", nullable: false, defaultValue: 2.5),
                    interval_days = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    lapses = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    last_review_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    repetitions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
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
                        principalSchema: "srs",
                        principalTable: "srs_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "srs_notes",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    learning_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    archived_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
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
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    reviewed_items = table.Column<int>(type: "integer", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "InProgress"),
                    total_items = table.Column<int>(type: "integer", nullable: false),
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
                name: "srs_session_items",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    srs_card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    srs_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    elapsed_ms = table.Column<int>(type: "integer", nullable: true),
                    position = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    shown_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_session_items", x => x.id);
                    table.CheckConstraint("ck_srs_session_items_elapsed_ms", "elapsed_ms IS NULL OR elapsed_ms >= 0");
                    table.CheckConstraint("ck_srs_session_items_position", "position > 0");
                    table.ForeignKey(
                        name: "fk_srs_session_items_srs_cards_srs_card_id",
                        column: x => x.srs_card_id,
                        principalSchema: "srs",
                        principalTable: "srs_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_srs_session_items_srs_sessions_srs_session_id",
                        column: x => x.srs_session_id,
                        principalSchema: "srs",
                        principalTable: "srs_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "srs_review_logs",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    srs_card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    elapsed_ms = table.Column<int>(type: "integer", nullable: true),
                    next_due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    next_easiness_factor = table.Column<double>(type: "double precision", nullable: true),
                    next_interval_days = table.Column<int>(type: "integer", nullable: true),
                    next_repetitions = table.Column<int>(type: "integer", nullable: true),
                    previous_due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    previous_easiness_factor = table.Column<double>(type: "double precision", nullable: true),
                    previous_interval_days = table.Column<int>(type: "integer", nullable: true),
                    previous_repetitions = table.Column<int>(type: "integer", nullable: true),
                    quality = table.Column<int>(type: "integer", nullable: false),
                    quiz_attempt_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    scheduler_version = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "SM2-v1"),
                    srs_session_item_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_review_logs", x => x.id);
                    table.CheckConstraint("ck_srs_review_logs_elapsed_ms", "elapsed_ms IS NULL OR elapsed_ms >= 0");
                    table.CheckConstraint("ck_srs_review_logs_quality", "quality BETWEEN 0 AND 5");
                    table.CheckConstraint("ck_srs_review_logs_single_source", "(srs_session_item_id IS NOT NULL AND quiz_attempt_item_id IS NULL) OR (srs_session_item_id IS NULL AND quiz_attempt_item_id IS NOT NULL)");
                    table.ForeignKey(
                        name: "fk_srs_review_logs_quiz_attempt_items_quiz_attempt_item_id",
                        column: x => x.quiz_attempt_item_id,
                        principalSchema: "srs",
                        principalTable: "quiz_attempt_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_srs_review_logs_srs_cards_srs_card_id",
                        column: x => x.srs_card_id,
                        principalSchema: "srs",
                        principalTable: "srs_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_srs_review_logs_srs_session_items_srs_session_item_id",
                        column: x => x.srs_session_item_id,
                        principalSchema: "srs",
                        principalTable: "srs_session_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_srs_cards_srs_note_id",
                schema: "srs",
                table: "srs_cards",
                column: "srs_note_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_cards_status",
                schema: "srs",
                table: "srs_cards",
                column: "status");

            migrationBuilder.AddCheckConstraint(
                name: "ck_srs_cards_status",
                schema: "srs",
                table: "srs_cards",
                sql: "status IN ('New', 'Learning', 'Review', 'Suspended', 'Archived')");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_source_attempt_id",
                schema: "srs",
                table: "quiz_attempts",
                column: "source_attempt_id");

            migrationBuilder.AddCheckConstraint(
                name: "ck_quiz_attempts_mode",
                schema: "srs",
                table: "quiz_attempts",
                sql: "attempt_mode IN ('SrsReview', 'Practice')");

            migrationBuilder.CreateIndex(
                name: "ix_srs_card_states_due_at",
                schema: "srs",
                table: "srs_card_states",
                column: "due_at");

            migrationBuilder.CreateIndex(
                name: "ix_srs_notes_learning_unit_id",
                schema: "srs",
                table: "srs_notes",
                column: "learning_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_srs_notes_user_id_created_at",
                schema: "srs",
                table: "srs_notes",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_srs_notes_user_id_learning_unit_id",
                schema: "srs",
                table: "srs_notes",
                columns: new[] { "user_id", "learning_unit_id" });

            migrationBuilder.CreateIndex(
                name: "ix_srs_review_logs_quiz_attempt_item_id",
                schema: "srs",
                table: "srs_review_logs",
                column: "quiz_attempt_item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_review_logs_srs_card_id_reviewed_at",
                schema: "srs",
                table: "srs_review_logs",
                columns: new[] { "srs_card_id", "reviewed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_srs_review_logs_srs_session_item_id",
                schema: "srs",
                table: "srs_review_logs",
                column: "srs_session_item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_session_items_srs_card_id",
                schema: "srs",
                table: "srs_session_items",
                column: "srs_card_id");

            migrationBuilder.CreateIndex(
                name: "ix_srs_session_items_srs_session_id_position",
                schema: "srs",
                table: "srs_session_items",
                columns: new[] { "srs_session_id", "position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_session_items_srs_session_id_srs_card_id",
                schema: "srs",
                table: "srs_session_items",
                columns: new[] { "srs_session_id", "srs_card_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_sessions_user_id_started_at",
                schema: "srs",
                table: "srs_sessions",
                columns: new[] { "user_id", "started_at" });

            migrationBuilder.AddForeignKey(
                name: "fk_quiz_attempts_quiz_attempts_source_attempt_id",
                schema: "srs",
                table: "quiz_attempts",
                column: "source_attempt_id",
                principalSchema: "srs",
                principalTable: "quiz_attempts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_quiz_attempts_users_user_id",
                schema: "srs",
                table: "quiz_attempts",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_srs_cards_srs_notes_srs_note_id",
                schema: "srs",
                table: "srs_cards",
                column: "srs_note_id",
                principalSchema: "srs",
                principalTable: "srs_notes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
