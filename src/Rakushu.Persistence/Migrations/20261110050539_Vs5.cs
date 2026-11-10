using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Vs5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "start_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "end_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "created_by_id",
                schema: "shadowing",
                table: "shadowing_materials",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_materials_created_by_id",
                schema: "shadowing",
                table: "shadowing_materials",
                column: "created_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_shadowing_materials_users_created_by_id",
                schema: "shadowing",
                table: "shadowing_materials",
                column: "created_by_id",
                principalSchema: "identity",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shadowing_materials_users_created_by_id",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.DropIndex(
                name: "ix_shadowing_materials_created_by_id",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.DropColumn(
                name: "created_by_id",
                schema: "shadowing",
                table: "shadowing_materials");

            migrationBuilder.AlterColumn<Guid>(
                name: "start_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "end_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
