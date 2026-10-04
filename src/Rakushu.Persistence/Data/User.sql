-- ============================================
-- Seed data: Proficiency Frameworks
-- ============================================
INSERT INTO proficiency_frameworks (
    id,
    code,
    name,
    description,
    is_active,
    created_at,
    updated_at
)
VALUES
    (
        '10000000-0000-0000-0000-000000000001',
        'JLPT',
        'Japanese Language Proficiency Test',
        'Official Japanese Language Proficiency Test (JLPT) framework with 5 levels from N5 to N1.',
        true,
        NOW(),
        NOW()
    ),
    (
        '10000000-0000-0000-0000-000000000002',
        'BJT',
        'Business Japanese Test',
        'Business Japanese Proficiency Test (BJT) framework with 6 levels from J5 to J1+.',
        true,
        NOW(),
        NOW()
    );


-- ============================================
-- Seed data: Proficiency Levels - JLPT
-- ============================================
INSERT INTO proficiency_levels (
    id,
    proficiency_framework_id,
    code,
    name,
    sort_order,
    description,
    is_active,
    created_at,
    updated_at
)
VALUES
    (
        '20000000-0000-0000-0000-000000000101',
        '10000000-0000-0000-0000-000000000001',
        'N5',
        'N5',
        1,
        'The ability to understand some basic Japanese.',
        true,
        NOW(),
        NOW()
    ),
    (
        '20000000-0000-0000-0000-000000000102',
        '10000000-0000-0000-0000-000000000001',
        'N4',
        'N4',
        2,
        'The ability to understand basic Japanese.',
        true,
        NOW(),
        NOW()
    ),
    (
        '20000000-0000-0000-0000-000000000103',
        '10000000-0000-0000-0000-000000000001',
        'N3',
        'N3',
        3,
        'The ability to understand Japanese used in everyday situations to a certain degree.',
        true,
        NOW(),
        NOW()
    ),
    (
        '20000000-0000-0000-0000-000000000104',
        '10000000-0000-0000-0000-000000000001',
        'N2',
        'N2',
        4,
        'The ability to understand Japanese used in everyday situations and in a variety of circumstances to a certain degree.',
        true,
        NOW(),
        NOW()
    ),
    (
        '20000000-0000-0000-0000-000000000105',
        '10000000-0000-0000-0000-000000000001',
        'N1',
        'N1',
        5,
        'The ability to understand Japanese used in a variety of circumstances.',
        true,
        NOW(),
        NOW()
    );


-- ============================================
-- Seed data: Proficiency Levels - BJT
-- ============================================
-- BJT official levels:
-- J5   : 0-199
-- J4   : 200-319
-- J3   : 320-419
-- J2   : 420-529
-- J1   : 530-599
-- J1+  : 600-800
--
-- sort_order increases with proficiency.
-- ============================================

INSERT INTO proficiency_levels (
    id,
    proficiency_framework_id,
    code,
    name,
    sort_order,
    description,
    is_active,
    created_at,
    updated_at
)
VALUES
    (
        '20000000-0000-0000-0000-000000000201',
        '10000000-0000-0000-0000-000000000002',
        'J5',
        'J5',
        1,
        'Virtually no ability to communicate in Japanese in business situations.',
        true,
        NOW(),
        NOW()
    ),
    (
        '20000000-0000-0000-0000-000000000202',
        '10000000-0000-0000-0000-000000000002',
        'J4',
        'J4',
        2,
        'Able to achieve a minimal degree of communication in Japanese in limited business situations.',
        true,
        NOW(),
        NOW()
    ),
    (
        '20000000-0000-0000-0000-000000000203',
        '10000000-0000-0000-0000-000000000002',
        'J3',
        'J3',
        3,
        'Able to achieve some degree of communication in Japanese in limited business situations.',
        true,
        NOW(),
        NOW()
    ),
    (
        '20000000-0000-0000-0000-000000000204',
        '10000000-0000-0000-0000-000000000002',
        'J2',
        'J2',
        4,
        'Able to communicate appropriately in Japanese in limited business situations.',
        true,
        NOW(),
        NOW()
    ),
    (
        '20000000-0000-0000-0000-000000000205',
        '10000000-0000-0000-0000-000000000002',
        'J1',
        'J1',
        5,
        'Able to communicate appropriately in Japanese in a wide range of business situations.',
        true,
        NOW(),
        NOW()
    ),
    (
        '20000000-0000-0000-0000-000000000206',
        '10000000-0000-0000-0000-000000000002',
        'J1_PLUS',
        'J1+',
        6,
        'Able to communicate sufficiently in Japanese in any business situation.',
        true,
        NOW(),
        NOW()
    );


