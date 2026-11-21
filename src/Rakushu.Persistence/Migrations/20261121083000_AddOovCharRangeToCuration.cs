using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOovCharRangeToCuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_issue_categories_target_type_target_id_category",
                schema: "curation",
                table: "issue_categories");

            migrationBuilder.AddColumn<int>(
                name: "end_char_index",
                schema: "curation",
                table: "reports",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "start_char_index",
                schema: "curation",
                table: "reports",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "end_char_index",
                schema: "curation",
                table: "issue_details",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "start_char_index",
                schema: "curation",
                table: "issue_details",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "end_char_index",
                schema: "curation",
                table: "issue_categories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "start_char_index",
                schema: "curation",
                table: "issue_categories",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_reports_target_category_span",
                schema: "curation",
                table: "reports",
                columns: new[] { "target_type", "target_id", "category", "start_char_index", "end_char_index" });

            migrationBuilder.CreateIndex(
                name: "ix_issue_details_oov_span",
                schema: "curation",
                table: "issue_details",
                columns: new[] { "target_id", "start_char_index", "end_char_index" },
                filter: "category = 'Oov' AND start_char_index IS NOT NULL AND end_char_index IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_issue_categories_non_oov_unique",
                schema: "curation",
                table: "issue_categories",
                columns: new[] { "target_type", "target_id", "category" },
                unique: true,
                filter: "start_char_index IS NULL AND end_char_index IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_issue_categories_oov_span_unique",
                schema: "curation",
                table: "issue_categories",
                columns: new[] { "target_type", "target_id", "category", "start_char_index", "end_char_index" },
                unique: true,
                filter: "start_char_index IS NOT NULL AND end_char_index IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_reports_target_category_span",
                schema: "curation",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "ix_issue_details_oov_span",
                schema: "curation",
                table: "issue_details");

            migrationBuilder.DropIndex(
                name: "ix_issue_categories_non_oov_unique",
                schema: "curation",
                table: "issue_categories");

            migrationBuilder.DropIndex(
                name: "ix_issue_categories_oov_span_unique",
                schema: "curation",
                table: "issue_categories");

            migrationBuilder.DropColumn(
                name: "end_char_index",
                schema: "curation",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "start_char_index",
                schema: "curation",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "end_char_index",
                schema: "curation",
                table: "issue_details");

            migrationBuilder.DropColumn(
                name: "start_char_index",
                schema: "curation",
                table: "issue_details");

            migrationBuilder.DropColumn(
                name: "end_char_index",
                schema: "curation",
                table: "issue_categories");

            migrationBuilder.DropColumn(
                name: "start_char_index",
                schema: "curation",
                table: "issue_categories");

            migrationBuilder.CreateIndex(
                name: "ix_issue_categories_target_type_target_id_category",
                schema: "curation",
                table: "issue_categories",
                columns: new[] { "target_type", "target_id", "category" },
                unique: true);
        }
    }
}
