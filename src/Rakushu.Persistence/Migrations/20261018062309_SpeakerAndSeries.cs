using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rakushu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SpeakerAndSeries : Migration
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
                    description = table.Column<string>(type: "text", nullable: true),
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
                name: "subtitles",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subtitles", x => x.id);
                    table.ForeignKey(
                        name: "fk_subtitles_videos_video_id",
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
                name: "transcript_speakers",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transcript_id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    speaker_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
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
                    transcript_speaker_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_learning_units_transcript_segments_transcript_segment_id",
                        column: x => x.transcript_segment_id,
                        principalSchema: "material",
                        principalTable: "transcript_segments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subtitle_segments",
                schema: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    subtitle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transcript_segment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    original_text = table.Column<string>(type: "text", nullable: false),
                    translated_text = table.Column<string>(type: "text", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subtitle_segments", x => x.id);
                    table.ForeignKey(
                        name: "fk_subtitle_segments_subtitles_subtitle_id",
                        column: x => x.subtitle_id,
                        principalSchema: "material",
                        principalTable: "subtitles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_subtitle_segments_transcript_segments_transcript_segment_id",
                        column: x => x.transcript_segment_id,
                        principalSchema: "material",
                        principalTable: "transcript_segments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "bunsetsu_dependency_relationships",
                schema: "material",
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
                        principalSchema: "material",
                        principalTable: "bunsetsu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bunsetsu_dependency_relationships_bunsetsu_to_bunsetsu_id",
                        column: x => x.to_bunsetsu_id,
                        principalSchema: "material",
                        principalTable: "bunsetsu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bunsetsu_dependency_relationships_dependency_relationships_",
                        column: x => x.dependency_relationship_id,
                        principalSchema: "knowledge",
                        principalTable: "dependency_relationships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateIndex(
                name: "ix_bunsetsu_transcript_segment_id",
                schema: "material",
                table: "bunsetsu",
                column: "transcript_segment_id");

            migrationBuilder.CreateIndex(
                name: "ix_bunsetsu_dependency_relationships_dependency_relationship_id",
                schema: "material",
                table: "bunsetsu_dependency_relationships",
                column: "dependency_relationship_id");

            migrationBuilder.CreateIndex(
                name: "ix_bunsetsu_dependency_relationships_from_bunsetsu_id",
                schema: "material",
                table: "bunsetsu_dependency_relationships",
                column: "from_bunsetsu_id");

            migrationBuilder.CreateIndex(
                name: "ix_bunsetsu_dependency_relationships_to_bunsetsu_id",
                schema: "material",
                table: "bunsetsu_dependency_relationships",
                column: "to_bunsetsu_id");

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
                name: "ix_roles_code",
                schema: "identity",
                table: "roles",
                column: "code",
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
                name: "ix_speakers_code",
                schema: "shadowing",
                table: "speakers",
                column: "code",
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
                name: "ix_subtitle_segments_subtitle_id",
                schema: "material",
                table: "subtitle_segments",
                column: "subtitle_id");

            migrationBuilder.CreateIndex(
                name: "ix_subtitle_segments_transcript_segment_id",
                schema: "material",
                table: "subtitle_segments",
                column: "transcript_segment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subtitles_code",
                schema: "material",
                table: "subtitles",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subtitles_video_id",
                schema: "material",
                table: "subtitles",
                column: "video_id",
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
                name: "bunsetsu_dependency_relationships",
                schema: "material");

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
                name: "interests",
                schema: "identity");

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
                name: "subscription_usages",
                schema: "subscription");

            migrationBuilder.DropTable(
                name: "subtitle_segments",
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
                name: "knowledge_meanings",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "features",
                schema: "subscription");

            migrationBuilder.DropTable(
                name: "subscriptions",
                schema: "subscription");

            migrationBuilder.DropTable(
                name: "subtitles",
                schema: "material");

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
                name: "knowledge_units",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "payments",
                schema: "subscription");

            migrationBuilder.DropTable(
                name: "transcript_segments",
                schema: "material");

            migrationBuilder.DropTable(
                name: "knowledge_collections",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "proficiency_levels",
                schema: "knowledge");

            migrationBuilder.DropTable(
                name: "plans",
                schema: "subscription");

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
