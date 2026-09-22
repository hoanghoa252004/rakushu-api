using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Payment_And_Update_Subscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "canceled_at",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "current_period_end",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "current_period_start",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "end_date",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: true),
                    order_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    qr_code_url = table.Column<string>(type: "text", nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payments", x => x.id);
                    table.ForeignKey(
                        name: "fk_payments_plans_plan_id",
                        column: x => x.plan_id,
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_payments_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_payments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payment_transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sepay_id = table.Column<long>(type: "bigint", nullable: false),
                    gateway = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    account_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transaction_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    transfer_type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    transfer_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    reference_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    raw_webhook_data = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_payment_transactions_payments_payment_id",
                        column: x => x.payment_id,
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_payment_transactions_payment_id",
                table: "payment_transactions",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_transactions_sepay_id",
                table: "payment_transactions",
                column: "sepay_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_payments_order_code",
                table: "payments",
                column: "order_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_payments_plan_id",
                table: "payments",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_subscription_id",
                table: "payments",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_user_id",
                table: "payments",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment_transactions");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropColumn(
                name: "canceled_at",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "current_period_end",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "current_period_start",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "end_date",
                table: "subscriptions");
        }
    }
}
