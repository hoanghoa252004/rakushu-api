-- ====================================================================================================
-- RAKUSHU STANDALONE SRS & QUIZ SEED SCRIPT (PostgreSQL)
-- Fully Self-Contained: Has zero dependency on any other seed file.
-- Dedicated Learner Account:
--   - Email: srs.learner@rakushu.com
--   - Password: rakushu@1
--   - User ID: c1111111-1111-1111-1111-111111111111
-- Contains:
--   1. Role & Proficiency Level prerequisites (Idempotent)
--   2. Dedicated SRS Learner Account & Profile
--   3. 20 Knowledge Units & Meanings (IT & Business Japanese)
--   4. 3 Question Types (Multiple Choice, Furigana Reading, Listening)
--   5. 20 Bank Questions with JSONB choices & explanations
--   6. 20 Saved Items for the Dedicated Learner
--   7. 20 SRS Schedules with diverse SM-2 statuses:
--        * 4 Due_today  (Items 1-4: đến hạn hôm nay)
--        * 4 Overdue    (Items 5-8: quá hạn 2-7 ngày)
--        * 4 Learning   (Items 9-12: đang học, hạn ngày mai/ngày kia)
--        * 4 Review     (Items 13-16: thành thạo, Repetitions >= 3, chu kỳ 5-20 ngày)
--        * 2 Suspended  (Items 17-18: tạm dừng ôn tập)
--        * 2 Ignored    (Items 19-20: đã xóa khỏi notebook - soft delete)
--   8. 30 SRS Item Details (SM-2 review logs with Quality 1-5, EF, Intervals, Lapses)
--   9. Sample Completed Quiz Attempt (Ready for GET result & POST retry APIs)
-- Idempotent: Can be run multiple times safely via ON CONFLICT DO NOTHING.
-- ====================================================================================================

BEGIN;

-- ====================================================================================================
-- 1. PREREQUISITES: ROLE & PROFICIENCY LEVEL
-- ====================================================================================================
INSERT INTO identity.roles (id, code, name, description, is_active, created_at, updated_at)
VALUES (
    '33333333-3333-3333-3333-333333333333',
    'LEARNER',
    'Learner',
    'Học viên học từ vựng và làm bài tập SRS',
    TRUE,
    NOW(),
    NOW()
)
ON CONFLICT (id) DO NOTHING;

INSERT INTO knowledge.proficiency_levels (id, code, name, japanese_name, sort_order, description, is_active, created_at, updated_at)
VALUES (
    '20000000-0000-0000-0000-000000000101',
    'N5',
    'Sơ cấp',
    '初級',
    1,
    'Trình độ tiếng Nhật sơ cấp N5',
    TRUE,
    NOW(),
    NOW()
)
ON CONFLICT (id) DO NOTHING;


-- ====================================================================================================
-- 2. DEDICATED SRS LEARNER ACCOUNT & PROFILE
-- ====================================================================================================
-- Email: srs.learner@rakushu.com | Password: rakushu@1
INSERT INTO identity.users (
    id,
    full_name,
    email,
    password_hash,
    role_id,
    status,
    created_at,
    updated_at
)
VALUES (
    'c1111111-1111-1111-1111-111111111111',
    'SRS Dedicated Learner',
    'srs.learner@rakushu.com',
    '52EEF92085E496C50A456A45AB6AF67E536D4A3D8F85CBDD725D76EB287AD343:BB0C48EDEE22C7C5D20DC5CAED72C2DC:100000:SHA512',
    '33333333-3333-3333-3333-333333333333',
    'Active',
    NOW(),
    NOW()
)
ON CONFLICT (id) DO NOTHING;

INSERT INTO identity.profiles (
    id,
    user_id,
    avatar_key,
    level_id,
    daily_learning_minutes,
    session_duration_minutes,
    created_at,
    updated_at
)
VALUES (
    '61111111-1111-1111-1111-111111111111',
    'c1111111-1111-1111-1111-111111111111',
    NULL,
    '20000000-0000-0000-0000-000000000101',
    20,
    10,
    NOW(),
    NOW()
)
ON CONFLICT (id) DO NOTHING;


-- ====================================================================================================
-- 3. KNOWLEDGE COLLECTION, UNITS & MEANINGS (20 VOCABULARY TERMS)
-- ====================================================================================================
INSERT INTO knowledge.knowledge_collections (id, code, name, description, collection_type, scope, is_active, created_at, updated_at)
VALUES (
    '30000000-0000-0000-0000-000000000001',
    'IT_ABBREVIATIONS',
    'IT Từ viết tắt',
    'Các từ viết tắt và thuật ngữ phổ biến trong ngành CNTT tiếng Nhật',
    'Terminology',
    'Domain',
    TRUE,
    NOW(),
    NOW()
)
ON CONFLICT (id) DO NOTHING;

INSERT INTO knowledge.knowledge_units (id, knowledge_type, knowledge_collection_id, proficiency_level_id, expression, reading, source_type, is_active, created_at, updated_at)
VALUES
    ('40000000-0000-0000-0000-000000000001', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'AI', 'エーアイ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000002', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'API', 'エーピーアイ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000003', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'AWS', 'アマゾンウェブサービス', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000004', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'BA', 'ビーエー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000005', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'DB', 'データベース', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000008', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'IT', 'アイティー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000010', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'OS', 'オーエス', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000013', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'PM', 'ピーエム', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000015', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'Q&A', 'キューアンドエー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000017', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'SE', 'エスイー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000021', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'UI', 'ユーアイ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000022', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'UT', 'ユーティー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000024', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', '結合テスト', 'けつごうテスト', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000025', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', '単体テスト', 'たんたいテスト', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000026', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', '要件定義', 'ようけんていぎ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000027', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'システム開発', 'システムかいはつ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000028', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', 'データ分析', 'データぶんせき', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000038', 'Vocabulary', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', '出張', 'しゅっちょう', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000041', 'Vocabulary', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', '名刺交換', 'めいしこうかん', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000047', 'Vocabulary', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000101', '弊社', 'へいしゃ', 'Defined', TRUE, NOW(), NOW())
ON CONFLICT (id) DO NOTHING;

