using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceTypeAndStringStatusToBankQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE srs.questions ALTER COLUMN status DROP DEFAULT;
                ALTER TABLE srs.questions
                    ALTER COLUMN status TYPE character varying(20)
                    USING CASE status
                        WHEN 1 THEN 'Draft'
                        WHEN 2 THEN 'Published'
                        ELSE 'Published'
                    END;
                ALTER TABLE srs.questions ALTER COLUMN status SET NOT NULL;
                ALTER TABLE srs.questions ALTER COLUMN status SET DEFAULT 'Published';
                """);

            migrationBuilder.AddColumn<string>(
                name: "source_type",
                schema: "srs",
                table: "questions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Curator");

            migrationBuilder.Sql("""
                UPDATE srs.questions
                SET source_type = 'Ai'
                WHERE status = 'Draft'
                   OR (reviewed_at IS NOT NULL AND reviewed_at > created_at);
                """);

            migrationBuilder.CreateIndex(
                name: "ix_questions_source_type",
                schema: "srs",
                table: "questions",
                column: "source_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_questions_source_type",
                schema: "srs",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "source_type",
                schema: "srs",
                table: "questions");

            migrationBuilder.Sql("""
                ALTER TABLE srs.questions ALTER COLUMN status DROP DEFAULT;
                ALTER TABLE srs.questions
                    ALTER COLUMN status TYPE integer
                    USING CASE status
                        WHEN 'Draft' THEN 1
                        WHEN 'Published' THEN 2
                        ELSE 2
                    END;
                ALTER TABLE srs.questions ALTER COLUMN status SET NOT NULL;
                ALTER TABLE srs.questions ALTER COLUMN status SET DEFAULT 2;
                """);
        }
    }
}
