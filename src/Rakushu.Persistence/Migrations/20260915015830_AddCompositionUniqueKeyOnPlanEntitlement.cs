using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositionUniqueKeyOnPlanEntitlement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_plan_entitlement_plan_id",
                table: "plan_entitlement");

            migrationBuilder.CreateIndex(
                name: "ix_plan_entitlement_plan_id_feature_id",
                table: "plan_entitlement",
                columns: new[] { "plan_id", "feature_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_plan_entitlement_plan_id_feature_id",
                table: "plan_entitlement");

            migrationBuilder.CreateIndex(
                name: "ix_plan_entitlement_plan_id",
                table: "plan_entitlement",
                column: "plan_id");
        }
    }
}
