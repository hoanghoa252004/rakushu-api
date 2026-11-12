using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSrsAndCurationArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "srs");

            migrationBuilder.EnsureSchema(
                name: "curation");

            migrationBuilder.CreateTable(
                name: "issue_categories",
                schema: "curation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    current_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    threshold = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    last_reported_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "question_types",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "quizzes",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_questions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
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
                name: "reports",
                schema: "curation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    curator_notes = table.Column<string>(type: "text", nullable: true),
                    resolved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reports", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "saved_items",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_meaning_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expression = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    reading = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    meaning = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    saved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_saved_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_saved_items_knowledge_meanings_knowledge_meaning_id",
                        column: x => x.knowledge_meaning_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_meanings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_saved_items_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bank_questions",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_meaning_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    prompt = table.Column<string>(type: "text", nullable: false),
                    audio_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    choices_json = table.Column<string>(type: "jsonb", nullable: false),
                    correct_choice_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    explanation = table.Column<string>(type: "text", nullable: true),
                    difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Medium"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bank_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_bank_questions_knowledge_meanings_knowledge_meaning_id",
                        column: x => x.knowledge_meaning_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_meanings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bank_questions_question_types_question_type_id",
                        column: x => x.question_type_id,
                        principalSchema: "srs",
                        principalTable: "question_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quiz_attempts",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quiz_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    mode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    total_items = table.Column<int>(type: "integer", nullable: false),
                    answered_items = table.Column<int>(type: "integer", nullable: false),
                    correct_items = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quiz_attempts", x => x.id);
                    table.ForeignKey(
                        name: "fk_quiz_attempts_quizzes_quiz_id",
                        column: x => x.quiz_id,
                        principalSchema: "srs",
                        principalTable: "quizzes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quiz_attempts_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "issue_details",
                schema: "curation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    learner_feedback = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    report_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_issue_details_reports_report_id",
                        column: x => x.report_id,
                        principalSchema: "curation",
                        principalTable: "reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_issue_details_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "srs_schedules",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    saved_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_schedules", x => x.id);
                    table.ForeignKey(
                        name: "fk_srs_schedules_saved_items_saved_item_id",
                        column: x => x.saved_item_id,
                        principalSchema: "srs",
                        principalTable: "saved_items",
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
                    saved_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_question_id = table.Column<Guid>(type: "uuid", nullable: true),
                    question_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    prompt = table.Column<string>(type: "text", nullable: false),
                    reading = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    choices_json = table.Column<string>(type: "jsonb", nullable: false),
                    correct_option_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_questions_bank_questions_bank_question_id",
                        column: x => x.bank_question_id,
                        principalSchema: "srs",
                        principalTable: "bank_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_questions_question_types_question_type_id",
                        column: x => x.question_type_id,
                        principalSchema: "srs",
                        principalTable: "question_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_questions_quizzes_quiz_id",
                        column: x => x.quiz_id,
                        principalSchema: "srs",
                        principalTable: "quizzes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_questions_saved_items_saved_item_id",
                        column: x => x.saved_item_id,
                        principalSchema: "srs",
                        principalTable: "saved_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_attempt_items",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quiz_attempt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    selected_option_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_correct = table.Column<bool>(type: "boolean", nullable: true),
                    elapsed_ms = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    feedback = table.Column<string>(type: "text", nullable: true),
                    answered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quiz_attempt_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_quiz_attempt_items_questions_question_id",
                        column: x => x.question_id,
                        principalSchema: "srs",
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quiz_attempt_items_quiz_attempts_quiz_attempt_id",
                        column: x => x.quiz_attempt_id,
                        principalSchema: "srs",
                        principalTable: "quiz_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "srs_item_details",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    srs_schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quiz_attempt_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    review_number = table.Column<int>(type: "integer", nullable: false),
                    quality = table.Column<int>(type: "integer", nullable: false),
                    interval_days = table.Column<int>(type: "integer", nullable: false),
                    easiness_factor = table.Column<double>(type: "double precision", nullable: false),
                    repetitions = table.Column<int>(type: "integer", nullable: false),
                    lapses = table.Column<int>(type: "integer", nullable: false),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_item_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_srs_item_details_quiz_attempt_items_quiz_attempt_item_id",
                        column: x => x.quiz_attempt_item_id,
                        principalSchema: "srs",
                        principalTable: "quiz_attempt_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_srs_item_details_srs_schedules_srs_schedule_id",
                        column: x => x.srs_schedule_id,
                        principalSchema: "srs",
                        principalTable: "srs_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bank_questions_knowledge_meaning_id",
                schema: "srs",
                table: "bank_questions",
                column: "knowledge_meaning_id");

            migrationBuilder.CreateIndex(
                name: "ix_bank_questions_question_type_id",
                schema: "srs",
                table: "bank_questions",
                column: "question_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_categories_target_type_target_id_category",
                schema: "curation",
                table: "issue_categories",
                columns: new[] { "target_type", "target_id", "category" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_issue_details_report_id",
                schema: "curation",
                table: "issue_details",
                column: "report_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_details_target_type_target_id_category",
                schema: "curation",
                table: "issue_details",
                columns: new[] { "target_type", "target_id", "category" });

            migrationBuilder.CreateIndex(
                name: "ix_issue_details_user_id",
                schema: "curation",
                table: "issue_details",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_types_code",
                schema: "srs",
                table: "question_types",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_questions_bank_question_id",
                schema: "srs",
                table: "questions",
                column: "bank_question_id");

            migrationBuilder.CreateIndex(
                name: "ix_questions_question_type_id",
                schema: "srs",
                table: "questions",
                column: "question_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_questions_quiz_id",
                schema: "srs",
                table: "questions",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "ix_questions_saved_item_id",
                schema: "srs",
                table: "questions",
                column: "saved_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempt_items_question_id",
                schema: "srs",
                table: "quiz_attempt_items",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempt_items_quiz_attempt_id",
                schema: "srs",
                table: "quiz_attempt_items",
                column: "quiz_attempt_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_quiz_id",
                schema: "srs",
                table: "quiz_attempts",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_user_id",
                schema: "srs",
                table: "quiz_attempts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_quizzes_user_id",
                schema: "srs",
                table: "quizzes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_reports_status",
                schema: "curation",
                table: "reports",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_reports_target_type_target_id_category",
                schema: "curation",
                table: "reports",
                columns: new[] { "target_type", "target_id", "category" });

            migrationBuilder.CreateIndex(
                name: "ix_saved_items_knowledge_meaning_id",
                schema: "srs",
                table: "saved_items",
                column: "knowledge_meaning_id");

            migrationBuilder.CreateIndex(
                name: "ix_saved_items_user_id_knowledge_meaning_id",
                schema: "srs",
                table: "saved_items",
                columns: new[] { "user_id", "knowledge_meaning_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_item_details_quiz_attempt_item_id",
                schema: "srs",
                table: "srs_item_details",
                column: "quiz_attempt_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_srs_item_details_srs_schedule_id_reviewed_at",
                schema: "srs",
                table: "srs_item_details",
                columns: new[] { "srs_schedule_id", "reviewed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_srs_schedules_saved_item_id",
                schema: "srs",
                table: "srs_schedules",
                column: "saved_item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_schedules_status_due_at",
                schema: "srs",
                table: "srs_schedules",
                columns: new[] { "status", "due_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "issue_categories",
                schema: "curation");

            migrationBuilder.DropTable(
                name: "issue_details",
                schema: "curation");

            migrationBuilder.DropTable(
                name: "srs_item_details",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "reports",
                schema: "curation");

            migrationBuilder.DropTable(
                name: "quiz_attempt_items",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "srs_schedules",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "questions",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "quiz_attempts",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "bank_questions",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "saved_items",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "quizzes",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "question_types",
                schema: "srs");
        }
    }
}
