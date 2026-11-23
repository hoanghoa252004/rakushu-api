using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCuratorReviewToBankQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "reviewed_at",
                schema: "srs",
                table: "questions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                schema: "srs",
                table: "questions",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateIndex(
                name: "ix_questions_knowledge_meaning_id_status_is_active",
                schema: "srs",
                table: "questions",
                columns: new[] { "knowledge_meaning_id", "status", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_questions_status",
                schema: "srs",
                table: "questions",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_questions_knowledge_meaning_id_status_is_active",
                schema: "srs",
                table: "questions");

            migrationBuilder.DropIndex(
                name: "ix_questions_status",
                schema: "srs",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "reviewed_at",
                schema: "srs",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "srs",
                table: "questions");
        }
    }
}