INSERT INTO knowledge.knowledge_meanings (id, sort_order, meaning, japanese_meaning, description, knowledge_unit_id)
VALUES
    ('50000000-0000-0000-0000-000000000001', 1, 'Trí tuệ nhân tạo', '人間の知的な能力をコンピュータで再現する技術。人工知能。', 'Artificial Intelligence. Công nghệ mô phỏng trí tuệ nhân tạo.', '40000000-0000-0000-0000-000000000001'),
    ('50000000-0000-0000-0000-000000000002', 1, 'Giao diện lập trình ứng dụng', 'ソフトウェア同士が機能やデータをやり取りするための接続の仕組み。', 'Application Programming Interface. Giao diện kết nối phần mềm.', '40000000-0000-0000-0000-000000000002'),
    ('50000000-0000-0000-0000-000000000003', 1, 'Dịch vụ web Amazon', 'Amazonが提供するクラウドコンピューティングサービス。', 'Amazon Web Services. Nền tảng điện toán đám mây.', '40000000-0000-0000-0000-000000000003'),
    ('50000000-0000-0000-0000-000000000004', 1, 'Người phân tích nghiệp vụ', '業務や要件を分析し、システムに必要な要求を整理する担当者。', 'Business Analyst. Người phân tích yêu cầu nghiệp vụ.', '40000000-0000-0000-0000-000000000004'),
    ('50000000-0000-0000-0000-000000000005', 1, 'Cơ sở dữ liệu', 'データを整理して保存・管理する仕組み。データベース。', 'Database. Hệ thống lưu trữ và quản lý dữ liệu.', '40000000-0000-0000-0000-000000000005'),
    ('50000000-0000-0000-0000-000000000008', 1, 'Công nghệ thông tin', '情報の処理や通信に関する技術。情報技術。', 'Information Technology. Công nghệ thông tin.', '40000000-0000-0000-0000-000000000008'),
    ('50000000-0000-0000-0000-000000000010', 1, 'Hệ điều hành', 'コンピュータのハードウェアやソフトウェアを管理する基本ソフト。', 'Operating System. Phần mềm quản lý tài nguyên máy tính.', '40000000-0000-0000-0000-000000000010'),
    ('50000000-0000-0000-0000-000000000013', 1, 'Quản lý dự án', 'プロジェクト全体の進行・品質・予算などを管理する責任者。', 'Project Manager. Quản lý dự án.', '40000000-0000-0000-0000-000000000013'),
    ('50000000-0000-0000-0000-000000000015', 1, 'Hỏi đáp', '質問とそれに対する回答。質疑応答。', 'Questions & Answers. Hoạt động trao đổi hỏi đáp.', '40000000-0000-0000-0000-000000000015'),
    ('50000000-0000-0000-0000-000000000017', 1, 'Kỹ sư hệ thống', 'システムの設計や開発を担当する技術者。システムエンジニア。', 'System Engineer. Kỹ sư hệ thống.', '40000000-0000-0000-0000-000000000017'),
    ('50000000-0000-0000-0000-000000000021', 1, 'Giao diện người dùng', '利用者がシステムを操作するときに目にする画面や操作部分。', 'User Interface. Giao diện tương tác người dùng.', '40000000-0000-0000-0000-000000000021'),
    ('50000000-0000-0000-0000-000000000022', 1, 'Kiểm thử đơn vị', 'プログラムの最小単位ごとに行うテスト。単体テスト。', 'Unit Testing. Kiểm thử đơn vị module.', '40000000-0000-0000-0000-000000000022'),
    ('50000000-0000-0000-0000-000000000024', 1, 'Kiểm thử tích hợp', 'モジュールを組み合わせ、連携が正しく動作するか確認するテスト。', 'Integration Testing. Kiểm thử liên kết các thành phần.', '40000000-0000-0000-0000-000000000024'),
    ('50000000-0000-0000-0000-000000000025', 1, 'Kiểm thử đơn vị', 'プログラムの最小単位が単独で正しく動作するか確認するテスト。', 'Unit Testing. Kiểm thử phần mềm độc lập.', '40000000-0000-0000-0000-000000000025'),
    ('50000000-0000-0000-0000-000000000026', 1, 'Định nghĩa yêu cầu', 'システムに必要な機能や条件を明確にして決める工程。', 'Công đoạn làm rõ và xác định yêu cầu hệ thống.', '40000000-0000-0000-0000-000000000026'),
    ('50000000-0000-0000-0000-000000000027', 1, 'Phát triển hệ thống', 'システムを企画・設計・開発・導入する一連の作業。', 'Hoạt động thiết kế và phát triển hệ thống phần mềm.', '40000000-0000-0000-0000-000000000027'),
    ('50000000-0000-0000-0000-000000000028', 1, 'Phân tích dữ liệu', 'データを調べて、役に立つ情報や傾向を見つけ出すこと。', 'Phân tích dữ liệu tìm xu hướng hữu ích.', '40000000-0000-0000-0000-000000000028'),
    ('50000000-0000-0000-0000-000000000038', 1, 'Công tác', '仕事のために、ふだんの勤務地とは別の場所へ行くこと。', 'Đi đến địa phương hoặc quốc gia khác để thực hiện công việc.', '40000000-0000-0000-0000-000000000038'),
    ('50000000-0000-0000-0000-000000000041', 1, 'Trao đổi danh thiếp', '初対面のあいさつとして、お互いに名刺を渡し合うこと。', 'Trao đổi danh thiếp khi lần đầu gặp đối tác.', '40000000-0000-0000-0000-000000000041'),
    ('50000000-0000-0000-0000-000000000047', 1, 'Công ty chúng tôi', '自分の会社をへりくだって言う言葉。弊社。', 'Khiêm nhường ngữ dùng để gọi công ty mình trước khách hàng.', '40000000-0000-0000-0000-000000000047')
ON CONFLICT (id) DO NOTHING;


-- ====================================================================================================
-- 4. QUESTION TYPES
-- ====================================================================================================
INSERT INTO srs.question_types (id, code, name, description, is_active, created_at, updated_at)
VALUES
    (
        'e0000000-0000-0000-0000-000000000001',
        'MULTIPLE_CHOICE',
        'Trắc nghiệm ý nghĩa từ vựng',
        'Chọn ý nghĩa tiếng Việt chính xác nhất cho từ vựng hoặc thuật ngữ tiếng Nhật.',
        TRUE,
        NOW(),
        NOW()
    ),
    (
        'e0000000-0000-0000-0000-000000000002',
        'READING_CHOICE',
        'Trắc nghiệm cách đọc (Furigana)',
        'Chọn cách phát âm / hiragana chính xác cho từ Hán tự (Kanji).',
        TRUE,
        NOW(),
        NOW()
    ),
    (
        'e0000000-0000-0000-0000-000000000003',
        'LISTENING_CHOICE',
        'Nghe và chọn từ vựng',
        'Nghe file phát âm tiếng Nhật và chọn từ vựng tương ứng.',
        TRUE,
        NOW(),
        NOW()
    )
ON CONFLICT (id) DO NOTHING;


