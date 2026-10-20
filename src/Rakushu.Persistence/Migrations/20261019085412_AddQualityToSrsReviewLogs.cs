using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQualityToSrsReviewLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "quality",
                schema: "srs",
                table: "srs_review_logs",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE srs.srs_review_logs
                SET quality = CASE LOWER(rating)
                    WHEN 'hard' THEN 3
                    WHEN 'good' THEN 4
                    WHEN 'easy' THEN 5
                    ELSE 0
                END;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "quality",
                schema: "srs",
                table: "srs_review_logs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_srs_review_logs_quality",
                schema: "srs",
                table: "srs_review_logs",
                sql: "quality BETWEEN 0 AND 5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_srs_review_logs_quality",
                schema: "srs",
                table: "srs_review_logs");

            migrationBuilder.DropColumn(
                name: "quality",
                schema: "srs",
                table: "srs_review_logs");
        }
    }
}
