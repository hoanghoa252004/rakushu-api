using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "material");

            migrationBuilder.EnsureSchema(
                name: "knowledge");

            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.EnsureSchema(
                name: "subscription");

            migrationBuilder.EnsureSchema(
                name: "srs");

            migrationBuilder.EnsureSchema(
                name: "curation");

            migrationBuilder.EnsureSchema(
                name: "shadowing");

            migrationBuilder.CreateTable(
                name: "content_categories",
                schema: "knowledge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    japanese_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    theme_color = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dependency_relationships",
                schema: "knowledge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    japanese_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dependency_relationships", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "features",
                schema: "subscription",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_features", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "issue_categories",
                schema: "curation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    current_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    threshold = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    last_reported_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "japanese_part_of_speeches",
                schema: "knowledge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    japanese_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_japanese_part_of_speeches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "knowledge_collections",
                schema: "knowledge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    collection_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    scope = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_knowledge_collections", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plans",
                schema: "subscription",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    japanese_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    billing_cycle = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "proficiency_levels",
                schema: "knowledge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    japanese_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_proficiency_levels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "question_types",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reports",
                schema: "curation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    curator_notes = table.Column<string>(type: "text", nullable: true),
                    resolved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reports", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "speakers",
                schema: "shadowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    japanese_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    avatar_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    theme_color = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_speakers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "content_knowledge_collections",
                schema: "knowledge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_collection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_knowledge_collections", x => x.id);
                    table.ForeignKey(
                        name: "fk_content_knowledge_collections_content_categories_content_ca",
                        column: x => x.content_category_id,
                        principalSchema: "knowledge",
                        principalTable: "content_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_content_knowledge_collections_knowledge_collections_knowled",
                        column: x => x.knowledge_collection_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_collections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entitlements",
                schema: "subscription",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    limit_value = table.Column<int>(type: "integer", nullable: false),
                    limit_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    limit_period = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entitlements", x => x.id);
                    table.ForeignKey(
                        name: "fk_entitlements_features_feature_id",
                        column: x => x.feature_id,
                        principalSchema: "subscription",
                        principalTable: "features",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_entitlements_plans_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "subscription",
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "knowledge_units",
                schema: "knowledge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    knowledge_collection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    proficiency_level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expression = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    reading = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    audio_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    source_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    knowledge_hash = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_knowledge_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_knowledge_units_knowledge_collections_knowledge_collection_",
                        column: x => x.knowledge_collection_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_collections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_knowledge_units_proficiency_levels_proficiency_level_id",
                        column: x => x.proficiency_level_id,
                        principalSchema: "knowledge",
                        principalTable: "proficiency_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "identity",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "knowledge_meanings",
                schema: "knowledge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    meaning = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    japanese_meaning = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    knowledge_unit_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_knowledge_meanings", x => x.id);
                    table.ForeignKey(
                        name: "fk_knowledge_meanings_knowledge_units_knowledge_unit_id",
                        column: x => x.knowledge_unit_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "email_verification_tokens",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code_hash = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    used_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_email_verification_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_email_verification_tokens_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "flashcard_sessions",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flashcard_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_flashcard_sessions_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "issue_details",
                schema: "curation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    learner_feedback = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    report_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_issue_details_reports_report_id",
                        column: x => x.report_id,
                        principalSchema: "curation",
                        principalTable: "reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_issue_details_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                schema: "subscription",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    expired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payments", x => x.id);
                    table.ForeignKey(
                        name: "fk_payments_plans_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "subscription",
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_payments_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "profiles",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    avatar_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    daily_learning_minutes = table.Column<int>(type: "integer", nullable: false),
                    session_duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_profiles", x => x.id);
                    table.ForeignKey(
                        name: "fk_profiles_proficiency_levels_level_id",
                        column: x => x.level_id,
                        principalSchema: "knowledge",
                        principalTable: "proficiency_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_profiles_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quizzes",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_questions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quizzes", x => x.id);
                    table.ForeignKey(
                        name: "fk_quizzes_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "text", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    used_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "series",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    japanese_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    thumbnail_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    source_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_series", x => x.id);
                    table.ForeignKey(
                        name: "fk_series_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_meaning_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question = table.Column<string>(type: "text", nullable: false),
                    answers = table.Column<string>(type: "jsonb", nullable: false),
                    correct_answer = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    explanation = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    media_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "NONE"),
                    media_url = table.Column<string>(type: "text", nullable: true),
                    media_metadata = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_questions_knowledge_meanings_knowledge_meaning_id",
                        column: x => x.knowledge_meaning_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_meanings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_questions_question_types_question_type_id",
                        column: x => x.question_type_id,
                        principalSchema: "srs",
                        principalTable: "question_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "saved_items",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_meaning_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    saved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_saved_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_saved_items_knowledge_meanings_knowledge_meaning_id",
                        column: x => x.knowledge_meaning_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_meanings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_saved_items_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                schema: "subscription",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    start_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    end_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "fk_subscriptions_payments_payment_id",
                        column: x => x.payment_id,
                        principalSchema: "subscription",
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_subscriptions_plans_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "subscription",
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_subscriptions_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                schema: "subscription",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    txn_ref = table.Column<string>(type: "text", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false),
                    transaction_no = table.Column<string>(type: "text", nullable: true),
                    raw_response_payload = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    expired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_transactions_payments_payment_id",
                        column: x => x.payment_id,
                        principalSchema: "subscription",
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "interests",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_interests", x => x.id);
                    table.ForeignKey(
                        name: "fk_interests_content_categories_content_category_id",
                        column: x => x.content_category_id,
                        principalSchema: "knowledge",
                        principalTable: "content_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_interests_profiles_profile_id",
                        column: x => x.profile_id,
                        principalSchema: "identity",
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_attempts",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quiz_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    mode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    total_items = table.Column<int>(type: "integer", nullable: false),
                    answered_items = table.Column<int>(type: "integer", nullable: false),
                    correct_items = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_evaluated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quiz_attempts", x => x.id);
                    table.ForeignKey(
                        name: "fk_quiz_attempts_quizzes_quiz_id",
                        column: x => x.quiz_id,
                        principalSchema: "srs",
                        principalTable: "quizzes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quiz_attempts_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "videos",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    content_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    series_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    source_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    source_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    learner_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_videos", x => x.id);
                    table.ForeignKey(
                        name: "fk_videos_content_categories_content_category_id",
                        column: x => x.content_category_id,
                        principalSchema: "knowledge",
                        principalTable: "content_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_videos_series_series_id",
                        column: x => x.series_id,
                        principalSchema: "material",
                        principalTable: "series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_videos_users_learner_id",
                        column: x => x.learner_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "flashcard_session_cards",
                schema: "srs",
                columns: table => new
                {
                    flashcard_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    saved_item_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flashcard_session_cards", x => new { x.flashcard_session_id, x.saved_item_id });
                    table.ForeignKey(
                        name: "fk_flashcard_session_cards_flashcard_sessions_flashcard_sessio",
                        column: x => x.flashcard_session_id,
                        principalSchema: "srs",
                        principalTable: "flashcard_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_flashcard_session_cards_saved_items_saved_item_id",
                        column: x => x.saved_item_id,
                        principalSchema: "srs",
                        principalTable: "saved_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "flashcard_session_items",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    flashcard_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    saved_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flashcard_session_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_flashcard_session_items_flashcard_sessions_flashcard_sessio",
                        column: x => x.flashcard_session_id,
                        principalSchema: "srs",
                        principalTable: "flashcard_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_flashcard_session_items_saved_items_saved_item_id",
                        column: x => x.saved_item_id,
                        principalSchema: "srs",
                        principalTable: "saved_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_items",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quiz_id = table.Column<Guid>(type: "uuid", nullable: false),
                    saved_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    question = table.Column<string>(type: "text", nullable: false),
                    answers = table.Column<string>(type: "jsonb", nullable: false),
                    correct_answer = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    media_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "NONE"),
                    media_url = table.Column<string>(type: "text", nullable: true),
                    media_metadata = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quiz_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_quiz_items_questions_bank_question_id",
                        column: x => x.question_id,
                        principalSchema: "srs",
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quiz_items_quizzes_quiz_id",
                        column: x => x.quiz_id,
                        principalSchema: "srs",
                        principalTable: "quizzes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quiz_items_saved_items_saved_item_id",
                        column: x => x.saved_item_id,
                        principalSchema: "srs",
                        principalTable: "saved_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "srs_schedules",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    saved_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_schedules", x => x.id);
                    table.ForeignKey(
                        name: "fk_srs_schedules_saved_items_saved_item_id",
                        column: x => x.saved_item_id,
                        principalSchema: "srs",
                        principalTable: "saved_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription_usages",
                schema: "subscription",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    period_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    max_value = table.Column<int>(type: "integer", nullable: false),
                    used_value = table.Column<int>(type: "integer", nullable: false),
                    is_over_limit = table.Column<bool>(type: "boolean", nullable: false),
                    is_expired = table.Column<bool>(type: "boolean", nullable: false),
                    is_canceled = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_usages", x => x.id);
                    table.ForeignKey(
                        name: "fk_subscription_usages_features_feature_id",
                        column: x => x.feature_id,
                        principalSchema: "subscription",
                        principalTable: "features",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_subscription_usages_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalSchema: "subscription",
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "media_assets",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    storage_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media_assets", x => x.id);
                    table.ForeignKey(
                        name: "fk_media_assets_videos_video_id",
                        column: x => x.video_id,
                        principalSchema: "material",
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transcripts",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_text = table.Column<string>(type: "text", nullable: false),
                    source_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    storage_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_reviewed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transcripts", x => x.id);
                    table.ForeignKey(
                        name: "fk_transcripts_videos_video_id",
                        column: x => x.video_id,
                        principalSchema: "material",
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_attempt_items",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quiz_attempt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    selected_answer = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_correct = table.Column<bool>(type: "boolean", nullable: true),
                    time_spent_ms = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    feedback = table.Column<string>(type: "text", nullable: true),
                    answered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quiz_attempt_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_quiz_attempt_items_quiz_attempts_quiz_attempt_id",
                        column: x => x.quiz_attempt_id,
                        principalSchema: "srs",
                        principalTable: "quiz_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quiz_attempt_items_quiz_items_question_id",
                        column: x => x.question_id,
                        principalSchema: "srs",
                        principalTable: "quiz_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transcript_speakers",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transcript_id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    speaker_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transcript_speakers", x => x.id);
                    table.ForeignKey(
                        name: "fk_transcript_speakers_speakers_speaker_id",
                        column: x => x.speaker_id,
                        principalSchema: "shadowing",
                        principalTable: "speakers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_transcript_speakers_transcripts_transcript_id",
                        column: x => x.transcript_id,
                        principalSchema: "material",
                        principalTable: "transcripts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "srs_schedule_details",
                schema: "srs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    srs_schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quiz_attempt_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    flashcard_session_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    review_number = table.Column<int>(type: "integer", nullable: false),
                    quality = table.Column<int>(type: "integer", nullable: false),
                    interval_days = table.Column<int>(type: "integer", nullable: false),
                    easiness_factor = table.Column<double>(type: "double precision", nullable: false),
                    repetitions = table.Column<int>(type: "integer", nullable: false),
                    lapses = table.Column<int>(type: "integer", nullable: false),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_srs_schedule_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_srs_schedule_details_flashcard_session_items_flashcard_sess",
                        column: x => x.flashcard_session_item_id,
                        principalSchema: "srs",
                        principalTable: "flashcard_session_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_srs_schedule_details_quiz_attempt_items_quiz_attempt_item_id",
                        column: x => x.quiz_attempt_item_id,
                        principalSchema: "srs",
                        principalTable: "quiz_attempt_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_srs_schedule_details_srs_schedules_srs_schedule_id",
                        column: x => x.srs_schedule_id,
                        principalSchema: "srs",
                        principalTable: "srs_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transcript_segments",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    transcript_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transcript_speaker_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transcript_segments", x => x.id);
                    table.ForeignKey(
                        name: "fk_transcript_segments_transcript_speakers_transcript_speaker_",
                        column: x => x.transcript_speaker_id,
                        principalSchema: "material",
                        principalTable: "transcript_speakers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_transcript_segments_transcripts_transcript_id",
                        column: x => x.transcript_id,
                        principalSchema: "material",
                        principalTable: "transcripts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bunsetsu",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    start_index = table.Column<int>(type: "integer", nullable: false),
                    end_index = table.Column<int>(type: "integer", nullable: false),
                    transcript_segment_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bunsetsu", x => x.id);
                    table.ForeignKey(
                        name: "fk_bunsetsu_transcript_segments_transcript_segment_id",
                        column: x => x.transcript_segment_id,
                        principalSchema: "material",
                        principalTable: "transcript_segments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "learning_units",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_meaning_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transcript_segment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_token_index = table.Column<int>(type: "integer", nullable: false),
                    end_token_index = table.Column<int>(type: "integer", nullable: false),
                    confidence = table.Column<double>(type: "double precision", nullable: false),
                    is_reviewed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    detection_method = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_learning_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_learning_units_knowledge_meanings_knowledge_meaning_id",
                        column: x => x.knowledge_meaning_id,
                        principalSchema: "knowledge",
                        principalTable: "knowledge_meanings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_learning_units_transcript_segments_transcript_segment_id",
                        column: x => x.transcript_segment_id,
                        principalSchema: "material",
                        principalTable: "transcript_segments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shadowing_materials",
                schema: "shadowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    start_transcript_segment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    end_transcript_segment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_materials", x => x.id);
                    table.ForeignKey(
                        name: "fk_shadowing_materials_transcript_segments_end_transcript_segm",
                        column: x => x.end_transcript_segment_id,
                        principalSchema: "material",
                        principalTable: "transcript_segments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_shadowing_materials_transcript_segments_start_transcript_se",
                        column: x => x.start_transcript_segment_id,
                        principalSchema: "material",
                        principalTable: "transcript_segments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_shadowing_materials_users_created_by_id",
                        column: x => x.created_by_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shadowing_materials_videos_video_id",
                        column: x => x.video_id,
                        principalSchema: "material",
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subtitles",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    transcript_segment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subtitles", x => x.id);
                    table.ForeignKey(
                        name: "fk_subtitles_transcript_segments_transcript_segment_id",
                        column: x => x.transcript_segment_id,
                        principalSchema: "material",
                        principalTable: "transcript_segments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transcript_items",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    transcript_segment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    confidence = table.Column<decimal>(type: "numeric(8,5)", precision: 8, scale: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transcript_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_transcript_items_transcript_segments_transcript_segment_id",
                        column: x => x.transcript_segment_id,
                        principalSchema: "material",
                        principalTable: "transcript_segments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tokens",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    bunsetsu_id = table.Column<Guid>(type: "uuid", nullable: false),
                    surface = table.Column<string>(type: "text", nullable: false),
                    lemma = table.Column<string>(type: "text", nullable: false),
                    reading = table.Column<string>(type: "text", nullable: false),
                    head_sequence = table.Column<int>(type: "integer", nullable: false),
                    japanese_part_of_speech_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dependency_relationship_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_index = table.Column<int>(type: "integer", nullable: false),
                    end_index = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_tokens_bunsetsu_bunsetsu_id",
                        column: x => x.bunsetsu_id,
                        principalSchema: "material",
                        principalTable: "bunsetsu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tokens_dependency_relationships_dependency_relationship_id",
                        column: x => x.dependency_relationship_id,
                        principalSchema: "knowledge",
                        principalTable: "dependency_relationships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tokens_japanese_part_of_speeches_japanese_part_of_speech_id",
                        column: x => x.japanese_part_of_speech_id,
                        principalSchema: "knowledge",
                        principalTable: "japanese_part_of_speeches",
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
                name: "shadowing_role_line_transcript_segment",
                schema: "shadowing",
                columns: table => new
                {
                    shadowing_role_line_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transcript_segment_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_role_line_transcript_segment", x => new { x.shadowing_role_line_id, x.transcript_segment_id });
                    table.ForeignKey(
                        name: "fk_shadowing_role_line_transcript_segment_shadowing_role_lines",
                        column: x => x.shadowing_role_line_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_role_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shadowing_role_line_transcript_segment_transcript_segments_",
                        column: x => x.transcript_segment_id,
                        principalSchema: "material",
                        principalTable: "transcript_segments",
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
                    shadowing_role_id = table.Column<Guid>(type: "uuid", nullable: true),
                    member_id = table.Column<Guid>(type: "uuid", nullable: false),
                    join_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    shadowing_session_invitation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    joined_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shadowing_session_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_shadowing_session_members_shadowing_roles_shadowing_role_id",
                        column: x => x.shadowing_role_id,
                        principalSchema: "shadowing",
                        principalTable: "shadowing_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
                    is_skipped = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
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
                    accuracy_assessment_response = table.Column<string>(type: "text", nullable: true),
                    pitch_similarity_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    pitch_similarity_assessment_response = table.Column<string>(type: "text", nullable: true),
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
                name: "ix_bunsetsu_transcript_segment_id",
                schema: "material",
                table: "bunsetsu",
                column: "transcript_segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_content_categories_code",
                schema: "knowledge",
                table: "content_categories",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_content_categories_slug",
                schema: "knowledge",
                table: "content_categories",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_content_knowledge_collections_content_category_id",
                schema: "knowledge",
                table: "content_knowledge_collections",
                column: "content_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_content_knowledge_collections_knowledge_collection_id",
                schema: "knowledge",
                table: "content_knowledge_collections",
                column: "knowledge_collection_id");

            migrationBuilder.CreateIndex(
                name: "ix_dependency_relationships_code",
                schema: "knowledge",
                table: "dependency_relationships",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_email_verification_tokens_user_id",
                schema: "identity",
                table: "email_verification_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_entitlements_feature_id",
                schema: "subscription",
                table: "entitlements",
                column: "feature_id");

            migrationBuilder.CreateIndex(
                name: "ix_entitlements_plan_id_feature_id",
                schema: "subscription",
                table: "entitlements",
                columns: new[] { "plan_id", "feature_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_features_code",
                schema: "subscription",
                table: "features",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_flashcard_session_cards_saved_item_id",
                schema: "srs",
                table: "flashcard_session_cards",
                column: "saved_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_flashcard_session_items_flashcard_session_id",
                schema: "srs",
                table: "flashcard_session_items",
                column: "flashcard_session_id");

            migrationBuilder.CreateIndex(
                name: "ix_flashcard_session_items_saved_item_id",
                schema: "srs",
                table: "flashcard_session_items",
                column: "saved_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_flashcard_sessions_user_id",
                schema: "srs",
                table: "flashcard_sessions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_interests_content_category_id",
                schema: "identity",
                table: "interests",
                column: "content_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_interests_profile_id_content_category_id",
                schema: "identity",
                table: "interests",
                columns: new[] { "profile_id", "content_category_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_issue_categories_target_type_target_id_category",
                schema: "curation",
                table: "issue_categories",
                columns: new[] { "target_type", "target_id", "category" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_issue_details_report_id",
                schema: "curation",
                table: "issue_details",
                column: "report_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_details_target_type_target_id_category",
                schema: "curation",
                table: "issue_details",
                columns: new[] { "target_type", "target_id", "category" });

            migrationBuilder.CreateIndex(
                name: "ix_issue_details_user_id",
                schema: "curation",
                table: "issue_details",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_japanese_part_of_speeches_code",
                schema: "knowledge",
                table: "japanese_part_of_speeches",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_collections_code",
                schema: "knowledge",
                table: "knowledge_collections",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_meanings_knowledge_unit_id",
                schema: "knowledge",
                table: "knowledge_meanings",
                column: "knowledge_unit_id");

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

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_units_knowledge_collection_id",
                schema: "knowledge",
                table: "knowledge_units",
                column: "knowledge_collection_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_units_proficiency_level_id",
                schema: "knowledge",
                table: "knowledge_units",
                column: "proficiency_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_learning_units_knowledge_meaning_id",
                schema: "material",
                table: "learning_units",
                column: "knowledge_meaning_id");

            migrationBuilder.CreateIndex(
                name: "ix_learning_units_transcript_segment_id",
                schema: "material",
                table: "learning_units",
                column: "transcript_segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_media_assets_video_id",
                schema: "material",
                table: "media_assets",
                column: "video_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_plan_id",
                schema: "subscription",
                table: "payments",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_user_id",
                schema: "subscription",
                table: "payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_plans_code",
                schema: "subscription",
                table: "plans",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_proficiency_levels_code",
                schema: "knowledge",
                table: "proficiency_levels",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_profiles_level_id",
                schema: "identity",
                table: "profiles",
                column: "level_id");

            migrationBuilder.CreateIndex(
                name: "ix_profiles_user_id",
                schema: "identity",
                table: "profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_question_types_code",
                schema: "srs",
                table: "question_types",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_questions_knowledge_meaning_id",
                schema: "srs",
                table: "questions",
                column: "knowledge_meaning_id");

            migrationBuilder.CreateIndex(
                name: "ix_questions_question_type_id",
                schema: "srs",
                table: "questions",
                column: "question_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempt_items_question_id",
                schema: "srs",
                table: "quiz_attempt_items",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempt_items_quiz_attempt_id",
                schema: "srs",
                table: "quiz_attempt_items",
                column: "quiz_attempt_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_quiz_id",
                schema: "srs",
                table: "quiz_attempts",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_user_id",
                schema: "srs",
                table: "quiz_attempts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_items_bank_question_id",
                schema: "srs",
                table: "quiz_items",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_items_quiz_id",
                schema: "srs",
                table: "quiz_items",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_items_saved_item_id",
                schema: "srs",
                table: "quiz_items",
                column: "saved_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_quizzes_user_id",
                schema: "srs",
                table: "quizzes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token_hash",
                schema: "identity",
                table: "refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                schema: "identity",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_reports_status",
                schema: "curation",
                table: "reports",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_reports_target_type_target_id_category",
                schema: "curation",
                table: "reports",
                columns: new[] { "target_type", "target_id", "category" });

            migrationBuilder.CreateIndex(
                name: "ix_roles_code",
                schema: "identity",
                table: "roles",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_saved_items_knowledge_meaning_id",
                schema: "srs",
                table: "saved_items",
                column: "knowledge_meaning_id");

            migrationBuilder.CreateIndex(
                name: "ix_saved_items_user_id_knowledge_meaning_id",
                schema: "srs",
                table: "saved_items",
                columns: new[] { "user_id", "knowledge_meaning_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_series_slug",
                schema: "material",
                table: "series",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_series_user_id",
                schema: "material",
                table: "series",
                column: "user_id");

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
                name: "ix_shadowing_materials_created_by_id",
                schema: "shadowing",
                table: "shadowing_materials",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_materials_end_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials",
                column: "end_transcript_segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_materials_start_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_materials",
                column: "start_transcript_segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_materials_video_id_version",
                schema: "shadowing",
                table: "shadowing_materials",
                columns: new[] { "video_id", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_role_line_transcript_segment_transcript_segment_id",
                schema: "shadowing",
                table: "shadowing_role_line_transcript_segment",
                column: "transcript_segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_shadowing_role_lines_shadowing_role_id_sort_order",
                schema: "shadowing",
                table: "shadowing_role_lines",
                columns: new[] { "shadowing_role_id", "sort_order" });

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
                name: "ix_shadowing_session_members_shadowing_role_id",
                schema: "shadowing",
                table: "shadowing_session_members",
                column: "shadowing_role_id");

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

            migrationBuilder.CreateIndex(
                name: "ix_speakers_code",
                schema: "shadowing",
                table: "speakers",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_srs_schedule_details_flashcard_session_item_id",
                schema: "srs",
                table: "srs_schedule_details",
                column: "flashcard_session_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_srs_schedule_details_quiz_attempt_item_id",
                schema: "srs",
                table: "srs_schedule_details",
                column: "quiz_attempt_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_srs_schedule_details_srs_schedule_id",
                schema: "srs",
                table: "srs_schedule_details",
                column: "srs_schedule_id");

            migrationBuilder.CreateIndex(
                name: "ix_srs_schedules_due_at",
                schema: "srs",
                table: "srs_schedules",
                column: "due_at");

            migrationBuilder.CreateIndex(
                name: "ix_srs_schedules_saved_item_id",
                schema: "srs",
                table: "srs_schedules",
                column: "saved_item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subscription_usages_feature_id",
                schema: "subscription",
                table: "subscription_usages",
                column: "feature_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_usages_subscription_id",
                schema: "subscription",
                table: "subscription_usages",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_payment_id",
                schema: "subscription",
                table: "subscriptions",
                column: "payment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_plan_id",
                schema: "subscription",
                table: "subscriptions",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_user_id",
                schema: "subscription",
                table: "subscriptions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_subtitles_transcript_segment_id",
                schema: "material",
                table: "subtitles",
                column: "transcript_segment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tokens_bunsetsu_id",
                schema: "material",
                table: "tokens",
                column: "bunsetsu_id");

            migrationBuilder.CreateIndex(
                name: "ix_tokens_dependency_relationship_id",
                schema: "material",
                table: "tokens",
                column: "dependency_relationship_id");

            migrationBuilder.CreateIndex(
                name: "ix_tokens_japanese_part_of_speech_id",
                schema: "material",
                table: "tokens",
                column: "japanese_part_of_speech_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_payment_id",
                schema: "subscription",
                table: "transactions",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_txn_ref",
                schema: "subscription",
                table: "transactions",
                column: "txn_ref",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_url",
                schema: "subscription",
                table: "transactions",
                column: "url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transcript_items_transcript_segment_id",
                schema: "material",
                table: "transcript_items",
                column: "transcript_segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_transcript_segments_transcript_id",
                schema: "material",
                table: "transcript_segments",
                column: "transcript_id");

            migrationBuilder.CreateIndex(
                name: "ix_transcript_segments_transcript_speaker_id",
                schema: "material",
                table: "transcript_segments",
                column: "transcript_speaker_id");

            migrationBuilder.CreateIndex(
                name: "ix_transcript_speakers_speaker_id",
                schema: "material",
                table: "transcript_speakers",
                column: "speaker_id");

            migrationBuilder.CreateIndex(
                name: "ix_transcript_speakers_transcript_id",
                schema: "material",
                table: "transcript_speakers",
                column: "transcript_id");

            migrationBuilder.CreateIndex(
                name: "ix_transcripts_video_id",
                schema: "material",
                table: "transcripts",
                column: "video_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                schema: "identity",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                schema: "identity",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_videos_content_category_id",
                schema: "material",
                table: "videos",
                column: "content_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_videos_learner_id",
                schema: "material",
                table: "videos",
                column: "learner_id");

            migrationBuilder.CreateIndex(
                name: "ix_videos_series_id",
                schema: "material",
                table: "videos",
                column: "series_id");

            migrationBuilder.CreateIndex(
                name: "ix_videos_slug",
                schema: "material",
                table: "videos",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "content_knowledge_collections",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "email_verification_tokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "entitlements",
                schema: "subscription");

            migrationBuilder.DropTable(
                name: "flashcard_session_cards",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "interests",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "issue_categories",
                schema: "curation");

            migrationBuilder.DropTable(
                name: "issue_details",
                schema: "curation");

            migrationBuilder.DropTable(
                name: "knowledge_unit_elements",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "learning_units",
                schema: "material");

            migrationBuilder.DropTable(
                name: "media_assets",
                schema: "material");

            migrationBuilder.DropTable(
                name: "refresh_tokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "shadowing_assessment_details",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "shadowing_role_line_transcript_segment",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "srs_schedule_details",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "subscription_usages",
                schema: "subscription");

            migrationBuilder.DropTable(
                name: "subtitles",
                schema: "material");

            migrationBuilder.DropTable(
                name: "tokens",
                schema: "material");

            migrationBuilder.DropTable(
                name: "transactions",
                schema: "subscription");

            migrationBuilder.DropTable(
                name: "transcript_items",
                schema: "material");

            migrationBuilder.DropTable(
                name: "profiles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "reports",
                schema: "curation");

            migrationBuilder.DropTable(
                name: "shadowing_assessments",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "shadowing_attempt_details",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "flashcard_session_items",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "quiz_attempt_items",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "srs_schedules",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "features",
                schema: "subscription");

            migrationBuilder.DropTable(
                name: "subscriptions",
                schema: "subscription");

            migrationBuilder.DropTable(
                name: "bunsetsu",
                schema: "material");

            migrationBuilder.DropTable(
                name: "dependency_relationships",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "japanese_part_of_speeches",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "shadowing_attempts",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "shadowing_role_lines",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "flashcard_sessions",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "quiz_attempts",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "quiz_items",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "payments",
                schema: "subscription");

            migrationBuilder.DropTable(
                name: "shadowing_session_members",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "questions",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "quizzes",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "saved_items",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "plans",
                schema: "subscription");

            migrationBuilder.DropTable(
                name: "shadowing_roles",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "shadowing_session_invitations",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "question_types",
                schema: "srs");

            migrationBuilder.DropTable(
                name: "knowledge_meanings",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "shadowing_sessions",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "knowledge_units",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "shadowing_materials",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "knowledge_collections",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "proficiency_levels",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "transcript_segments",
                schema: "material");

            migrationBuilder.DropTable(
                name: "transcript_speakers",
                schema: "material");

            migrationBuilder.DropTable(
                name: "speakers",
                schema: "shadowing");

            migrationBuilder.DropTable(
                name: "transcripts",
                schema: "material");

            migrationBuilder.DropTable(
                name: "videos",
                schema: "material");

            migrationBuilder.DropTable(
                name: "content_categories",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "series",
                schema: "material");

            migrationBuilder.DropTable(
                name: "users",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "identity");
        }
    }
}