-- ============================================
-- NOTE:
-- No JLPT <-> BJT equivalence seed is inserted.
--
-- JLPT and BJT are separate proficiency frameworks.
-- BJT provides J5-J1+ levels based on BJT scores.
-- JLPT provides N5-N1 levels.
--
-- If Rakushu wants cross-framework equivalence,
-- create application-defined mappings explicitly
-- and mark them as approximate/non-official.
-- ============================================


-- ============================================
-- Seed data: Supported Languages
-- ============================================
INSERT INTO supported_languages (
    id,
    code,
    name,
    native_name,
    is_active,
    created_at,
    updated_at
)
VALUES
(
    '40000000-0000-0000-0000-000000000001',
    'en',
    'English',
    'English',
    TRUE,
    NOW(),
    NOW()
),
(
    '40000000-0000-0000-0000-000000000002',
    'vi',
    'Vietnamese',
    'Vietnamese',
    TRUE,
    NOW(),
    NOW()
),
(
    '40000000-0000-0000-0000-000000000003',
    'ja',
    'Japanese',
    'Japanese',
    TRUE,
    NOW(),
    NOW()
);


-- ============================================
-- Seed data: Content Categories
-- Hierarchical structure
-- ============================================

INSERT INTO content_categories (
    id,
    slug,
    code,
    name,
    description,
    parent_id,
    level,
    display_order,
    is_active,
    created_at,
    updated_at
)
VALUES

    -- ========================================
    -- Top-level categories
    -- display_order is global among parent_id IS NULL
    -- ========================================

    (
        '50000000-0000-0000-0000-000000000001',
        'learning',
        'LEARNING',
        'Learning & Development',
        'Learning and personal development',
        NULL,
        1,
        1,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000002',
        'entertainment',
        'ENTERTAINMENT',
        'Entertainment & Media',
        'Entertainment, movies, music, and media',
        NULL,
        1,
        2,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000003',
        'business-finance',
        'BUSINESS_FINANCE',
        'Business & Finance',
        'Business, finance, and career topics',
        NULL,
        1,
        3,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000004',
        'health-wellness',
        'HEALTH_WELLNESS',
        'Health & Wellness',
        'Health, sports, and wellness topics',
        NULL,
        1,
        4,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000005',
        'culture-society',
        'CULTURE_SOCIETY',
        'Culture & Society',
        'Culture, history, politics, and social topics',
        NULL,
        1,
        5,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000006',
        'science-nature',
        'SCIENCE_NATURE',
        'Science & Nature',
        'Science, technology, and environment',
        NULL,
        1,
        6,
        TRUE,
        NOW(),
        NOW()
    ),

    -- ========================================
    -- Learning & Development
    -- Parent: LEARNING
    -- ========================================

    (
        '50000000-0000-0000-0000-000000000101',
        'education',
        'EDUCATION',
        'Education',
        'Educational content and academic topics',
        '50000000-0000-0000-0000-000000000001',
        2,
        1,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000102',
        'languages',
        'LANGUAGES',
        'Languages',
        'Language learning and linguistics',
        '50000000-0000-0000-0000-000000000001',
        2,
        2,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000103',
        'personal-development',
        'PERSONAL_DEV',
        'Personal Development',
        'Self-improvement and professional growth',
        '50000000-0000-0000-0000-000000000001',
        2,
        3,
        TRUE,
        NOW(),
        NOW()
    ),

    -- ========================================
    -- Entertainment & Media
    -- Parent: ENTERTAINMENT
    -- ========================================

    (
        '50000000-0000-0000-0000-000000000201',
        'movies-music',
        'MOVIES_MUSIC',
        'Movies & Music',
        'Films, television, and music entertainment',
        '50000000-0000-0000-0000-000000000002',
        2,
        1,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000202',
        'anime-manga',
        'ANIME_MANGA',
        'Anime & Manga',
        'Japanese anime and manga culture',
        '50000000-0000-0000-0000-000000000002',
        2,
        2,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000203',
        'literature',
        'LITERATURE',
        'Literature',
        'Books, novels, and literary works',
        '50000000-0000-0000-0000-000000000002',
        2,
        3,
        TRUE,
        NOW(),
        NOW()
    ),

    -- ========================================
    -- Business & Finance
    -- Parent: BUSINESS_FINANCE
    -- ========================================

    (
        '50000000-0000-0000-0000-000000000301',
        'business',
        'BUSINESS',
        'Business',
        'Business and corporate communication',
        '50000000-0000-0000-0000-000000000003',
        2,
        1,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000302',
        'finance',
        'FINANCE',
        'Finance',
        'Finance, investments, and economics',
        '50000000-0000-0000-0000-000000000003',
        2,
        2,
        TRUE,
        NOW(),
        NOW()
    ),

    -- ========================================
    -- Health & Wellness
    -- ========================================

    (
        '50000000-0000-0000-0000-000000000401',
        'health',
        'HEALTH',
        'Health',
        'Health, medicine, and wellness topics',
        '50000000-0000-0000-0000-000000000004',
        2,
        1,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000402',
        'sports',
        'SPORTS',
        'Sports & Fitness',
        'Sports, fitness, and physical activities',
        '50000000-0000-0000-0000-000000000004',
        2,
        2,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000403',
        'food',
        'FOOD',
        'Food & Cuisine',
        'Cooking, recipes, and food culture',
        '50000000-0000-0000-0000-000000000004',
        2,
        3,
        TRUE,
        NOW(),
        NOW()
    ),

    -- ========================================
    -- Culture & Society
    -- ========================================

    (
        '50000000-0000-0000-0000-000000000501',
        'culture',
        'CULTURE',
        'Culture & Arts',
        'Cultural practices, traditions, and customs',
        '50000000-0000-0000-0000-000000000005',
        2,
        1,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000502',
        'history',
        'HISTORY',
        'History',
        'Historical events, periods, and civilizations',
        '50000000-0000-0000-0000-000000000005',
        2,
        2,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000503',
        'politics',
        'POLITICS',
        'Politics & Society',
        'Politics, government, and current affairs',
        '50000000-0000-0000-0000-000000000005',
        2,
        3,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000504',
        'relationships',
        'RELATIONSHIPS',
        'Relationships',
        'Relationships, social dynamics, and psychology',
        '50000000-0000-0000-0000-000000000005',
        2,
        4,
        TRUE,
        NOW(),
        NOW()
    ),

    -- ========================================
    -- Science & Nature
    -- ========================================

    (
        '50000000-0000-0000-0000-000000000601',
        'technology',
        'TECHNOLOGY',
        'Technology',
        'Technology, computing, and innovation',
        '50000000-0000-0000-0000-000000000006',
        2,
        1,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000602',
        'science',
        'SCIENCE',
        'Science & Research',
        'Science, physics, chemistry, and biology',
        '50000000-0000-0000-0000-000000000006',
        2,
        2,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000603',
        'environment',
        'ENVIRONMENT',
        'Environment & Nature',
        'Environmental issues, ecology, and nature',
        '50000000-0000-0000-0000-000000000006',
        2,
        3,
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000604',
        'art-design',
        'ART_DESIGN',
        'Art & Design',
        'Visual arts, graphic design, and creative work',
        '50000000-0000-0000-0000-000000000006',
        2,
        4,
        TRUE,
        NOW(),
        NOW()
    ),

    -- ========================================
    -- Standalone / Top-level category
    -- ========================================

    (
        '50000000-0000-0000-0000-000000000701',
        'travel',
        'TRAVEL',
        'Travel & Tourism',
        'Travel, tourism, and geographical exploration',
        NULL,
        1,
        7,
        TRUE,
        NOW(),
        NOW()
    );


