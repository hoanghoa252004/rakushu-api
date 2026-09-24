using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_PaymentTransactions_SepayId_And_ExpiresAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_payment_transactions_sepay_id",
                table: "payment_transactions");

            migrationBuilder.AlterColumn<long>(
                name: "sepay_id",
                table: "payment_transactions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "expires_at",
                table: "payment_transactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "ix_payment_transactions_sepay_id",
                table: "payment_transactions",
                column: "sepay_id",
                unique: true,
                filter: "sepay_id IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_payment_transactions_sepay_id",
                table: "payment_transactions");

            migrationBuilder.DropColumn(
                name: "expires_at",
                table: "payment_transactions");

            migrationBuilder.AlterColumn<long>(
                name: "sepay_id",
                table: "payment_transactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_payment_transactions_sepay_id",
                table: "payment_transactions",
                column: "sepay_id",
                unique: true);
        }
    }
}
