using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class KnowledgeElement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "knowledge_unit_elements",
                schema: "knowledge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    knowledge_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    surface = table.Column<string>(type: "text", nullable: false),
                    lemma = table.Column<string>(type: "text", nullable: false),
                    reading = table.Column<string>(type: "text", nullable: false),
                    head_sequence = table.Column<int>(type: "integer", nullable: false),
                    start_index = table.Column<int>(type: "integer", nullable: false),
                    end_index = table.Column<int>(type: "integer", nullable: false),
                    knowledge_hash = table.Column<string>(type: "text", nullable: false),
                    japanese_part_of_speech_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dependency_relationship_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_knowledge_unit_elements", x => x.id);
                    table.ForeignKey(
                        name: "fk_knowledge_unit_elements_dependency_relationships_dependency",
                        column: x => x.dependency_relationship_id,
                        principalSchema: "knowledge",
                        principalTable: "dependency_relationships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_knowledge_unit_elements_japanese_part_of_speeches_japanese_",
                        column: x => x.japanese_part_of_speech_id,
                        principalSchema: "knowledge",
                        principalTable: "japanese_part_of_speeches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_knowledge_unit_elements_knowledge_units_knowledge_unit_id",
                        column: x => x.knowledge_unit_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_unit_elements_dependency_relationship_id",
                schema: "knowledge",
                table: "knowledge_unit_elements",
                column: "dependency_relationship_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_unit_elements_japanese_part_of_speech_id",
                schema: "knowledge",
                table: "knowledge_unit_elements",
                column: "japanese_part_of_speech_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_unit_elements_knowledge_unit_id",
                schema: "knowledge",
                table: "knowledge_unit_elements",
                column: "knowledge_unit_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "knowledge_unit_elements",
                schema: "knowledge");
        }
    }
}