-- ============================================
-- Seed data: Roles
-- ============================================
INSERT INTO roles (
    id,
    code,
    name,
    description,
    is_active,
    created_at,
    updated_at
)
VALUES
    (
        '11111111-1111-1111-1111-111111111111',
        'SYSTEM_ADMINISTRATOR',
        'System Administrator',
        'System Administrator',
        true,
        NOW(),
        NOW()
    ),
    (
        '22222222-2222-2222-2222-222222222222',
        'LINGUISTIC_CURATOR',
        'Linguistic Curator',
        'Linguistic Curator',
        true,
        NOW(),
        NOW()
    ),
    (
        '33333333-3333-3333-3333-333333333333',
        'LEARNER',
        'Learner',
        'Learner',
        true,
        NOW(),
        NOW()
    );


-- ============================================
-- Seed data: Users
-- ============================================
INSERT INTO users (
    id,
    email,
    password_hash,
    role_id,
    status,
    created_at,
    updated_at
)
VALUES
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        'admin@rakushu.com',
        'rakushu@1',
        '11111111-1111-1111-1111-111111111111',
        'Active',
        NOW(),
        NOW()
    ),
    (
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        'curator@rakushu.com',
        'rakushu@1',
        '22222222-2222-2222-2222-222222222222',
        'Active',
        NOW(),
        NOW()
    ),
    (
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        'learner1@rakushu.com',
        'rakushu@1',
        '33333333-3333-3333-3333-333333333333',
        'Active',
        NOW(),
        NOW()
    ),
    (
        'dddddddd-dddd-dddd-dddd-dddddddddddd',
        'learner2@rakushu.com',
        'rakushu@1',
        '33333333-3333-3333-3333-333333333333',
        'Active',
        NOW(),
        NOW()
    );


