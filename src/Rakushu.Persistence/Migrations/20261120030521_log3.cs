using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class log3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "parent_job_log_id",
                schema: "processing",
                table: "processing_job_logs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_processing_job_logs_parent_job_log_id",
                schema: "processing",
                table: "processing_job_logs",
                column: "parent_job_log_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_processing_job_logs_parent_job_log_id",
                schema: "processing",
                table: "processing_job_logs");

            migrationBuilder.DropColumn(
                name: "parent_job_log_id",
                schema: "processing",
                table: "processing_job_logs");
        }
    }
}
