using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class KnowledgeDomainModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "content_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    level = table.Column<int>(type: "integer", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_content_categories_content_categories_parent_id",
                        column: x => x.parent_id,
                        principalTable: "content_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dependency_relationships",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    vietnamese_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dependency_relationships", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "features",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_features", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "japanese_conjugation_forms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    vietnamese_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_japanese_conjugation_forms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "japanese_part_of_speeches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    vietnamese_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_japanese_part_of_speeches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "linguistic_knowledges",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_type = table.Column<string>(type: "text", nullable: false),
                    expression = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    reading = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    source_type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_linguistic_knowledges", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    billing_cycle = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "proficiency_frameworks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_proficiency_frameworks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "supported_languages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    native_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_supported_languages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "universal_part_of_speeches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    vietnamese_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_universal_part_of_speeches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "content_processing_policy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_processing_policy", x => x.id);
                    table.ForeignKey(
                        name: "fk_content_processing_policy_content_categories_content_catego",
                        column: x => x.content_category_id,
                        principalTable: "content_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "series",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    source_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    content_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_series", x => x.id);
                    table.ForeignKey(
                        name: "fk_series_content_categories_content_category_id",
                        column: x => x.content_category_id,
                        principalTable: "content_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "knowledge_meanings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    linguistic_knowledge_id = table.Column<Guid>(type: "uuid", nullable: false),
                    universal_meaning = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_knowledge_meanings", x => x.id);
                    table.ForeignKey(
                        name: "fk_knowledge_meanings_linguistic_knowledges_linguistic_knowled",
                        column: x => x.linguistic_knowledge_id,
                        principalTable: "linguistic_knowledges",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "knowledge_patterns",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    linguistic_knowledge_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_knowledge_patterns", x => x.id);
                    table.ForeignKey(
                        name: "fk_knowledge_patterns_linguistic_knowledges_linguistic_knowled",
                        column: x => x.linguistic_knowledge_id,
                        principalTable: "linguistic_knowledges",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_entitlements",
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
                    table.PrimaryKey("pk_plan_entitlements", x => x.id);
                    table.ForeignKey(
                        name: "fk_plan_entitlements_features_feature_id",
                        column: x => x.feature_id,
                        principalTable: "features",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_plan_entitlements_plans_plan_id",
                        column: x => x.plan_id,
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "proficiency_levels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    framework_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_proficiency_levels", x => x.id);
                    table.ForeignKey(
                        name: "fk_proficiency_levels_proficiency_frameworks_framework_id",
                        column: x => x.framework_id,
                        principalTable: "proficiency_frameworks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
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
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "knowledge_meaning_detail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_meaning_id = table.Column<Guid>(type: "uuid", nullable: false),
                    supported_language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    translated_meaning = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    native_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_knowledge_meaning_detail", x => x.id);
                    table.ForeignKey(
                        name: "fk_knowledge_meaning_detail_knowledge_meanings_knowledge_meani",
                        column: x => x.knowledge_meaning_id,
                        principalTable: "knowledge_meanings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_knowledge_meaning_detail_supported_languages_supported_lang",
                        column: x => x.supported_language_id,
                        principalTable: "supported_languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "knowledge_pattern_elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_pattern_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    role = table.Column<int>(type: "integer", maxLength: 100, nullable: false),
                    scope = table.Column<int>(type: "integer", maxLength: 100, nullable: false),
                    surface = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    lemma = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    universal_part_of_speech_id = table.Column<Guid>(type: "uuid", nullable: true),
                    japanese_part_of_speech_id = table.Column<Guid>(type: "uuid", nullable: true),
                    japanese_conjugation_form_id = table.Column<Guid>(type: "uuid", nullable: true),
                    particle = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    slot_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_knowledge_pattern_elements", x => x.id);
                    table.ForeignKey(
                        name: "fk_knowledge_pattern_elements_japanese_conjugation_forms_japan",
                        column: x => x.japanese_conjugation_form_id,
                        principalTable: "japanese_conjugation_forms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_knowledge_pattern_elements_japanese_part_of_speeches_japane",
                        column: x => x.japanese_part_of_speech_id,
                        principalTable: "japanese_part_of_speeches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_knowledge_pattern_elements_knowledge_patterns_knowledge_pat",
                        column: x => x.knowledge_pattern_id,
                        principalTable: "knowledge_patterns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_knowledge_pattern_elements_universal_part_of_speeches_unive",
                        column: x => x.universal_part_of_speech_id,
                        principalTable: "universal_part_of_speeches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "proficiency_equivalences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    note = table.Column<string>(type: "text", nullable: true),
                    reference = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_proficiency_equivalences", x => x.id);
                    table.ForeignKey(
                        name: "fk_proficiency_equivalences_proficiency_levels_source_level_id",
                        column: x => x.source_level_id,
                        principalTable: "proficiency_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_proficiency_equivalences_proficiency_levels_target_level_id",
                        column: x => x.target_level_id,
                        principalTable: "proficiency_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "email_verification_token",
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
                    table.PrimaryKey("pk_email_verification_token", x => x.id);
                    table.ForeignKey(
                        name: "fk_email_verification_token_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
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
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_payments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    avatar_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    native_language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    daily_learning_minutes = table.Column<int>(type: "integer", nullable: false),
                    session_duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_profiles", x => x.id);
                    table.ForeignKey(
                        name: "fk_profiles_proficiency_levels_current_level_id",
                        column: x => x.current_level_id,
                        principalTable: "proficiency_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_profiles_proficiency_levels_target_level_id",
                        column: x => x.target_level_id,
                        principalTable: "proficiency_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_profiles_supported_languages_native_language_id",
                        column: x => x.native_language_id,
                        principalTable: "supported_languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
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
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "fk_subscriptions_plans_plan_id",
                        column: x => x.plan_id,
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_subscriptions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "videos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    content_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    source_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    source_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_videos", x => x.id);
                    table.ForeignKey(
                        name: "fk_videos_content_categories_content_category_id",
                        column: x => x.content_category_id,
                        principalTable: "content_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_videos_series_series_id",
                        column: x => x.series_id,
                        principalTable: "series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_videos_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "knowledge_pattern_relations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    knowledge_pattern_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_element_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_element_id = table.Column<Guid>(type: "uuid", nullable: false),
                    relation_type = table.Column<string>(type: "text", nullable: false),
                    dependency_relationship_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_knowledge_pattern_relations", x => x.id);
                    table.ForeignKey(
                        name: "fk_knowledge_pattern_relations_dependency_relationships_depend",
                        column: x => x.dependency_relationship_id,
                        principalTable: "dependency_relationships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_knowledge_pattern_relations_knowledge_pattern_elements_from",
                        column: x => x.from_element_id,
                        principalTable: "knowledge_pattern_elements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_knowledge_pattern_relations_knowledge_pattern_elements_to_e",
                        column: x => x.to_element_id,
                        principalTable: "knowledge_pattern_elements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_knowledge_pattern_relations_knowledge_patterns_knowledge_pa",
                        column: x => x.knowledge_pattern_id,
                        principalTable: "knowledge_patterns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
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
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "interests",
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
                        principalTable: "content_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_interests_profiles_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription_usages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    period_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    used_value = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_usages", x => x.id);
                    table.ForeignKey(
                        name: "fk_subscription_usages_features_feature_id",
                        column: x => x.feature_id,
                        principalTable: "features",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_subscription_usages_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "media_assets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    storage_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    container = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    codec = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    width = table.Column<int>(type: "integer", nullable: true),
                    height = table.Column<int>(type: "integer", nullable: true),
                    frame_rate = table.Column<double>(type: "double precision", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media_assets", x => x.id);
                    table.ForeignKey(
                        name: "fk_media_assets_videos_video_id",
                        column: x => x.video_id,
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subtitles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    supported_language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    storage_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subtitles", x => x.id);
                    table.ForeignKey(
                        name: "fk_subtitles_supported_languages_supported_language_id",
                        column: x => x.supported_language_id,
                        principalTable: "supported_languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_subtitles_videos_video_id",
                        column: x => x.video_id,
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transcripts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_text = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transcripts", x => x.id);
                    table.ForeignKey(
                        name: "fk_transcripts_videos_video_id",
                        column: x => x.video_id,
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transcript_segments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    transcript_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transcript_segments", x => x.id);
                    table.ForeignKey(
                        name: "fk_transcript_segments_transcripts_transcript_id",
                        column: x => x.transcript_id,
                        principalTable: "transcripts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bunsetsu",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    start_index = table.Column<int>(type: "integer", nullable: false),
                    end_index = table.Column<int>(type: "integer", nullable: false),
                    transcript_segment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bunsetsu", x => x.id);
                    table.ForeignKey(
                        name: "fk_bunsetsu_transcript_segments_transcript_segment_id",
                        column: x => x.transcript_segment_id,
                        principalTable: "transcript_segments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "learning_units",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    linguistic_knowledge_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transcript_segment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_token_index = table.Column<int>(type: "integer", nullable: false),
                    end_token_index = table.Column<int>(type: "integer", nullable: false),
                    confidence = table.Column<double>(type: "double precision", nullable: false),
                    detection_method = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    detected_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_learning_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_learning_units_linguistic_knowledges_linguistic_knowledge_id",
                        column: x => x.linguistic_knowledge_id,
                        principalTable: "linguistic_knowledges",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_learning_units_transcript_segments_transcript_segment_id",
                        column: x => x.transcript_segment_id,
                        principalTable: "transcript_segments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subtitle_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subtitle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transcript_segment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    original_text = table.Column<string>(type: "text", nullable: false),
                    translated_text = table.Column<string>(type: "text", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subtitle_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_subtitle_items_subtitles_subtitle_id",
                        column: x => x.subtitle_id,
                        principalTable: "subtitles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_subtitle_items_transcript_segments_transcript_segment_id",
                        column: x => x.transcript_segment_id,
                        principalTable: "transcript_segments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bunsetsu_dependency_relationships",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_bunsetsu_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_bunsetsu_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dependency_relationship_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bunsetsu_dependency_relationships", x => x.id);
                    table.ForeignKey(
                        name: "fk_bunsetsu_dependency_relationships_bunsetsu_from_bunsetsu_id",
                        column: x => x.from_bunsetsu_id,
                        principalTable: "bunsetsu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bunsetsu_dependency_relationships_bunsetsu_to_bunsetsu_id",
                        column: x => x.to_bunsetsu_id,
                        principalTable: "bunsetsu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bunsetsu_dependency_relationships_dependency_relationships_",
                        column: x => x.dependency_relationship_id,
                        principalTable: "dependency_relationships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bunsetsu_id = table.Column<Guid>(type: "uuid", nullable: false),
                    surface = table.Column<string>(type: "text", nullable: false),
                    lemma = table.Column<string>(type: "text", nullable: false),
                    reading = table.Column<string>(type: "text", nullable: false),
                    japanese_part_of_speech_id = table.Column<Guid>(type: "uuid", nullable: false),
                    universal_part_of_speech_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dependency_relationship_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_index = table.Column<int>(type: "integer", nullable: false),
                    end_index = table.Column<int>(type: "integer", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_tokens_bunsetsu_bunsetsu_id",
                        column: x => x.bunsetsu_id,
                        principalTable: "bunsetsu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tokens_dependency_relationships_dependency_relationship_id",
                        column: x => x.dependency_relationship_id,
                        principalTable: "dependency_relationships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tokens_japanese_part_of_speeches_japanese_part_of_speech_id",
                        column: x => x.japanese_part_of_speech_id,
                        principalTable: "japanese_part_of_speeches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tokens_universal_part_of_speeches_universal_part_of_speech_",
                        column: x => x.universal_part_of_speech_id,
                        principalTable: "universal_part_of_speeches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bunsetsu_transcript_segment_id",
                table: "bunsetsu",
                column: "transcript_segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_bunsetsu_dependency_relationships_dependency_relationship_id",
                table: "bunsetsu_dependency_relationships",
                column: "dependency_relationship_id");

            migrationBuilder.CreateIndex(
                name: "ix_bunsetsu_dependency_relationships_from_bunsetsu_id",
                table: "bunsetsu_dependency_relationships",
                column: "from_bunsetsu_id");

            migrationBuilder.CreateIndex(
                name: "ix_bunsetsu_dependency_relationships_to_bunsetsu_id",
                table: "bunsetsu_dependency_relationships",
                column: "to_bunsetsu_id");

            migrationBuilder.CreateIndex(
                name: "ix_content_categories_code",
                table: "content_categories",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_content_categories_parent_id",
                table: "content_categories",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_content_categories_slug",
                table: "content_categories",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_content_processing_policy_content_category_id",
                table: "content_processing_policy",
                column: "content_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_dependency_relationships_code",
                table: "dependency_relationships",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_email_verification_token_user_id",
                table: "email_verification_token",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_features_code",
                table: "features",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_interests_content_category_id",
                table: "interests",
                column: "content_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_interests_profile_id",
                table: "interests",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_japanese_conjugation_forms_code",
                table: "japanese_conjugation_forms",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_japanese_part_of_speeches_code",
                table: "japanese_part_of_speeches",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_meaning_detail_knowledge_meaning_id",
                table: "knowledge_meaning_detail",
                column: "knowledge_meaning_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_meaning_detail_supported_language_id",
                table: "knowledge_meaning_detail",
                column: "supported_language_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_meanings_linguistic_knowledge_id",
                table: "knowledge_meanings",
                column: "linguistic_knowledge_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_pattern_elements_japanese_conjugation_form_id",
                table: "knowledge_pattern_elements",
                column: "japanese_conjugation_form_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_pattern_elements_japanese_part_of_speech_id",
                table: "knowledge_pattern_elements",
                column: "japanese_part_of_speech_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_pattern_elements_knowledge_pattern_id",
                table: "knowledge_pattern_elements",
                column: "knowledge_pattern_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_pattern_elements_universal_part_of_speech_id",
                table: "knowledge_pattern_elements",
                column: "universal_part_of_speech_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_pattern_relations_dependency_relationship_id",
                table: "knowledge_pattern_relations",
                column: "dependency_relationship_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_pattern_relations_from_element_id",
                table: "knowledge_pattern_relations",
                column: "from_element_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_pattern_relations_knowledge_pattern_id",
                table: "knowledge_pattern_relations",
                column: "knowledge_pattern_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_pattern_relations_to_element_id",
                table: "knowledge_pattern_relations",
                column: "to_element_id");

            migrationBuilder.CreateIndex(
                name: "ix_knowledge_patterns_linguistic_knowledge_id",
                table: "knowledge_patterns",
                column: "linguistic_knowledge_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_learning_units_linguistic_knowledge_id",
                table: "learning_units",
                column: "linguistic_knowledge_id");

            migrationBuilder.CreateIndex(
                name: "ix_learning_units_transcript_segment_id",
                table: "learning_units",
                column: "transcript_segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_media_assets_video_id",
                table: "media_assets",
                column: "video_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_plan_id",
                table: "payments",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_user_id",
                table: "payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_entitlements_feature_id",
                table: "plan_entitlements",
                column: "feature_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_entitlements_plan_id_feature_id",
                table: "plan_entitlements",
                columns: new[] { "plan_id", "feature_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_plans_code",
                table: "plans",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_proficiency_equivalences_source_level_id",
                table: "proficiency_equivalences",
                column: "source_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_proficiency_equivalences_target_level_id",
                table: "proficiency_equivalences",
                column: "target_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_proficiency_frameworks_code",
                table: "proficiency_frameworks",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_proficiency_levels_framework_id",
                table: "proficiency_levels",
                column: "framework_id");

            migrationBuilder.CreateIndex(
                name: "ix_profiles_current_level_id",
                table: "profiles",
                column: "current_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_profiles_native_language_id",
                table: "profiles",
                column: "native_language_id");

            migrationBuilder.CreateIndex(
                name: "ix_profiles_target_level_id",
                table: "profiles",
                column: "target_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_profiles_user_id",
                table: "profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token_hash",
                table: "refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_title",
                table: "roles",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_series_content_category_id",
                table: "series",
                column: "content_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_series_slug",
                table: "series",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subscription_usages_feature_id",
                table: "subscription_usages",
                column: "feature_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_usages_subscription_id",
                table: "subscription_usages",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_plan_id",
                table: "subscriptions",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_user_id",
                table: "subscriptions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_subtitle_items_subtitle_id",
                table: "subtitle_items",
                column: "subtitle_id");

            migrationBuilder.CreateIndex(
                name: "ix_subtitle_items_transcript_segment_id",
                table: "subtitle_items",
                column: "transcript_segment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subtitles_supported_language_id",
                table: "subtitles",
                column: "supported_language_id");

            migrationBuilder.CreateIndex(
                name: "ix_subtitles_video_id",
                table: "subtitles",
                column: "video_id");

            migrationBuilder.CreateIndex(
                name: "ix_supported_languages_code",
                table: "supported_languages",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tokens_bunsetsu_id",
                table: "tokens",
                column: "bunsetsu_id");

            migrationBuilder.CreateIndex(
                name: "ix_tokens_dependency_relationship_id",
                table: "tokens",
                column: "dependency_relationship_id");

            migrationBuilder.CreateIndex(
                name: "ix_tokens_japanese_part_of_speech_id",
                table: "tokens",
                column: "japanese_part_of_speech_id");

            migrationBuilder.CreateIndex(
                name: "ix_tokens_universal_part_of_speech_id",
                table: "tokens",
                column: "universal_part_of_speech_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_payment_id",
                table: "transactions",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_txn_ref",
                table: "transactions",
                column: "txn_ref",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_url",
                table: "transactions",
                column: "url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transcript_segments_transcript_id",
                table: "transcript_segments",
                column: "transcript_id");

            migrationBuilder.CreateIndex(
                name: "ix_transcripts_video_id",
                table: "transcripts",
                column: "video_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_universal_part_of_speeches_code",
                table: "universal_part_of_speeches",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_videos_content_category_id",
                table: "videos",
                column: "content_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_videos_created_by",
                table: "videos",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_videos_series_id",
                table: "videos",
                column: "series_id");

            migrationBuilder.CreateIndex(
                name: "ix_videos_slug",
                table: "videos",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bunsetsu_dependency_relationships");

            migrationBuilder.DropTable(
                name: "content_processing_policy");

            migrationBuilder.DropTable(
                name: "email_verification_token");

            migrationBuilder.DropTable(
                name: "interests");

            migrationBuilder.DropTable(
                name: "knowledge_meaning_detail");

            migrationBuilder.DropTable(
                name: "knowledge_pattern_relations");

            migrationBuilder.DropTable(
                name: "learning_units");

            migrationBuilder.DropTable(
                name: "media_assets");

            migrationBuilder.DropTable(
                name: "plan_entitlements");

            migrationBuilder.DropTable(
                name: "proficiency_equivalences");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "subscription_usages");

            migrationBuilder.DropTable(
                name: "subtitle_items");

            migrationBuilder.DropTable(
                name: "tokens");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "profiles");

            migrationBuilder.DropTable(
                name: "knowledge_meanings");

            migrationBuilder.DropTable(
                name: "knowledge_pattern_elements");

            migrationBuilder.DropTable(
                name: "features");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "subtitles");

            migrationBuilder.DropTable(
                name: "bunsetsu");

            migrationBuilder.DropTable(
                name: "dependency_relationships");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "proficiency_levels");

            migrationBuilder.DropTable(
                name: "japanese_conjugation_forms");

            migrationBuilder.DropTable(
                name: "japanese_part_of_speeches");

            migrationBuilder.DropTable(
                name: "knowledge_patterns");

            migrationBuilder.DropTable(
                name: "universal_part_of_speeches");

            migrationBuilder.DropTable(
                name: "supported_languages");

            migrationBuilder.DropTable(
                name: "transcript_segments");

            migrationBuilder.DropTable(
                name: "plans");

            migrationBuilder.DropTable(
                name: "proficiency_frameworks");

            migrationBuilder.DropTable(
                name: "linguistic_knowledges");

            migrationBuilder.DropTable(
                name: "transcripts");

            migrationBuilder.DropTable(
                name: "videos");

            migrationBuilder.DropTable(
                name: "series");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "content_categories");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
