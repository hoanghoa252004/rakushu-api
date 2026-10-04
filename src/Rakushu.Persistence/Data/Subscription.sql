
-- =========================================================
-- FEATURES
-- =========================================================

INSERT INTO features
(
    id,
    code,
    name,
    description,
    status,
    created_at,
    updated_at
)
VALUES
(
    '11111111-1111-1111-1111-111111111111',
    'AI_CHAT',
    'AI Chat',
    'Chat with AI for Japanese learning assistance.',
    'Active',
    NOW(),
    NOW()
),
(
    '22222222-2222-2222-2222-222222222222',
    'VIDEO_GENERATION',
    'Video Generation',
    'Generate AI-enhanced Japanese learning videos.',
    'Active',
    NOW(),
    NOW()
),
(
    '33333333-3333-3333-3333-333333333333',
    'STORAGE',
    'Storage',
    'Cloud storage for uploaded learning materials.',
    'Active',
    NOW(),
    NOW()
),
(
    '44444444-4444-4444-4444-444444444444',
    'PREMIUM_CONTENT',
    'Premium Content',
    'Access to premium Japanese learning content.',
    'Archived',
    NOW(),
    NOW()
);


-- =========================================================
-- PLANS
-- =========================================================

INSERT INTO plans
(
    id,
    code,
    name,
    description,
    price,
    currency,
    billing_cycle,
    status,
    created_at,
    updated_at
)
VALUES
(
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    'MINION',
    'Minion',
    'Plan for basic Japanese learning.',
    10000.00,
    'VND',
    'Monthly',
    'Active',
    NOW(),
    NOW()
),
(
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
    'PRO',
    'Pro',
    'Pro plan for serious Japanese learners.',
    99000.00,
    'VND',
    'Monthly',
    'Archived',
    NOW(),
    NOW()
),
(
    'cccccccc-cccc-cccc-cccc-cccccccccccc',
    'PREMIUM',
    'Premium',
    'Premium plan with advanced AI learning capabilities.',
    199000.00,
    'VND',
    'Monthly',
    'Draft',
    NOW(),
    NOW()
);


-- =========================================================
-- PLAN ENTITLEMENTS
-- =========================================================

-- ---------------------------------------------------------
-- FREE
-- ---------------------------------------------------------

INSERT INTO plan_entitlements
(
    id,
    plan_id,
    feature_id,
    is_enabled,
    limit_value,
    limit_unit,
    limit_period
)
VALUES
(
    'aaaaaaaa-1111-1111-1111-111111111111',
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    '11111111-1111-1111-1111-111111111111',
    true,
    20,
    'Count',
    'Month'
),
(
    'aaaaaaaa-2222-2222-2222-222222222222',
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    '22222222-2222-2222-2222-222222222222',
    false,
    0,
    'Count',
    'Month'
),
(
    'aaaaaaaa-3333-3333-3333-333333333333',
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    '33333333-3333-3333-3333-333333333333',
    true,
    1,
    'GB',
    'Total'
),
(
    'aaaaaaaa-4444-4444-4444-444444444444',
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    '44444444-4444-4444-4444-444444444444',
    false,
    0,
    'Count',
    'Month'
);


-- ---------------------------------------------------------
-- PRO
-- ---------------------------------------------------------

INSERT INTO plan_entitlements
(
    id,
    plan_id,
    feature_id,
    is_enabled,
    limit_value,
    limit_unit,
    limit_period
)
VALUES
(
    'bbbbbbbb-1111-1111-1111-111111111111',
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
    '11111111-1111-1111-1111-111111111111',
    true,
    500,
    'Count',
    'Month'
),
(
    'bbbbbbbb-2222-2222-2222-222222222222',
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
    '22222222-2222-2222-2222-222222222222',
    true,
    20,
    'Count',
    'Month'
),
(
    'bbbbbbbb-3333-3333-3333-333333333333',
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
    '33333333-3333-3333-3333-333333333333',
    true,
    10,
    'GB',
    'Total'
),
(
    'bbbbbbbb-4444-4444-4444-444444444444',
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
    '44444444-4444-4444-4444-444444444444',
    true,
    1,
    'Count',
    'Month'
);


-- ---------------------------------------------------------
-- PREMIUM
-- ---------------------------------------------------------

INSERT INTO plan_entitlements
(
    id,
    plan_id,
    feature_id,
    is_enabled,
    limit_value,
    limit_unit,
    limit_period
)
VALUES
(
    'cccccccc-1111-1111-1111-111111111111',
    'cccccccc-cccc-cccc-cccc-cccccccccccc',
    '11111111-1111-1111-1111-111111111111',
    true,
    2000,
    'Count',
    'Month'
),
(
    'cccccccc-2222-2222-2222-222222222222',
    'cccccccc-cccc-cccc-cccc-cccccccccccc',
    '22222222-2222-2222-2222-222222222222',
    true,
    100,
    'Count',
    'Month'
),
(
    'cccccccc-3333-3333-3333-333333333333',
    'cccccccc-cccc-cccc-cccc-cccccccccccc',
    '33333333-3333-3333-3333-333333333333',
    true,
    50,
    'GB',
    'Total'
),
(
    'cccccccc-4444-4444-4444-444444444444',
    'cccccccc-cccc-cccc-cccc-cccccccccccc',
    '44444444-4444-4444-4444-444444444444',
    true,
    1,
    'Count',
    'Month'
);