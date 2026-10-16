using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Series1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "learner_id",
                schema: "material",
                table: "videos",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                schema: "material",
                table: "series",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_series_user_id",
                schema: "material",
                table: "series",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_series_users_user_id",
                schema: "material",
                table: "series",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_series_users_user_id",
                schema: "material",
                table: "series");

            migrationBuilder.DropIndex(
                name: "ix_series_user_id",
                schema: "material",
                table: "series");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "material",
                table: "series");

            migrationBuilder.AlterColumn<Guid>(
                name: "learner_id",
                schema: "material",
                table: "videos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
