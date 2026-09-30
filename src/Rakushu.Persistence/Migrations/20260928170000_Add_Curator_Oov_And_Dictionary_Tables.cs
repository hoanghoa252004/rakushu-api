using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Curator_Oov_And_Dictionary_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dictionary_entries",
                columns: table => new
                {
                    dictionary_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    term = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    reading = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    part_of_speech = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    meaning = table.Column<string>(type: "text", nullable: false),
                    meaning_vietnamese = table.Column<string>(type: "text", nullable: true),
                    jlpt_level = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    word_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    audio_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    definition_tags = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dictionary_entries", x => x.dictionary_entry_id);
                });

            migrationBuilder.CreateTable(
                name: "oov_candidates",
                columns: table => new
                {
                    oov_candidate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_id = table.Column<Guid>(type: "uuid", nullable: true),
                    term = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    tentative_reading = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    tentative_pos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    suggested_meaning = table.Column<string>(type: "text", nullable: true),
                    context_snippet = table.Column<string>(type: "text", nullable: true),
                    confidence_score = table.Column<double>(type: "double precision", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    detected_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_oov_candidates", x => x.oov_candidate_id);
                });

            migrationBuilder.CreateTable(
                name: "curator_reviews",
                columns: table => new
                {
                    review_id = table.Column<Guid>(type: "uuid", nullable: false),
                    oov_candidate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    curator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    decision = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    edited_term = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    edited_reading = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    edited_pos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    edited_meaning = table.Column<string>(type: "text", nullable: true),
                    comment = table.Column<string>(type: "text", nullable: true),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_curator_reviews", x => x.review_id);
                    table.ForeignKey(
                        name: "fk_curator_reviews_oov_candidates_oov_candidate_id",
                        column: x => x.oov_candidate_id,
                        principalTable: "oov_candidates",
                        principalColumn: "oov_candidate_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_curator_reviews_users_curator_id",
                        column: x => x.curator_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_curator_reviews_curator_id",
                table: "curator_reviews",
                column: "curator_id");

            migrationBuilder.CreateIndex(
                name: "ix_curator_reviews_oov_candidate_id",
                table: "curator_reviews",
                column: "oov_candidate_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_dictionary_entries_reading",
                table: "dictionary_entries",
                column: "reading");

            migrationBuilder.CreateIndex(
                name: "ix_dictionary_entries_term",
                table: "dictionary_entries",
                column: "term");

            migrationBuilder.CreateIndex(
                name: "ix_oov_candidates_status",
                table: "oov_candidates",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_oov_candidates_term",
                table: "oov_candidates",
                column: "term");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "curator_reviews");

            migrationBuilder.DropTable(
                name: "dictionary_entries");

            migrationBuilder.DropTable(
                name: "oov_candidates");
        }
    }
}
