using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSavedItemAndRenameScheduleDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_quiz_items_questions_question_id",
                schema: "srs",
                table: "quiz_items");

            migrationBuilder.DropForeignKey(
                name: "fk_saved_items_knowledge_meanings_knowledge_meaning_id",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.DropIndex(
                name: "ix_srs_schedules_status_due_at",
                schema: "srs",
                table: "srs_schedules");

            migrationBuilder.DropColumn(
                name: "last_reviewed_at",
                schema: "srs",
                table: "srs_schedules");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "srs",
                table: "srs_schedules");

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.RenameIndex(
                name: "ix_quiz_items_question_id",
                schema: "srs",
                table: "quiz_items",
                newName: "ix_quiz_items_bank_question_id");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "srs",
                table: "saved_items",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.RenameTable(
                name: "srs_item_details",
                schema: "srs",
                newName: "srs_schedule_details",
                newSchema: "srs");

            migrationBuilder.CreateIndex(
                name: "ix_srs_schedules_due_at",
                schema: "srs",
                table: "srs_schedules",
                column: "due_at");

            migrationBuilder.AddForeignKey(
                name: "fk_quiz_items_questions_bank_question_id",
                schema: "srs",
                table: "quiz_items",
                column: "question_id",
                principalSchema: "srs",
                principalTable: "questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_saved_items_knowledge_meanings_knowledge_meaning_id",
                schema: "srs",
                table: "saved_items",
                column: "knowledge_meaning_id",
                principalSchema: "knowledge",
                principalTable: "knowledge_meanings",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_quiz_items_questions_bank_question_id",
                schema: "srs",
                table: "quiz_items");

            migrationBuilder.DropForeignKey(
                name: "fk_saved_items_knowledge_meanings_knowledge_meaning_id",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.RenameTable(
                name: "srs_schedule_details",
                schema: "srs",
                newName: "srs_item_details",
                newSchema: "srs");

            migrationBuilder.DropIndex(
                name: "ix_srs_schedules_due_at",
                schema: "srs",
                table: "srs_schedules");

            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "srs",
                table: "saved_items");

            migrationBuilder.RenameIndex(
                name: "ix_quiz_items_bank_question_id",
                schema: "srs",
                table: "quiz_items",
                newName: "ix_quiz_items_question_id");

            migrationBuilder.AddColumn<string>(
                name: "status",
                schema: "srs",
                table: "srs_schedules",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_reviewed_at",
                schema: "srs",
                table: "srs_schedules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                schema: "srs",
                table: "saved_items",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));


            migrationBuilder.CreateIndex(
                name: "ix_srs_schedules_status_due_at",
                schema: "srs",
                table: "srs_schedules",
                columns: new[] { "status", "due_at" });

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
                name: "fk_saved_items_knowledge_meanings_knowledge_meaning_id",
                schema: "srs",
                table: "saved_items",
                column: "knowledge_meaning_id",
                principalSchema: "knowledge",
                principalTable: "knowledge_meanings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
