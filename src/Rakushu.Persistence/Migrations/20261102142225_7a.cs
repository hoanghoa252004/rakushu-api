using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _7a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "shadowing_role_id",
                schema: "shadowing",
                table: "shadowing_session_members",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_session_members_shadowing_role_id",
                schema: "shadowing",
                table: "shadowing_session_members",
                column: "shadowing_role_id");

            migrationBuilder.AddForeignKey(
                name: "fk_shadowing_session_members_shadowing_roles_shadowing_role_id",
                schema: "shadowing",
                table: "shadowing_session_members",
                column: "shadowing_role_id",
                principalSchema: "shadowing",
                principalTable: "shadowing_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shadowing_session_members_shadowing_roles_shadowing_role_id",
                schema: "shadowing",
                table: "shadowing_session_members");

            migrationBuilder.DropIndex(
                name: "ix_shadowing_session_members_shadowing_role_id",
                schema: "shadowing",
                table: "shadowing_session_members");

            migrationBuilder.DropColumn(
                name: "shadowing_role_id",
                schema: "shadowing",
                table: "shadowing_session_members");
        }
    }
}
