-- ============================================
-- Seed data: Roles
-- ============================================
INSERT INTO roles (id, title, description, created_at, updated_at)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'SystemAdministrator', 'System Administrator', NOW(), NOW()),
    ('22222222-2222-2222-2222-222222222222', 'LinguisticCurator', 'Linguistic Curator', NOW(), NOW()),
    ('33333333-3333-3333-3333-333333333333', 'Learner', 'Learner', NOW(), NOW());

-- ============================================
-- Seed data: Users
-- ============================================
INSERT INTO users (
    id, email, password_hash, role_id, status,
    profile_full_name, profile_avatar_key, profile_native_language,
    created_at, updated_at
)
VALUES
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        'admin@rakushu.com',
        'rakushu@1',
        '11111111-1111-1111-1111-111111111111',
        'Active',
        'System Administrator',
        NULL,
        'English',
        NOW(), NOW()
    ),
    (
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        'curator@rakushu.com',
        'rakushu@1',
        '22222222-2222-2222-2222-222222222222',
        'Active',
        'Linguistic Curator',
        NULL,
        'English',
        NOW(), NOW()
    ),
    (
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        'learner1@rakushu.com',
        'rakushu@1',
        '33333333-3333-3333-3333-333333333333',
        'Active',
        'Learner One',
        NULL,
        'Vietnamese',
        NOW(), NOW()
    ),
    (
        'dddddddd-dddd-dddd-dddd-dddddddddddd',
        'learner2@rakushu.com',
        'rakushu@1',
        '33333333-3333-3333-3333-333333333333',
        'Active',
        'Learner Two',
        NULL,
        'Japanese',
        NOW(), NOW()
    );