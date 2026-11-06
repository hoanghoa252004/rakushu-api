using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesAndColumnsForQuestionsAndQuizItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop foreign keys referencing or owned by old questions and bank_questions
            migrationBuilder.DropForeignKey(
                name: "fk_quiz_attempt_items_questions_question_id",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropForeignKey(
                name: "fk_questions_bank_questions_bank_question_id",
                schema: "srs",
                table: "questions");

            migrationBuilder.DropForeignKey(
                name: "fk_questions_quizzes_quiz_id",
                schema: "srs",
                table: "questions");

            migrationBuilder.DropForeignKey(
                name: "fk_questions_saved_items_saved_item_id",
                schema: "srs",
                table: "questions");

            migrationBuilder.DropForeignKey(
                name: "fk_questions_question_types_question_type_id",
                schema: "srs",
                table: "questions");

            migrationBuilder.DropForeignKey(
                name: "fk_bank_questions_knowledge_meanings_knowledge_meaning_id",
                schema: "srs",
                table: "bank_questions");

            migrationBuilder.DropForeignKey(
                name: "fk_bank_questions_question_types_question_type_id",
                schema: "srs",
                table: "bank_questions");

            // 2. Rename tables safely (first questions -> quiz_items, then bank_questions -> questions)
            migrationBuilder.RenameTable(
                name: "questions",
                schema: "srs",
                newName: "quiz_items",
                newSchema: "srs");

            migrationBuilder.RenameTable(
                name: "bank_questions",
                schema: "srs",
                newName: "questions",
                newSchema: "srs");

            // 3. Rename columns on srs.questions (formerly bank_questions)
            migrationBuilder.RenameColumn(
                name: "prompt",
                schema: "srs",
                table: "questions",
                newName: "question");

            migrationBuilder.RenameColumn(
                name: "choices_json",
                schema: "srs",
                table: "questions",
                newName: "answers");

            migrationBuilder.RenameColumn(
                name: "correct_choice_id",
                schema: "srs",
                table: "questions",
                newName: "correct_answer");

            // 4. Update columns on srs.quiz_items (formerly questions)
            migrationBuilder.RenameColumn(
                name: "position",
                schema: "srs",
                table: "quiz_items",
                newName: "sequence");

            migrationBuilder.RenameColumn(
                name: "prompt",
                schema: "srs",
                table: "quiz_items",
                newName: "question");

            migrationBuilder.RenameColumn(
                name: "choices_json",
                schema: "srs",
                table: "quiz_items",
                newName: "answers");

            migrationBuilder.RenameColumn(
                name: "correct_option_id",
                schema: "srs",
                table: "quiz_items",
                newName: "correct_answer");

            // Rename FK column bank_question_id -> question_id
            migrationBuilder.RenameColumn(
                name: "bank_question_id",
                schema: "srs",
                table: "quiz_items",
                newName: "question_id");

            // Drop reading, created_at, and redundant question_type_id from quiz_items
            migrationBuilder.DropColumn(
                name: "reading",
                schema: "srs",
                table: "quiz_items");

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "srs",
                table: "quiz_items");

            migrationBuilder.DropIndex(
                name: "ix_questions_question_type_id",
                schema: "srs",
                table: "quiz_items");

            migrationBuilder.DropColumn(
                name: "question_type_id",
                schema: "srs",
                table: "quiz_items");

            // 5. Update primary keys & indexes
            // On quiz_items:
            migrationBuilder.DropPrimaryKey(
                name: "pk_questions",
                schema: "srs",
                table: "quiz_items");

            migrationBuilder.AddPrimaryKey(
                name: "pk_quiz_items",
                schema: "srs",
                table: "quiz_items",
                column: "id");

            migrationBuilder.RenameIndex(
                name: "ix_questions_bank_question_id",
                schema: "srs",
                table: "quiz_items",
                newName: "ix_quiz_items_question_id");

            migrationBuilder.RenameIndex(
                name: "ix_questions_quiz_id",
                schema: "srs",
                table: "quiz_items",
                newName: "ix_quiz_items_quiz_id");

            migrationBuilder.RenameIndex(
                name: "ix_questions_saved_item_id",
                schema: "srs",
                table: "quiz_items",
                newName: "ix_quiz_items_saved_item_id");

            // On questions (formerly bank_questions):
            migrationBuilder.DropPrimaryKey(
                name: "pk_bank_questions",
                schema: "srs",
                table: "questions");

            migrationBuilder.AddPrimaryKey(
                name: "pk_questions",
                schema: "srs",
                table: "questions",
                column: "id");

            migrationBuilder.RenameIndex(
                name: "ix_bank_questions_knowledge_meaning_id",
                schema: "srs",
                table: "questions",
                newName: "ix_questions_knowledge_meaning_id");

            migrationBuilder.RenameIndex(
                name: "ix_bank_questions_question_type_id",
                schema: "srs",
                table: "questions",
                newName: "ix_questions_question_type_id");

            // 6. Re-add foreign keys with new table and constraint names
            // Foreign keys on srs.questions
            migrationBuilder.AddForeignKey(
                name: "fk_questions_knowledge_meanings_knowledge_meaning_id",
                schema: "srs",
                table: "questions",
                column: "knowledge_meaning_id",
                principalSchema: "knowledge",
                principalTable: "knowledge_meanings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_questions_question_types_question_type_id",
                schema: "srs",
                table: "questions",
                column: "question_type_id",
                principalSchema: "srs",
                principalTable: "question_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            // Foreign keys on srs.quiz_items
            migrationBuilder.AddForeignKey(
                name: "fk_quiz_items_questions_question_id",
                schema: "srs",
                table: "quiz_items",
                column: "question_id",
                principalSchema: "srs",
                principalTable: "questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_quiz_items_quizzes_quiz_id",
                schema: "srs",
                table: "quiz_items",
                column: "quiz_id",
                principalSchema: "srs",
                principalTable: "quizzes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_quiz_items_saved_items_saved_item_id",
                schema: "srs",
                table: "quiz_items",
                column: "saved_item_id",
                principalSchema: "srs",
                principalTable: "saved_items",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            // Foreign key from quiz_attempt_items to quiz_items
            migrationBuilder.AddForeignKey(
                name: "fk_quiz_attempt_items_quiz_items_question_id",
                schema: "srs",
                table: "quiz_attempt_items",
                column: "question_id",
                principalSchema: "srs",
                principalTable: "quiz_items",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback foreign keys
            migrationBuilder.DropForeignKey(
                name: "fk_quiz_attempt_items_quiz_items_question_id",
                schema: "srs",
                table: "quiz_attempt_items");

            migrationBuilder.DropForeignKey(
                name: "fk_quiz_items_questions_question_id",
                schema: "srs",
                table: "quiz_items");

            migrationBuilder.DropForeignKey(
                name: "fk_quiz_items_quizzes_quiz_id",
                schema: "srs",
                table: "quiz_items");

            migrationBuilder.DropForeignKey(
                name: "fk_quiz_items_saved_items_saved_item_id",
                schema: "srs",
                table: "quiz_items");

            migrationBuilder.DropForeignKey(
                name: "fk_questions_knowledge_meanings_knowledge_meaning_id",
                schema: "srs",
                table: "questions");

            migrationBuilder.DropForeignKey(
                name: "fk_questions_question_types_question_type_id",
                schema: "srs",
                table: "questions");

            // Re-add dropped columns to quiz_items before rename
            migrationBuilder.AddColumn<Guid>(
                name: "question_type_id",
                schema: "srs",
                table: "quiz_items",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateIndex(
                name: "ix_questions_question_type_id",
                schema: "srs",
                table: "quiz_items",
                column: "question_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_questions_question_types_question_type_id",
                schema: "srs",
                table: "quiz_items",
                column: "question_type_id",
                principalSchema: "srs",
                principalTable: "question_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddColumn<string>(
                name: "reading",
                schema: "srs",
                table: "quiz_items",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                schema: "srs",
                table: "quiz_items",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: DateTimeOffset.UtcNow);

            // Rename columns back on quiz_items
            migrationBuilder.RenameColumn(
                name: "question_id",
                schema: "srs",
                table: "quiz_items",
                newName: "bank_question_id");

            migrationBuilder.RenameColumn(
                name: "sequence",
                schema: "srs",
                table: "quiz_items",
                newName: "position");

            migrationBuilder.RenameColumn(
                name: "question",
                schema: "srs",
                table: "quiz_items",
                newName: "prompt");

            migrationBuilder.RenameColumn(
                name: "answers",
                schema: "srs",
                table: "quiz_items",
                newName: "choices_json");

            migrationBuilder.RenameColumn(
                name: "correct_answer",
                schema: "srs",
                table: "quiz_items",
                newName: "correct_option_id");

            // Rename columns back on questions
            migrationBuilder.RenameColumn(
                name: "question",
                schema: "srs",
                table: "questions",
                newName: "prompt");

            migrationBuilder.RenameColumn(
                name: "answers",
                schema: "srs",
                table: "questions",
                newName: "choices_json");

            migrationBuilder.RenameColumn(
                name: "correct_answer",
                schema: "srs",
                table: "questions",
                newName: "correct_choice_id");

            // Swap tables back (questions -> bank_questions, quiz_items -> questions)
            migrationBuilder.RenameTable(
                name: "questions",
                schema: "srs",
                newName: "bank_questions",
                newSchema: "srs");

            migrationBuilder.RenameTable(
                name: "quiz_items",
                schema: "srs",
                newName: "questions",
                newSchema: "srs");

            // Restore primary keys
            migrationBuilder.DropPrimaryKey(
                name: "pk_questions",
                schema: "srs",
                table: "bank_questions");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bank_questions",
                schema: "srs",
                table: "bank_questions",
                column: "id");

            migrationBuilder.DropPrimaryKey(
                name: "pk_quiz_items",
                schema: "srs",
                table: "questions");

            migrationBuilder.AddPrimaryKey(
                name: "pk_questions",
                schema: "srs",
                table: "questions",
                column: "id");

            // Restore index names
            migrationBuilder.RenameIndex(
                name: "ix_questions_knowledge_meaning_id",
                schema: "srs",
                table: "bank_questions",
                newName: "ix_bank_questions_knowledge_meaning_id");

            migrationBuilder.RenameIndex(
                name: "ix_questions_question_type_id",
                schema: "srs",
                table: "bank_questions",
                newName: "ix_bank_questions_question_type_id");

            migrationBuilder.RenameIndex(
                name: "ix_quiz_items_question_id",
                schema: "srs",
                table: "questions",
                newName: "ix_questions_bank_question_id");

            migrationBuilder.RenameIndex(
                name: "ix_quiz_items_quiz_id",
                schema: "srs",
                table: "questions",
                newName: "ix_questions_quiz_id");

            migrationBuilder.RenameIndex(
                name: "ix_quiz_items_saved_item_id",
                schema: "srs",
                table: "questions",
                newName: "ix_questions_saved_item_id");

            // Restore foreign keys
            migrationBuilder.AddForeignKey(
                name: "fk_bank_questions_knowledge_meanings_knowledge_meaning_id",
                schema: "srs",
                table: "bank_questions",
                column: "knowledge_meaning_id",
                principalSchema: "knowledge",
                principalTable: "knowledge_meanings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bank_questions_question_types_question_type_id",
                schema: "srs",
                table: "bank_questions",
                column: "question_type_id",
                principalSchema: "srs",
                principalTable: "question_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_questions_bank_questions_bank_question_id",
                schema: "srs",
                table: "questions",
                column: "bank_question_id",
                principalSchema: "srs",
                principalTable: "bank_questions",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_questions_quizzes_quiz_id",
                schema: "srs",
                table: "questions",
                column: "quiz_id",
                principalSchema: "srs",
                principalTable: "quizzes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_questions_saved_items_saved_item_id",
                schema: "srs",
                table: "questions",
                column: "saved_item_id",
                principalSchema: "srs",
                principalTable: "saved_items",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_quiz_attempt_items_questions_question_id",
                schema: "srs",
                table: "quiz_attempt_items",
                column: "question_id",
                principalSchema: "srs",
                principalTable: "questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
