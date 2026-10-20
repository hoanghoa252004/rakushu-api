using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Embe2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_knowledge_embedding",
                schema: "knowledge",
                table: "knowledge_embedding");

            migrationBuilder.DropIndex(
                name: "uq_knowledge_embeddings_meaning_model",
                schema: "knowledge",
                table: "knowledge_embedding");

            migrationBuilder.DropColumn(
                name: "model_id",
                schema: "knowledge",
                table: "knowledge_embedding");

            migrationBuilder.RenameTable(
                name: "knowledge_embedding",
                schema: "knowledge",
                newName: "knowledge_embeddings",
                newSchema: "knowledge");

            migrationBuilder.AddPrimaryKey(
                name: "pk_knowledge_embeddings",
                schema: "knowledge",
                table: "knowledge_embeddings",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_embeddings_knowledge_meaning_id",
                schema: "knowledge",
                table: "knowledge_embeddings",
                column: "knowledge_meaning_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_knowledge_embeddings",
                schema: "knowledge",
                table: "knowledge_embeddings");

            migrationBuilder.DropIndex(
                name: "ix_knowledge_embeddings_knowledge_meaning_id",
                schema: "knowledge",
                table: "knowledge_embeddings");

            migrationBuilder.RenameTable(
                name: "knowledge_embeddings",
                schema: "knowledge",
                newName: "knowledge_embedding",
                newSchema: "knowledge");

            migrationBuilder.AddColumn<string>(
                name: "model_id",
                schema: "knowledge",
                table: "knowledge_embedding",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_knowledge_embedding",
                schema: "knowledge",
                table: "knowledge_embedding",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "uq_knowledge_embeddings_meaning_model",
                schema: "knowledge",
                table: "knowledge_embedding",
                columns: new[] { "knowledge_meaning_id", "model_id" },
                unique: true);
        }
    }
}
