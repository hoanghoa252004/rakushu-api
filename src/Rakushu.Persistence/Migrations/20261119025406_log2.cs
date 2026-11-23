using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class log2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sequence",
                schema: "processing",
                table: "processing_job_logs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_processing_job_logs_target_type_target_id_sequence",
                schema: "processing",
                table: "processing_job_logs",
                columns: new[] { "target_type", "target_id", "sequence" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_processing_job_logs_target_type_target_id_sequence",
                schema: "processing",
                table: "processing_job_logs");

            migrationBuilder.DropColumn(
                name: "sequence",
                schema: "processing",
                table: "processing_job_logs");
        }
    }
}