-- ============================================
-- Seed data: Profiles
-- ============================================
-- Profiles are only created for Learner users.
-- System Administrator and Linguistic Curator do not require profiles.

INSERT INTO profiles (
    id,
    user_id,
    full_name,
    avatar_key,
    native_language_id,
    current_level_id,
    target_level_id,
    daily_learning_minutes,
    session_duration_minutes,
    created_at,
    updated_at
)
VALUES
    (
        '60000000-0000-0000-0000-000000000003',
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        'Learner One',
        NULL,
        '40000000-0000-0000-0000-000000000002',
        '20000000-0000-0000-0000-000000000101',
        '20000000-0000-0000-0000-000000000103',
        90,
        45,
        NOW(),
        NOW()
    ),
(
        '60000000-0000-0000-0000-000000000004',
        'dddddddd-dddd-dddd-dddd-dddddddddddd',
        'Learner Two',
        NULL,
        '40000000-0000-0000-0000-000000000003',
        '20000000-0000-0000-0000-000000000101',
        '20000000-0000-0000-0000-000000000103',
        75,
        40,
        NOW(),
        NOW()
    );


-- ============================================
-- Seed data: User Interests
-- ============================================
INSERT INTO interests (
    id,
    profile_id,
    content_category_id,
    priority
)
VALUES
    -- Learner One
    (
        '70000000-0000-0000-0000-000000000001',
        '60000000-0000-0000-0000-000000000003',
        '50000000-0000-0000-0000-000000000701',
        3
    ),
    (
        '70000000-0000-0000-0000-000000000002',
        '60000000-0000-0000-0000-000000000003',
        '50000000-0000-0000-0000-000000000403',
        2
    ),
    (
        '70000000-0000-0000-0000-000000000003',
        '60000000-0000-0000-0000-000000000003',
        '50000000-0000-0000-0000-000000000201',
        1
    ),

    -- Learner Two
    (
        '70000000-0000-0000-0000-000000000004',
        '60000000-0000-0000-0000-000000000004',
        '50000000-0000-0000-0000-000000000202',
        3
    ),
    (
        '70000000-0000-0000-0000-000000000005',
        '60000000-0000-0000-0000-000000000004',
        '50000000-0000-0000-0000-000000000601',
        2
    ),
    (
        '70000000-0000-0000-0000-000000000006',
        '60000000-0000-0000-0000-000000000004',
        '50000000-0000-0000-0000-000000000402',
        1
    );
