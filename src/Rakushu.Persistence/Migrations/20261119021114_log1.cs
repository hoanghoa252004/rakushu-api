using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class log1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "processing");

            migrationBuilder.CreateTable(
                name: "processing_job_logs",
                schema: "processing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    target_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    job_label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    input = table.Column<string>(type: "text", nullable: true),
                    output = table.Column<string>(type: "text", nullable: true),
                    is_success = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_failed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    start_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_processing_job_logs", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_processing_job_logs_job_label",
                schema: "processing",
                table: "processing_job_logs",
                column: "job_label");

            migrationBuilder.CreateIndex(
                name: "ix_processing_job_logs_target_id_job_type_start_at",
                schema: "processing",
                table: "processing_job_logs",
                columns: new[] { "target_id", "job_type", "start_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "processing_job_logs",
                schema: "processing");
        }
    }
}
