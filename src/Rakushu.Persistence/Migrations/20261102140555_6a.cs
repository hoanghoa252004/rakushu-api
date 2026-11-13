using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _6a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "shadowing_materials",
                schema: "shadowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_materials", x => x.id);
                    table.ForeignKey(
                        name: "fk_shadowing_materials_videos_video_id",
                        column: x => x.video_id,
                        principalSchema: "material",
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shadowing_roles",
                schema: "shadowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_material_id = table.Column<Guid>(type: "uuid", nullable: false),
                    speaker_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_roles", x => x.id);
                    table.ForeignKey(
                        name: "fk_shadowing_roles_shadowing_materials_shadowing_material_id",
                        column: x => x.shadowing_material_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_materials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shadowing_roles_speakers_speaker_id",
                        column: x => x.speaker_id,
                        principalSchema: "shadowing",
                        principalTable: "speakers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shadowing_sessions",
                schema: "shadowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_material_id = table.Column<Guid>(type: "uuid", nullable: false),
                    host_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    visibility = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    scheduled_start_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_shadowing_sessions_shadowing_materials_shadowing_material_id",
                        column: x => x.shadowing_material_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_materials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shadowing_sessions_users_host_id",
                        column: x => x.host_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shadowing_role_lines",
                schema: "shadowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_material_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    audio_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_role_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_shadowing_role_lines_shadowing_materials_shadowing_material",
                        column: x => x.shadowing_material_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_materials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shadowing_role_lines_shadowing_roles_shadowing_role_id",
                        column: x => x.shadowing_role_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shadowing_session_invitations",
                schema: "shadowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    host_id = table.Column<Guid>(type: "uuid", nullable: false),
                    learner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    sent_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    responsed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_session_invitations", x => x.id);
                    table.ForeignKey(
                        name: "fk_shadowing_session_invitations_shadowing_sessions_shadowing_",
                        column: x => x.shadowing_session_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shadowing_session_invitations_users_host_id",
                        column: x => x.host_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shadowing_session_invitations_users_learner_id",
                        column: x => x.learner_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shadowing_session_members",
                schema: "shadowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    member_id = table.Column<Guid>(type: "uuid", nullable: false),
                    join_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    shadowing_session_invitation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    joined_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_session_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_shadowing_session_members_shadowing_session_invitations_sha",
                        column: x => x.shadowing_session_invitation_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_session_invitations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shadowing_session_members_shadowing_sessions_shadowing_sess",
                        column: x => x.shadowing_session_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shadowing_session_members_users_member_id",
                        column: x => x.member_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shadowing_attempts",
                schema: "shadowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_session_member_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_attempts", x => x.id);
                    table.ForeignKey(
                        name: "fk_shadowing_attempts_shadowing_session_members_shadowing_sess",
                        column: x => x.shadowing_session_member_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_session_members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shadowing_attempts_shadowing_sessions_shadowing_session_id",
                        column: x => x.shadowing_session_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shadowing_assessments",
                schema: "shadowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_attempt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    evaluator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    overall_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    overal_feedback = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_assessments", x => x.id);
                    table.ForeignKey(
                        name: "fk_shadowing_assessments_shadowing_attempts_shadowing_attempt_",
                        column: x => x.shadowing_attempt_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shadowing_assessments_users_evaluator_id",
                        column: x => x.evaluator_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shadowing_attempt_details",
                schema: "shadowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_attempt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_role_line_id = table.Column<Guid>(type: "uuid", nullable: false),
                    audio_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    duration_ms = table.Column<long>(type: "bigint", nullable: true),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_attempt_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_shadowing_attempt_details_shadowing_attempts_shadowing_atte",
                        column: x => x.shadowing_attempt_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shadowing_attempt_details_shadowing_role_lines_shadowing_ro",
                        column: x => x.shadowing_role_line_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_role_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shadowing_assessment_details",
                schema: "shadowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_assessment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadowing_attempt_detail_id = table.Column<Guid>(type: "uuid", nullable: false),
                    accuracy_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    timing_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    pitch_similarity_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    intensity_similarity_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    feedback = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_assessment_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_shadowing_assessment_details_shadowing_assessments_shadowin",
                        column: x => x.shadowing_assessment_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_assessments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shadowing_assessment_details_shadowing_attempt_details_shad",
                        column: x => x.shadowing_attempt_detail_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_attempt_details",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_assessment_details_shadowing_assessment_id_shadow",
                schema: "shadowing",
                table: "shadowing_assessment_details",
                columns: new[] { "shadowing_assessment_id", "shadowing_attempt_detail_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_assessment_details_shadowing_attempt_detail_id",
                schema: "shadowing",
                table: "shadowing_assessment_details",
                column: "shadowing_attempt_detail_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_assessments_evaluator_id",
                schema: "shadowing",
                table: "shadowing_assessments",
                column: "evaluator_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_assessments_shadowing_attempt_id",
                schema: "shadowing",
                table: "shadowing_assessments",
                column: "shadowing_attempt_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_attempt_details_shadowing_attempt_id_shadowing_ro",
                schema: "shadowing",
                table: "shadowing_attempt_details",
                columns: new[] { "shadowing_attempt_id", "shadowing_role_line_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_attempt_details_shadowing_role_line_id",
                schema: "shadowing",
                table: "shadowing_attempt_details",
                column: "shadowing_role_line_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_attempts_shadowing_session_id",
                schema: "shadowing",
                table: "shadowing_attempts",
                column: "shadowing_session_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_attempts_shadowing_session_member_id",
                schema: "shadowing",
                table: "shadowing_attempts",
                column: "shadowing_session_member_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_materials_code",
                schema: "shadowing",
                table: "shadowing_materials",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_materials_video_id_version",
                schema: "shadowing",
                table: "shadowing_materials",
                columns: new[] { "video_id", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_role_lines_shadowing_material_id_sort_order",
                schema: "shadowing",
                table: "shadowing_role_lines",
                columns: new[] { "shadowing_material_id", "sort_order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_role_lines_shadowing_role_id",
                schema: "shadowing",
                table: "shadowing_role_lines",
                column: "shadowing_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_roles_shadowing_material_id_sort_order",
                schema: "shadowing",
                table: "shadowing_roles",
                columns: new[] { "shadowing_material_id", "sort_order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_roles_shadowing_material_id_speaker_id",
                schema: "shadowing",
                table: "shadowing_roles",
                columns: new[] { "shadowing_material_id", "speaker_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_roles_speaker_id",
                schema: "shadowing",
                table: "shadowing_roles",
                column: "speaker_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_session_invitations_host_id",
                schema: "shadowing",
                table: "shadowing_session_invitations",
                column: "host_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_session_invitations_learner_id",
                schema: "shadowing",
                table: "shadowing_session_invitations",
                column: "learner_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_session_invitations_shadowing_session_id_learner_",
                schema: "shadowing",
                table: "shadowing_session_invitations",
                columns: new[] { "shadowing_session_id", "learner_id" });

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_session_members_member_id",
                schema: "shadowing",
                table: "shadowing_session_members",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_session_members_shadowing_session_id_member_id",
                schema: "shadowing",
                table: "shadowing_session_members",
                columns: new[] { "shadowing_session_id", "member_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_session_members_shadowing_session_invitation_id",
                schema: "shadowing",
                table: "shadowing_session_members",
                column: "shadowing_session_invitation_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_sessions_host_id",
                schema: "shadowing",
                table: "shadowing_sessions",
                column: "host_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_sessions_shadowing_material_id",
                schema: "shadowing",
                table: "shadowing_sessions",
                column: "shadowing_material_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shadowing_assessment_details",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "shadowing_assessments",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "shadowing_attempt_details",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "shadowing_attempts",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "shadowing_role_lines",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "shadowing_session_members",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "shadowing_roles",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "shadowing_session_invitations",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "shadowing_sessions",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "shadowing_materials",
                schema: "shadowing");
        }
    }
}
