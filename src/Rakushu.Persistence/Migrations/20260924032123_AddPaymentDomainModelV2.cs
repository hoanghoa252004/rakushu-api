using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentDomainModelV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_payment_plans_plan_id",
                table: "payment");

            migrationBuilder.DropForeignKey(
                name: "fk_payment_users_user_id",
                table: "payment");

            migrationBuilder.DropForeignKey(
                name: "fk_transaction_payment_payment_id",
                table: "transaction");

            migrationBuilder.DropPrimaryKey(
                name: "pk_transaction",
                table: "transaction");

            migrationBuilder.DropPrimaryKey(
                name: "pk_payment",
                table: "payment");

            migrationBuilder.RenameTable(
                name: "transaction",
                newName: "transactions");

            migrationBuilder.RenameTable(
                name: "payment",
                newName: "payments");

            migrationBuilder.RenameIndex(
                name: "ix_transaction_url",
                table: "transactions",
                newName: "ix_transactions_url");

            migrationBuilder.RenameIndex(
                name: "ix_transaction_txn_ref",
                table: "transactions",
                newName: "ix_transactions_txn_ref");

            migrationBuilder.RenameIndex(
                name: "ix_transaction_payment_id",
                table: "transactions",
                newName: "ix_transactions_payment_id");

            migrationBuilder.RenameIndex(
                name: "ix_payment_user_id",
                table: "payments",
                newName: "ix_payments_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_payment_plan_id",
                table: "payments",
                newName: "ix_payments_plan_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_transactions",
                table: "transactions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_payments",
                table: "payments",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_payments_plans_plan_id",
                table: "payments",
                column: "plan_id",
                principalTable: "plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_payments_users_user_id",
                table: "payments",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_payments_payment_id",
                table: "transactions",
                column: "payment_id",
                principalTable: "payments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_payments_plans_plan_id",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "fk_payments_users_user_id",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_payments_payment_id",
                table: "transactions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_transactions",
                table: "transactions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_payments",
                table: "payments");

            migrationBuilder.RenameTable(
                name: "transactions",
                newName: "transaction");

            migrationBuilder.RenameTable(
                name: "payments",
                newName: "payment");

            migrationBuilder.RenameIndex(
                name: "ix_transactions_url",
                table: "transaction",
                newName: "ix_transaction_url");

            migrationBuilder.RenameIndex(
                name: "ix_transactions_txn_ref",
                table: "transaction",
                newName: "ix_transaction_txn_ref");

            migrationBuilder.RenameIndex(
                name: "ix_transactions_payment_id",
                table: "transaction",
                newName: "ix_transaction_payment_id");

            migrationBuilder.RenameIndex(
                name: "ix_payments_user_id",
                table: "payment",
                newName: "ix_payment_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_payments_plan_id",
                table: "payment",
                newName: "ix_payment_plan_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_transaction",
                table: "transaction",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_payment",
                table: "payment",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_payment_plans_plan_id",
                table: "payment",
                column: "plan_id",
                principalTable: "plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_payment_users_user_id",
                table: "payment",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transaction_payment_payment_id",
                table: "transaction",
                column: "payment_id",
                principalTable: "payment",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