-- ====================================================================================================
-- 5. BANK QUESTIONS (20 câu hỏi tương ứng 20 Knowledge Meanings)
-- ====================================================================================================
INSERT INTO srs.bank_questions (
    id,
    knowledge_meaning_id,
    question_type_id,
    prompt,
    audio_url,
    choices_json,
    correct_choice_id,
    explanation,
    difficulty,
    is_active,
    created_at,
    updated_at
)
VALUES
    -- 01. AI
    ('f0000000-0000-0000-0000-000000000001', '50000000-0000-0000-0000-000000000001', 'e0000000-0000-0000-0000-000000000001', '「AI」の意味として最も適切なものを選択してください。', NULL, '[{"id":"A","text":"Trí tuệ nhân tạo"},{"id":"B","text":"Cơ sở dữ liệu"},{"id":"C","text":"Giao diện lập trình"},{"id":"D","text":"Hệ điều hành"}]'::jsonb, 'A', 'AI (Artificial Intelligence) nghĩa là Trí tuệ nhân tạo (人工知能).', 'Medium', TRUE, NOW(), NOW()),
    -- 02. API
    ('f0000000-0000-0000-0000-000000000002', '50000000-0000-0000-0000-000000000002', 'e0000000-0000-0000-0000-000000000001', '「API」の役割として正しい説明を選んでください。', NULL, '[{"id":"A","text":"Giao diện kết nối và trao đổi dữ liệu giữa các phần mềm"},{"id":"B","text":"Thiết bị lưu trữ dữ liệu tạm thời"},{"id":"C","text":"Bộ vi xử lý trung tâm của máy chủ"},{"id":"D","text":"Quy trình kiểm thử chấp nhận người dùng"}]'::jsonb, 'A', 'API (Application Programming Interface) là giao diện cho phép các phần mềm giao tiếp với nhau.', 'Medium', TRUE, NOW(), NOW()),
    -- 03. AWS
    ('f0000000-0000-0000-0000-000000000003', '50000000-0000-0000-0000-000000000003', 'e0000000-0000-0000-0000-000000000001', '「AWS」は何の略称ですか？', NULL, '[{"id":"A","text":"Amazon Web Services - Dịch vụ điện toán đám mây của Amazon"},{"id":"B","text":"Advanced Web Security - Hệ thống bảo mật nâng cao"},{"id":"C","text":"Application Workflow Standard - Chuẩn quy trình ứng dụng"},{"id":"D","text":"Automated Web Server - Máy chủ web tự động"}]'::jsonb, 'A', 'AWS là viết tắt của Amazon Web Services.', 'Medium', TRUE, NOW(), NOW()),
    -- 04. BA
    ('f0000000-0000-0000-0000-000000000004', '50000000-0000-0000-0000-000000000004', 'e0000000-0000-0000-0000-000000000001', '「BA」が担当する主な業務は何ですか？', NULL, '[{"id":"A","text":"Phân tích nghiệp vụ và làm rõ yêu cầu khách hàng"},{"id":"B","text":"Viết mã nguồn backend"},{"id":"C","text":"Quản trị hệ thống máy chủ mạng"},{"id":"D","text":"Kiểm thử tự động hiệu năng hệ thống"}]'::jsonb, 'A', 'BA (Business Analyst) phụ trách phân tích nghiệp vụ và yêu cầu hệ thống.', 'Medium', TRUE, NOW(), NOW()),
    -- 05. DB
    ('f0000000-0000-0000-0000-000000000005', '50000000-0000-0000-0000-000000000005', 'e0000000-0000-0000-0000-000000000001', '「DB」の日本語の意味として正しいものはどれですか？', NULL, '[{"id":"A","text":"データベース (Cơ sở dữ liệu)"},{"id":"B","text":"データブロック (Khối dữ liệu)"},{"id":"C","text":"デジタルビジネス (Kinh doanh số)"},{"id":"D","text":"ドメインブランチ (Nhánh tên miền)"}]'::jsonb, 'A', 'DB là Database (データベース) - Hệ thống lưu trữ và quản lý dữ liệu.', 'Easy', TRUE, NOW(), NOW()),
    -- 06. IT
    ('f0000000-0000-0000-0000-000000000006', '50000000-0000-0000-0000-000000000008', 'e0000000-0000-0000-0000-000000000001', '「IT」は何の略称ですか？', NULL, '[{"id":"A","text":"Information Technology (Công nghệ thông tin)"},{"id":"B","text":"Internet Terminal (Thiết bị đầu cuối internet)"},{"id":"C","text":"Integrated Testing (Kiểm thử tích hợp)"},{"id":"D","text":"Internal Tool (Công cụ nội bộ)"}]'::jsonb, 'A', 'IT là viết tắt của Information Technology (情報技術).', 'Easy', TRUE, NOW(), NOW()),
    -- 07. OS
    ('f0000000-0000-0000-0000-000000000007', '50000000-0000-0000-0000-000000000010', 'e0000000-0000-0000-0000-000000000001', '「OS」に該当するものとして正しい組み合わせを選んでください。', NULL, '[{"id":"A","text":"Windows, macOS, Linux"},{"id":"B","text":"Word, Excel, PowerPoint"},{"id":"C","text":"PostgreSQL, MySQL, Oracle"},{"id":"D","text":"Chrome, Firefox, Safari"}]'::jsonb, 'A', 'OS (Operating System) là hệ điều hành quản lý phần cứng và ứng dụng như Windows, macOS, Linux.', 'Medium', TRUE, NOW(), NOW()),
    -- 08. PM
    ('f0000000-0000-0000-0000-000000000008', '50000000-0000-0000-0000-000000000013', 'e0000000-0000-0000-0000-000000000001', '「PM」の主な責任は何ですか？', NULL, '[{"id":"A","text":"Quản lý tiến độ, chất lượng và nguồn lực của dự án"},{"id":"B","text":"Trực tiếp sửa lỗi mã nguồn của lập trình viên"},{"id":"C","text":"Thiết kế giao diện mỹ thuật cho trang web"},{"id":"D","text":"Quản lý hợp đồng bảo hiểm nhân sự"}]'::jsonb, 'A', 'PM (Project Manager) là người quản lý dự án (プロジェクトマネージャー).', 'Medium', TRUE, NOW(), NOW()),
    -- 09. Q&A
    ('f0000000-0000-0000-0000-000000000009', '50000000-0000-0000-0000-000000000015', 'e0000000-0000-0000-0000-000000000001', '「Q&A」の一般的な日本語表現を選んでください。', NULL, '[{"id":"A","text":"質疑応答 (Hỏi đáp)"},{"id":"B","text":"要求仕様 (Yêu cầu đặc tả)"},{"id":"C","text":"品質保証 (Bảo đảm chất lượng)"},{"id":"D","text":"進捗報告 (Báo cáo tiến độ)"}]'::jsonb, 'A', 'Q&A (Questions and Answers) nghĩa là hỏi đáp hoặc chất vấn phản hồi.', 'Easy', TRUE, NOW(), NOW()),
    -- 10. SE
    ('f0000000-0000-0000-0000-000000000010', '50000000-0000-0000-0000-000000000017', 'e0000000-0000-0000-0000-000000000001', '「SE」の職種名として正しいものを選んでください。', NULL, '[{"id":"A","text":"Kỹ sư hệ thống (System Engineer)"},{"id":"B","text":"Kỹ sư bảo mật (Security Engineer)"},{"id":"C","text":"Kỹ sư phần mềm (Software Engineer)"},{"id":"D","text":"Kỹ sư hỗ trợ (Support Engineer)"}]'::jsonb, 'A', 'Tại Nhật, SE thường chỉ System Engineer (システムエンジニア) - người phụ trách thiết kế và phát triển hệ thống.', 'Medium', TRUE, NOW(), NOW()),
    -- 11. UI
    ('f0000000-0000-0000-0000-000000000011', '50000000-0000-0000-0000-000000000021', 'e0000000-0000-0000-0000-000000000001', '「UI」の意味として最も適切なものはどれですか？', NULL, '[{"id":"A","text":"Giao diện người dùng (User Interface)"},{"id":"B","text":"Trải nghiệm người dùng (User Experience)"},{"id":"C","text":"Mã nhận diện tài khoản (User Identifier)"},{"id":"D","text":"Quyền truy cập cá nhân (User Isolation)"}]'::jsonb, 'A', 'UI là User Interface (ユーザーインターフェース) - giao diện người dùng nhìn và tương tác.', 'Easy', TRUE, NOW(), NOW()),
    -- 12. UT
    ('f0000000-0000-0000-0000-000000000012', '50000000-0000-0000-0000-000000000022', 'e0000000-0000-0000-0000-000000000001', '「UT」の日本語のテスト工程名を選んでください。', NULL, '[{"id":"A","text":"単体テスト (Unit Testing)"},{"id":"B","text":"結合テスト (Integration Testing)"},{"id":"C","text":"総合テスト (System Testing)"},{"id":"D","text":"受入テスト (Acceptance Testing)"}]'::jsonb, 'A', 'UT là Unit Testing tương đương với 単体テスト trong quy trình phát triển phần mềm.', 'Medium', TRUE, NOW(), NOW()),
    -- 13. 結合テスト
    ('f0000000-0000-0000-0000-000000000013', '50000000-0000-0000-0000-000000000024', 'e0000000-0000-0000-0000-000000000002', '「結合テスト」の正しい読み方を選択してください。', NULL, '[{"id":"A","text":"けつごうテスト"},{"id":"B","text":"ゆいごうテスト"},{"id":"C","text":"けつあいテスト"},{"id":"D","text":"ゆうごうテスト"}]'::jsonb, 'A', '結合テスト được đọc là けつごうテスト, nghĩa là Kiểm thử tích hợp (Integration Test).', 'Medium', TRUE, NOW(), NOW()),
    -- 14. 単体テスト
    ('f0000000-0000-0000-0000-000000000014', '50000000-0000-0000-0000-000000000025', 'e0000000-0000-0000-0000-000000000002', '「単体テスト」の読み方を選んでください。', NULL, '[{"id":"A","text":"たんたいテスト"},{"id":"B","text":"たんたい試験"},{"id":"C","text":"ひとりたいテスト"},{"id":"D","text":"たんていテスト"}]'::jsonb, 'A', '単体テスト được đọc là たんたいテスト (Kiểm thử đơn vị).', 'Easy', TRUE, NOW(), NOW()),
    -- 15. 要件定義
    ('f0000000-0000-0000-0000-000000000015', '50000000-0000-0000-0000-000000000026', 'e0000000-0000-0000-0000-000000000001', '「要件定義」の説明として正しいものはどれですか？', NULL, '[{"id":"A","text":"Công đoạn làm rõ và xác định các yêu cầu của hệ thống"},{"id":"B","text":"Công đoạn cài đặt máy chủ và cấu hình tường lửa"},{"id":"C","text":"Công đoạn viết tài liệu hướng dẫn người dùng cuối"},{"id":"D","text":"Công đoạn thanh lý hợp đồng và bàn giao mã nguồn"}]'::jsonb, 'A', '要件定義 (ようけんていぎ) là công đoạn định nghĩa và làm rõ yêu cầu.', 'Medium', TRUE, NOW(), NOW()),
    -- 16. システム開発
    ('f0000000-0000-0000-0000-000000000016', '50000000-0000-0000-0000-000000000027', 'e0000000-0000-0000-0000-000000000001', '「システム開発」の意味を選んでください。', NULL, '[{"id":"A","text":"Phát triển hệ thống"},{"id":"B","text":"Bảo trì máy in"},{"id":"C","text":"Đào tạo học viên"},{"id":"D","text":"Khảo sát thị trường"}]'::jsonb, 'A', 'システム開発 (システムかいはつ) nghĩa là phát triển hệ thống phần mềm.', 'Easy', TRUE, NOW(), NOW()),
    -- 17. データ分析
    ('f0000000-0000-0000-0000-000000000017', '50000000-0000-0000-0000-000000000028', 'e0000000-0000-0000-0000-000000000001', '「データ分析」の正しい意味はどれですか？', NULL, '[{"id":"A","text":"Phân tích dữ liệu"},{"id":"B","text":"Sao lưu dữ liệu"},{"id":"C","text":"Nhập liệu bảng tính"},{"id":"D","text":"Xóa dữ liệu trùng lặp"}]'::jsonb, 'A', 'データ分析 (データぶんせき) nghĩa là phân tích dữ liệu nhằm tìm ra thông tin hữu ích.', 'Easy', TRUE, NOW(), NOW()),
    -- 18. 出張
    ('f0000000-0000-0000-0000-000000000018', '50000000-0000-0000-0000-000000000038', 'e0000000-0000-0000-0000-000000000002', '「出張」の読み方と意味として正しいものを選んでください。', NULL, '[{"id":"A","text":"しゅっちょう (Đi công tác)"},{"id":"B","text":"でばり (Mở rộng phạm vi)"},{"id":"C","text":"しゅつば (Ra ứng cử)"},{"id":"D","text":"でちょう (Sổ theo dõi)"}]'::jsonb, 'A', '出張 (しゅっちょう) nghĩa là đi công tác vì công việc.', 'Easy', TRUE, NOW(), NOW()),
    -- 19. 名刺交換
    ('f0000000-0000-0000-0000-000000000019', '50000000-0000-0000-0000-000000000041', 'e0000000-0000-0000-0000-000000000001', 'ビジネスシーンにおける「名刺交換」の意味を選んでください。', NULL, '[{"id":"A","text":"Trao đổi danh thiếp khi lần đầu gặp gỡ"},{"id":"B","text":"Ký hợp đồng dịch vụ"},{"id":"C","text":"Bàn giao con dấu công ty"},{"id":"D","text":"Thanh toán tiền công tác phí"}]'::jsonb, 'A', '名刺交換 (めいしこうかん) là nét văn hóa trao đổi danh thiếp trong kinh doanh Nhật Bản.', 'Medium', TRUE, NOW(), NOW()),
    -- 20. 弊社
    ('f0000000-0000-0000-0000-000000000020', '50000000-0000-0000-0000-000000000047', 'e0000000-0000-0000-0000-000000000001', '「弊社」の読み方とビジネスでの使い方の説明を選んでください。', NULL, '[{"id":"A","text":"へいしゃ - Từ khiêm nhường để chỉ công ty của chính mình"},{"id":"B","text":"ごしゃ - Từ tôn kính để chỉ công ty đối tác"},{"id":"C","text":"へいしゃ - Từ chỉ công ty đã phá sản"},{"id":"D","text":"きしゃ - Từ dùng trong văn viết gửi thư cho đối tác"}]'::jsonb, 'A', '弊社 (へいしゃ) là từ khiêm nhường (謙譲語) người nói dùng để gọi công ty mình trước đối tác.', 'Medium', TRUE, NOW(), NOW())
ON CONFLICT (id) DO NOTHING;


-- ====================================================================================================
-- 6. SAVED ITEMS (20 TỪ VỰNG DÀNH RIÊNG CHO TÀI KHOẢN SRS LEARNER)
-- User: 'c1111111-1111-1111-1111-111111111111'
-- ====================================================================================================
INSERT INTO srs.saved_items (
    id,
    user_id,
    knowledge_meaning_id,
    expression,
    reading,
    meaning,
    saved_at,
    created_at
)
VALUES
    -- Group 1: Due_today (4 items)
    ('70000000-0000-0000-0000-000000000001', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000001', 'AI', 'エーアイ', 'Trí tuệ nhân tạo', NOW() - INTERVAL '7 days', NOW() - INTERVAL '7 days'),
    ('70000000-0000-0000-0000-000000000002', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000002', 'API', 'エーピーアイ', 'Giao diện lập trình ứng dụng', NOW() - INTERVAL '14 days', NOW() - INTERVAL '14 days'),
    ('70000000-0000-0000-0000-000000000003', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000003', 'AWS', 'アマゾンウェブサービス', 'Dịch vụ web Amazon', NOW() - INTERVAL '5 days', NOW() - INTERVAL '5 days'),
    ('70000000-0000-0000-0000-000000000004', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000004', 'BA', 'ビーエー', 'Người phân tích nghiệp vụ', NOW() - INTERVAL '3 days', NOW() - INTERVAL '3 days'),

    -- Group 2: Overdue (4 items)
    ('70000000-0000-0000-0000-000000000005', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000005', 'DB', 'データベース', 'Cơ sở dữ liệu', NOW() - INTERVAL '10 days', NOW() - INTERVAL '10 days'),
    ('70000000-0000-0000-0000-000000000006', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000008', 'IT', 'アイティー', 'Công nghệ thông tin', NOW() - INTERVAL '12 days', NOW() - INTERVAL '12 days'),
    ('70000000-0000-0000-0000-000000000007', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000010', 'OS', 'オーエス', 'Hệ điều hành', NOW() - INTERVAL '15 days', NOW() - INTERVAL '15 days'),
    ('70000000-0000-0000-0000-000000000008', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000013', 'PM', 'ピーエム', 'Quản lý dự án', NOW() - INTERVAL '16 days', NOW() - INTERVAL '16 days'),

    -- Group 3: Learning (4 items)
    ('70000000-0000-0000-0000-000000000009', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000015', 'Q&A', 'キューアンドエー', 'Hỏi đáp', NOW() - INTERVAL '1 hour', NOW() - INTERVAL '1 hour'),
    ('70000000-0000-0000-0000-000000000010', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000017', 'SE', 'エスイー', 'Kỹ sư hệ thống', NOW() - INTERVAL '2 days', NOW() - INTERVAL '2 days'),
    ('70000000-0000-0000-0000-000000000011', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000021', 'UI', 'ユーアイ', 'Giao diện người dùng', NOW() - INTERVAL '3 days', NOW() - INTERVAL '3 days'),
    ('70000000-0000-0000-0000-000000000012', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000022', 'UT', 'ユーティー', 'Kiểm thử đơn vị', NOW() - INTERVAL '4 days', NOW() - INTERVAL '4 days'),

    -- Group 4: Review / Mastered (4 items)
    ('70000000-0000-0000-0000-000000000013', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000024', '結合テスト', 'けつごうテスト', 'Kiểm thử tích hợp', NOW() - INTERVAL '20 days', NOW() - INTERVAL '20 days'),
    ('70000000-0000-0000-0000-000000000014', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000025', '単体テスト', 'たんたいテスト', 'Kiểm thử đơn vị', NOW() - INTERVAL '30 days', NOW() - INTERVAL '30 days'),
    ('70000000-0000-0000-0000-000000000026', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000026', '要件定義', 'ようけんていぎ', 'Định nghĩa yêu cầu', NOW() - INTERVAL '40 days', NOW() - INTERVAL '40 days'),
    ('70000000-0000-0000-0000-000000000016', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000027', 'システム開発', 'システムかいはつ', 'Phát triển hệ thống', NOW() - INTERVAL '25 days', NOW() - INTERVAL '25 days'),

    -- Group 5: Suspended (2 items)
    ('70000000-0000-0000-0000-000000000017', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000028', 'データ分析', 'データぶんせき', 'Phân tích dữ liệu', NOW() - INTERVAL '15 days', NOW() - INTERVAL '15 days'),
    ('70000000-0000-0000-0000-000000000018', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000038', '出張', 'しゅっちょう', 'Công tác', NOW() - INTERVAL '8 days', NOW() - INTERVAL '8 days'),

    -- Group 6: Ignored / Soft-deleted (2 items)
    ('70000000-0000-0000-0000-000000000019', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000041', '名刺交換', 'めいしこうかん', 'Trao đổi danh thiếp', NOW() - INTERVAL '18 days', NOW() - INTERVAL '18 days'),
    ('70000000-0000-0000-0000-000000000020', 'c1111111-1111-1111-1111-111111111111', '50000000-0000-0000-0000-000000000047', '弊社', 'へいしゃ', 'Công ty chúng tôi', NOW() - INTERVAL '22 days', NOW() - INTERVAL '22 days')
ON CONFLICT (id) DO NOTHING;


-- ====================================================================================================
-- 7. SRS SCHEDULES (ĐA DẠNG 6 TRẠNG THÁI THEO SM-2 STATE DIAGRAM)
-- ====================================================================================================
INSERT INTO srs.srs_schedules (
    id,
    saved_item_id,
    status,
    due_at,
    last_reviewed_at,
    created_at,
    updated_at
)
VALUES
    -- Group 1: Due_today (Đến hạn ôn tập hôm nay)
    (
        '80000000-0000-0000-0000-000000000001',
        '70000000-0000-0000-0000-000000000001',
        'Due_today',
        NOW() - INTERVAL '2 hours',
        NOW() - INTERVAL '1 day',
        NOW() - INTERVAL '7 days',
        NOW() - INTERVAL '1 day'
    ),
    (
        '80000000-0000-0000-0000-000000000002',
        '70000000-0000-0000-0000-000000000002',
        'Due_today',
        NOW() - INTERVAL '4 hours',
        NOW() - INTERVAL '6 days',
        NOW() - INTERVAL '14 days',
        NOW() - INTERVAL '6 days'
    ),
    (
        '80000000-0000-0000-0000-000000000003',
        '70000000-0000-0000-0000-000000000003',
        'Due_today',
        NOW() - INTERVAL '1 hour',
        NOW() - INTERVAL '1 day',
        NOW() - INTERVAL '5 days',
        NOW() - INTERVAL '1 day'
    ),
    (
        '80000000-0000-0000-0000-000000000004',
        '70000000-0000-0000-0000-000000000004',
        'Due_today',
        NOW() - INTERVAL '30 minutes',
        NOW() - INTERVAL '1 day',
        NOW() - INTERVAL '3 days',
        NOW() - INTERVAL '1 day'
    ),

    -- Group 2: Overdue (Quá hạn ôn tập từ 2-7 ngày trước)
    (
        '80000000-0000-0000-0000-000000000005',
        '70000000-0000-0000-0000-000000000005',
        'Overdue',
        NOW() - INTERVAL '2 days',
        NOW() - INTERVAL '3 days',
        NOW() - INTERVAL '10 days',
        NOW() - INTERVAL '3 days'
    ),
    (
        '80000000-0000-0000-0000-000000000006',
        '70000000-0000-0000-0000-000000000006',
        'Overdue',
        NOW() - INTERVAL '3 days',
        NOW() - INTERVAL '4 days',
        NOW() - INTERVAL '12 days',
        NOW() - INTERVAL '4 days'
    ),
    (
        '80000000-0000-0000-0000-000000000007',
        '70000000-0000-0000-0000-000000000007',
        'Overdue',
        NOW() - INTERVAL '5 days',
        NOW() - INTERVAL '11 days',
        NOW() - INTERVAL '15 days',
        NOW() - INTERVAL '11 days'
    ),
    (
        '80000000-0000-0000-0000-000000000008',
        '70000000-0000-0000-0000-000000000008',
        'Overdue',
        NOW() - INTERVAL '7 days',
        NOW() - INTERVAL '8 days',
        NOW() - INTERVAL '16 days',
        NOW() - INTERVAL '8 days'
    ),

    -- Group 3: Learning (Đang trong giai đoạn học, due_at rơi vào ngày mai hoặc tương lai gần)
    (
        '80000000-0000-0000-0000-000000000009',
        '70000000-0000-0000-0000-000000000009',
        'Learning',
        NOW() + INTERVAL '1 day',
        NULL,
        NOW() - INTERVAL '1 hour',
        NOW() - INTERVAL '1 hour'
    ),
    (
        '80000000-0000-0000-0000-000000000010',
        '70000000-0000-0000-0000-000000000010',
        'Learning',
        NOW() + INTERVAL '2 days',
        NOW() - INTERVAL '4 hours',
        NOW() - INTERVAL '2 days',
        NOW() - INTERVAL '4 hours'
    ),
    (
        '80000000-0000-0000-0000-000000000011',
        '70000000-0000-0000-0000-000000000011',
        'Learning',
        NOW() + INTERVAL '1 day',
        NOW() - INTERVAL '2 hours',
        NOW() - INTERVAL '3 days',
        NOW() - INTERVAL '2 hours'
    ),
    (
        '80000000-0000-0000-0000-000000000012',
        '70000000-0000-0000-0000-000000000012',
        'Learning',
        NOW() + INTERVAL '3 days',
        NOW() - INTERVAL '1 day',
        NOW() - INTERVAL '4 days',
        NOW() - INTERVAL '1 day'
    ),

    -- Group 4: Review (Đã thành thạo, Repetitions >= 3, chu kỳ ôn tập xa)
    (
        '80000000-0000-0000-0000-000000000013',
        '70000000-0000-0000-0000-000000000013',
        'Review',
        NOW() + INTERVAL '5 days',
        NOW() - INTERVAL '1 day',
        NOW() - INTERVAL '20 days',
        NOW() - INTERVAL '1 day'
    ),
    (
        '80000000-0000-0000-0000-000000000014',
        '70000000-0000-0000-0000-000000000014',
        'Review',
        NOW() + INTERVAL '10 days',
        NOW() - INTERVAL '2 days',
        NOW() - INTERVAL '30 days',
        NOW() - INTERVAL '2 days'
    ),
    (
        '80000000-0000-0000-0000-000000000015',
        '70000000-0000-0000-0000-000000000026',
        'Review',
        NOW() + INTERVAL '20 days',
        NOW() - INTERVAL '5 days',
        NOW() - INTERVAL '40 days',
        NOW() - INTERVAL '5 days'
    ),
    (
        '80000000-0000-0000-0000-000000000016',
        '70000000-0000-0000-0000-000000000016',
        'Review',
        NOW() + INTERVAL '12 days',
        NOW() - INTERVAL '3 days',
        NOW() - INTERVAL '25 days',
        NOW() - INTERVAL '3 days'
    ),

    -- Group 5: Suspended (Tạm dừng ôn tập)
    (
        '80000000-0000-0000-0000-000000000017',
        '70000000-0000-0000-0000-000000000017',
        'Suspended',
        NOW() + INTERVAL '2 days',
        NOW() - INTERVAL '2 days',
        NOW() - INTERVAL '15 days',
        NOW() - INTERVAL '1 day'
    ),
    (
        '80000000-0000-0000-0000-000000000018',
        '70000000-0000-0000-0000-000000000018',
        'Suspended',
        NOW() - INTERVAL '1 day',
        NULL,
        NOW() - INTERVAL '8 days',
        NOW() - INTERVAL '2 days'
    ),

    -- Group 6: Ignored (Đã xóa khỏi sổ tay - soft delete)
    (
        '80000000-0000-0000-0000-000000000019',
        '70000000-0000-0000-0000-000000000019',
        'Ignored',
        NOW() + INTERVAL '1 day',
        NULL,
        NOW() - INTERVAL '18 days',
        NOW() - INTERVAL '2 days'
    ),
    (
        '80000000-0000-0000-0000-000000000020',
        '70000000-0000-0000-0000-000000000020',
        'Ignored',
        NOW() - INTERVAL '3 days',
        NOW() - INTERVAL '4 days',
        NOW() - INTERVAL '22 days',
        NOW() - INTERVAL '1 day'
    )
ON CONFLICT (id) DO NOTHING;


-- ====================================================================================================
-- 8. SAMPLE QUIZ, QUESTIONS & COMPLETED ATTEMPT (SẴN SÀNG TEST GET RESULT & POST RETRY)
-- Quiz gồm 3 câu: Item 1 (AI), Item 2 (API), Item 3 (AWS)
-- ====================================================================================================
INSERT INTO srs.quizzes (id, user_id, total_questions, created_at)
VALUES
    ('90000000-0000-0000-0000-000000000001', 'c1111111-1111-1111-1111-111111111111', 3, NOW() - INTERVAL '1 day')
ON CONFLICT (id) DO NOTHING;

INSERT INTO srs.questions (
    id,
    quiz_id,
    saved_item_id,
    bank_question_id,
    question_type_id,
    position,
    prompt,
    reading,
    choices_json,
    correct_option_id,
    created_at
)
VALUES
    (
        'a0000000-0000-0000-0000-000000000001',
        '90000000-0000-0000-0000-000000000001',
        '70000000-0000-0000-0000-000000000001',
        'f0000000-0000-0000-0000-000000000001',
        'e0000000-0000-0000-0000-000000000001',
        1,
        '「AI」の意味として最も適切なものを選択してください。',
        'エーアイ',
        '[{"id":"A","text":"Trí tuệ nhân tạo"},{"id":"B","text":"Cơ sở dữ liệu"},{"id":"C","text":"Giao diện lập trình"},{"id":"D","text":"Hệ điều hành"}]'::jsonb,
        'A',
        NOW() - INTERVAL '1 day'
    ),
    (
        'a0000000-0000-0000-0000-000000000002',
        '90000000-0000-0000-0000-000000000001',
        '70000000-0000-0000-0000-000000000002',
        'f0000000-0000-0000-0000-000000000002',
        'e0000000-0000-0000-0000-000000000001',
        2,
        '「API」の役割として正しい説明を選んでください。',
        'エーピーアイ',
        '[{"id":"A","text":"Giao diện kết nối và trao đổi dữ liệu giữa các phần mềm"},{"id":"B","text":"Thiết bị lưu trữ dữ liệu tạm thời"},{"id":"C","text":"Bộ vi xử lý trung tâm của máy chủ"},{"id":"D","text":"Quy trình kiểm thử chấp nhận người dùng"}]'::jsonb,
        'A',
        NOW() - INTERVAL '1 day'
    ),
    (
        'a0000000-0000-0000-0000-000000000003',
        '90000000-0000-0000-0000-000000000001',
        '70000000-0000-0000-0000-000000000003',
        'f0000000-0000-0000-0000-000000000003',
        'e0000000-0000-0000-0000-000000000001',
        3,
        '「AWS」は何の略称ですか？',
        'アマゾンウェブサービス',
        '[{"id":"A","text":"Amazon Web Services - Dịch vụ điện toán đám mây của Amazon"},{"id":"B","text":"Advanced Web Security - Hệ thống bảo mật nâng cao"},{"id":"C","text":"Application Workflow Standard - Chuẩn quy trình ứng dụng"},{"id":"D","text":"Automated Web Server - Máy chủ web tự động"}]'::jsonb,
        'A',
        NOW() - INTERVAL '1 day'
    )
ON CONFLICT (id) DO NOTHING;

INSERT INTO srs.quiz_attempts (
    id,
    user_id,
    quiz_id,
    status,
    mode,
    total_items,
    answered_items,
    correct_items,
    score,
    started_at,
    completed_at,
    created_at,
    updated_at
)
VALUES
    (
        'b0000000-0000-0000-0000-000000000001',
        'c1111111-1111-1111-1111-111111111111',
        '90000000-0000-0000-0000-000000000001',
        'Completed',
        'SrsReview',
        3,
        3,
        2,
        66.67,
        NOW() - INTERVAL '1 day',
        NOW() - INTERVAL '1 day' + INTERVAL '35 seconds',
        NOW() - INTERVAL '1 day',
        NOW() - INTERVAL '1 day' + INTERVAL '35 seconds'
    )
ON CONFLICT (id) DO NOTHING;

INSERT INTO srs.quiz_attempt_items (
    id,
    quiz_attempt_id,
    question_id,
    position,
    selected_option_id,
    is_correct,
    elapsed_ms,
    feedback,
    answered_at,
    created_at
)
VALUES
    (
        'c0000000-0000-0000-0000-000000000001',
        'b0000000-0000-0000-0000-000000000001',
        'a0000000-0000-0000-0000-000000000001',
        1,
        'A',
        TRUE,
        3800,
        NULL,
        NOW() - INTERVAL '1 day' + INTERVAL '4 seconds',
        NOW() - INTERVAL '1 day'
    ),
    (
        'c0000000-0000-0000-0000-000000000002',
        'b0000000-0000-0000-0000-000000000001',
        'a0000000-0000-0000-0000-000000000002',
        2,
        'A',
        TRUE,
        8500,
        NULL,
        NOW() - INTERVAL '1 day' + INTERVAL '15 seconds',
        NOW() - INTERVAL '1 day'
    ),
    (
        'c0000000-0000-0000-0000-000000000003',
        'b0000000-0000-0000-0000-000000000001',
        'a0000000-0000-0000-0000-000000000003',
        3,
        'B',
        FALSE,
        6200,
        'Đáp án đúng là A: Amazon Web Services',
        NOW() - INTERVAL '1 day' + INTERVAL '30 seconds',
        NOW() - INTERVAL '1 day'
    )
ON CONFLICT (id) DO NOTHING;


-- ====================================================================================================
-- 9. SRS ITEM DETAILS (LỊCH SỬ ÔN TẬP CHI TIẾT THEO SM-2)
-- ====================================================================================================
INSERT INTO srs.srs_item_details (
    id,
    srs_schedule_id,
    quiz_attempt_item_id,
    review_number,
    quality,
    interval_days,
    easiness_factor,
    repetitions,
    lapses,
    reviewed_at
)
VALUES
    -- Schedule 01: Item 1 (AI) - Review 1
    (
        'd0000000-0000-0000-0000-000000000001',
        '80000000-0000-0000-0000-000000000001',
        'c0000000-0000-0000-0000-000000000001',
        1,
        5,
        1,
        2.6,
        1,
        0,
        NOW() - INTERVAL '1 day'
    ),

    -- Schedule 02: Item 2 (API) - 2 reviews
    (
        'd0000000-0000-0000-0000-000000000002',
        '80000000-0000-0000-0000-000000000002',
        NULL,
        1,
        4,
        1,
        2.5,
        1,
        0,
        NOW() - INTERVAL '12 days'
    ),
    (
        'd0000000-0000-0000-0000-000000000003',
        '80000000-0000-0000-0000-000000000002',
        'c0000000-0000-0000-0000-000000000002',
        2,
        4,
        6,
        2.5,
        2,
        0,
        NOW() - INTERVAL '6 days'
    ),

    -- Schedule 03: Item 3 (AWS) - Trả lời sai (Quality 2)
    (
        'd0000000-0000-0000-0000-000000000004',
        '80000000-0000-0000-0000-000000000003',
        'c0000000-0000-0000-0000-000000000003',
        1,
        2,
        1,
        2.5,
        0,
        1,
        NOW() - INTERVAL '1 day'
    ),

    -- Schedule 04: Item 4 (BA) - 1 review
    (
        'd0000000-0000-0000-0000-000000000005',
        '80000000-0000-0000-0000-000000000004',
        NULL,
        1,
        4,
        1,
        2.5,
        1,
        0,
        NOW() - INTERVAL '1 day'
    ),

    -- Schedule 05: Item 5 (DB) - Overdue, 1 review
    (
        'd0000000-0000-0000-0000-000000000006',
        '80000000-0000-0000-0000-000000000005',
        NULL,
        1,
        4,
        1,
        2.5,
        1,
        0,
        NOW() - INTERVAL '3 days'
    ),

    -- Schedule 06: Item 6 (IT) - Overdue, 1 review
    (
        'd0000000-0000-0000-0000-000000000007',
        '80000000-0000-0000-0000-000000000006',
        NULL,
        1,
        5,
        1,
        2.6,
        1,
        0,
        NOW() - INTERVAL '4 days'
    ),

    -- Schedule 07: Item 7 (OS) - Overdue, 2 reviews
    (
        'd0000000-0000-0000-0000-000000000008',
        '80000000-0000-0000-0000-000000000007',
        NULL,
        1,
        4,
        1,
        2.5,
        1,
        0,
        NOW() - INTERVAL '17 days'
    ),
    (
        'd0000000-0000-0000-0000-000000000009',
        '80000000-0000-0000-0000-000000000007',
        NULL,
        2,
        4,
        6,
        2.5,
        2,
        0,
        NOW() - INTERVAL '11 days'
    ),

    -- Schedule 08: Item 8 (PM) - Overdue, 1 review
    (
        'd0000000-0000-0000-0000-000000000010',
        '80000000-0000-0000-0000-000000000008',
        NULL,
        1,
        3,
        1,
        2.36,
        1,
        0,
        NOW() - INTERVAL '8 days'
    ),

    -- Schedule 10: Item 10 (SE) - Learning, 1 review
    (
        'd0000000-0000-0000-0000-000000000011',
        '80000000-0000-0000-0000-000000000010',
        NULL,
        1,
        4,
        2,
        2.5,
        1,
        0,
        NOW() - INTERVAL '4 hours'
    ),

    -- Schedule 11: Item 11 (UI) - Learning, 1 review
    (
        'd0000000-0000-0000-0000-000000000012',
        '80000000-0000-0000-0000-000000000011',
        NULL,
        1,
        5,
        1,
        2.6,
        1,
        0,
        NOW() - INTERVAL '2 hours'
    ),

    -- Schedule 12: Item 12 (UT) - Learning, 2 reviews (từng sai rồi làm lại đúng)
    (
        'd0000000-0000-0000-0000-000000000013',
        '80000000-0000-0000-0000-000000000012',
        NULL,
        1,
        2,
        1,
        2.5,
        0,
        1,
        NOW() - INTERVAL '3 days'
    ),
    (
        'd0000000-0000-0000-0000-000000000014',
        '80000000-0000-0000-0000-000000000012',
        NULL,
        2,
        4,
        1,
        2.5,
        1,
        1,
        NOW() - INTERVAL '1 day'
    ),

    -- Schedule 13: Item 13 (結合テスト) - Review status (3 reviews, Repetitions = 3)
    (
        'd0000000-0000-0000-0000-000000000015',
        '80000000-0000-0000-0000-000000000013',
        NULL,
        1,
        4,
        1,
        2.5,
        1,
        0,
        NOW() - INTERVAL '15 days'
    ),
    (
        'd0000000-0000-0000-0000-000000000016',
        '80000000-0000-0000-0000-000000000013',
        NULL,
        2,
        4,
        6,
        2.5,
        2,
        0,
        NOW() - INTERVAL '8 days'
    ),
    (
        'd0000000-0000-0000-0000-000000000017',
        '80000000-0000-0000-0000-000000000013',
        NULL,
        3,
        5,
        15,
        2.6,
        3,
        0,
        NOW() - INTERVAL '1 day'
    ),

    -- Schedule 14: Item 14 (単体テスト) - Review status (4 reviews, Repetitions = 4)
    (
        'd0000000-0000-0000-0000-000000000018',
        '80000000-0000-0000-0000-000000000014',
        NULL,
        1,
        4,
        1,
        2.5,
        1,
        0,
        NOW() - INTERVAL '25 days'
    ),
    (
        'd0000000-0000-0000-0000-000000000019',
        '80000000-0000-0000-0000-000000000014',
        NULL,
        2,
        5,
        6,
        2.6,
        2,
        0,
        NOW() - INTERVAL '18 days'
    ),
    (
        'd0000000-0000-0000-0000-000000000020',
        '80000000-0000-0000-0000-000000000014',
        NULL,
        3,
        5,
        16,
        2.7,
        3,
        0,
        NOW() - INTERVAL '9 days'
    ),
    (
        'd0000000-0000-0000-0000-000000000021',
        '80000000-0000-0000-0000-000000000014',
        NULL,
        4,
        4,
        43,
        2.7,
        4,
        0,
        NOW() - INTERVAL '2 days'
    ),

    -- Schedule 15: Item 15 (要件定義) - Review status (4 reviews, Repetitions = 4)
    (
        'd0000000-0000-0000-0000-000000000022',
        '80000000-0000-0000-0000-000000000015',
        NULL,
        1,
        4,
        1,
        2.5,
        1,
        0,
        NOW() - INTERVAL '35 days'
    ),
    (
        'd0000000-0000-0000-0000-000000000023',
        '80000000-0000-0000-0000-000000000015',
        NULL,
        2,
        4,
        6,
        2.5,
        2,
        0,
        NOW() - INTERVAL '28 days'
    ),
    (
        'd0000000-0000-0000-0000-000000000024',
        '80000000-0000-0000-0000-000000000015',
        NULL,
        3,
        5,
        15,
        2.6,
        3,
        0,
        NOW() - INTERVAL '16 days'
    ),
    (
        'd0000000-0000-0000-0000-000000000025',
        '80000000-0000-0000-0000-000000000015',
        NULL,
        4,
        5,
        39,
        2.7,
        4,
        0,
        NOW() - INTERVAL '5 days'
    ),

    -- Schedule 16: Item 16 (システム開発) - Review status (3 reviews, Repetitions = 3)
    (
        'd0000000-0000-0000-0000-000000000026',
        '80000000-0000-0000-0000-000000000016',
        NULL,
        1,
        4,
        1,
        2.5,
        1,
        0,
        NOW() - INTERVAL '20 days'
    ),
    (
        'd0000000-0000-0000-0000-000000000027',
        '80000000-0000-0000-0000-000000000016',
        NULL,
        2,
        4,
        6,
        2.5,
        2,
        0,
        NOW() - INTERVAL '13 days'
    ),
    (
        'd0000000-0000-0000-0000-000000000028',
        '80000000-0000-0000-0000-000000000016',
        NULL,
        3,
        4,
        15,
        2.5,
        3,
        0,
        NOW() - INTERVAL '3 days'
    ),

    -- Schedule 17: Item 17 (データ分析) - Suspended, 1 review
    (
        'd0000000-0000-0000-0000-000000000029',
        '80000000-0000-0000-0000-000000000017',
        NULL,
        1,
        4,
        1,
        2.5,
        1,
        0,
        NOW() - INTERVAL '2 days'
    ),

    -- Schedule 20: Item 20 (弊社) - Ignored, 1 review
    (
        'd0000000-0000-0000-0000-000000000030',
        '80000000-0000-0000-0000-000000000020',
        NULL,
        1,
        3,
        1,
        2.36,
        1,
        0,
        NOW() - INTERVAL '4 days'
    )
ON CONFLICT (id) DO NOTHING;

COMMIT;
