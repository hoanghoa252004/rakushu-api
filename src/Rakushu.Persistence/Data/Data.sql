
BEGIN;
-- ====================================================================================================
-- Rakushu Linguistic Metadata Seed Script (PostgreSQL)
-- Tables:
--   1. japanese_part_of_speeches  (UniDic/Sudachi XPOS tags)
--   2. dependency_relationships   (29 Universal Dependency tags)
-- Columns: id, code, name (tiếng Việt), japanese_name (tiếng Nhật), description
-- Script có thể chạy lại nhiều lần (idempotent) nhờ ON CONFLICT (code).
-- ====================================================================================================
-- ----------------------------------------------------------------------------------------------------

-- ----------------------------------------------------------------------------------------------------
-- 1. JAPANESE PART OF SPEECH (JPOS / UniDic XPOS)
-- ----------------------------------------------------------------------------------------------------
INSERT INTO knowledge.japanese_part_of_speeches (id, code, name, japanese_name, description)
VALUES
    (gen_random_uuid(), '代名詞', 'Đại từ', '代名詞', 'Đại từ chỉ người, vật, địa điểm (ví dụ: 私, 彼, ここ)'),
    (gen_random_uuid(), '副詞', 'Phó từ', '副詞', 'Phó từ bổ nghĩa hành động/trạng thái (ví dụ: ゆっくり, すぐ, もっと)'),
    (gen_random_uuid(), '助動詞', 'Trợ động từ', '助動詞', 'Trợ động từ biểu thị thể, thời, ý chí (ví dụ: だ, です, ます, ない, た, れる)'),
    (gen_random_uuid(), '助詞-係助詞', 'Trợ từ liên kết/nhấn mạnh', '係助詞', 'Trợ từ nhấn mạnh/nêu chủ đề (ví dụ: は, も, こそ, さえ)'),
    (gen_random_uuid(), '助詞-副助詞', 'Phó trợ từ', '副助詞', 'Trợ từ biểu thị mức độ, phạm vi (ví dụ: だけ, ばかり, ほど, くらい)'),
    (gen_random_uuid(), '助詞-接続助詞', 'Trợ từ nối tiếp', '接続助詞', 'Nối tiếp mệnh đề chỉ điều kiện, nguyên nhân, chuyển tiếp (ví dụ: て, たら, ので, から)'),
    (gen_random_uuid(), '助詞-格助詞', 'Trợ từ cách (Cách trợ từ)', '格助詞', 'Chỉ quan hệ ngữ pháp giữa danh từ và vị ngữ (ví dụ: が, を, に, で, へ, と, から, より)'),
    (gen_random_uuid(), '助詞-準体助詞', 'Trợ từ danh từ hoá', '準体助詞', 'Trợ từ thay thế hoặc danh từ hoá ngữ (ví dụ: の - trong 赤いのが好き)'),
    (gen_random_uuid(), '助詞-終助詞', 'Trợ từ cuối câu (Thán trợ từ)', '終助詞', 'Biểu thị ngữ điệu, cảm xúc người nói ở cuối câu (ví dụ: ね, よ, な, わ, か)'),
    (gen_random_uuid(), '動詞-一般', 'Động từ thông thường', '一般動詞', 'Động từ độc lập biểu thị hành động, trạng thái (ví dụ: 走る, 読む, 聞く)'),
    (gen_random_uuid(), '動詞-非自立可能', 'Động từ có thể làm trợ động từ', '非自立可能動詞', 'Động từ phụ/hỗ trợ ngữ pháp khi đi sau て (ví dụ: いる, みる, くる, いく, くれる)'),
    (gen_random_uuid(), '名詞-助動詞語幹', 'Gốc danh từ làm trợ động từ', '助動詞語幹', 'Gốc từ danh từ có chức năng tương tự trợ động từ'),
    (gen_random_uuid(), '名詞-固有名詞-一般', 'Danh từ riêng thông thường', '固有名詞（一般）', 'Tên tổ chức, thương hiệu, tác phẩm (ví dụ: Google, トヨタ)'),
    (gen_random_uuid(), '名詞-固有名詞-人名-一般', 'Tên người chung', '人名（一般）', 'Tên nhân vật lịch sử, tên đầy đủ'),
    (gen_random_uuid(), '名詞-固有名詞-人名-名', 'Tên riêng (Tên gọi)', '人名（名）', 'Phần tên trong họ tên người (ví dụ: 太郎, 花子, Huy)'),
    (gen_random_uuid(), '名詞-固有名詞-人名-姓', 'Họ của người', '人名（姓）', 'Phần họ trong họ tên người (ví dụ: 田中, 佐藤, Nguyễn)'),
    (gen_random_uuid(), '名詞-固有名詞-地名-一般', 'Địa danh chung', '地名（一般）', 'Tên thành phố, quận, địa điểm (ví dụ: 銀座, 新宿, 京都)'),
    (gen_random_uuid(), '名詞-固有名詞-地名-国', 'Tên quốc gia', '地名（国）', 'Tên đất nước (ví dụ: 日本, ベトナム, アメリカ)'),
    (gen_random_uuid(), '名詞-数詞', 'Số từ', '数詞', 'Chữ số, số lượng (ví dụ: 一, 二, 1, 2)'),
    (gen_random_uuid(), '名詞-普通名詞-サ変可能', 'Danh từ có thể ghép する', 'サ変可能名詞', 'Danh từ hành động (ví dụ: 勉強, 説明, 案内, 連絡)'),
    (gen_random_uuid(), '名詞-普通名詞-サ変形状詞可能', 'Danh từ vừa làm する vừa làm tính từ な', 'サ変形状詞可能名詞', 'Từ linh hoạt (ví dụ: 安心, 心配)'),
    (gen_random_uuid(), '名詞-普通名詞-一般', 'Danh từ chung', '普通名詞（一般）', 'Danh từ thông dụng chỉ sự vật, hiện tượng (ví dụ: 友達, 言葉, 本)'),
    (gen_random_uuid(), '名詞-普通名詞-副詞可能', 'Danh từ kiêm phó từ', '副詞可能名詞', 'Danh từ có thể dùng độc lập như phó từ chỉ thời gian (ví dụ: 昨日, 今日, 今)'),
    (gen_random_uuid(), '名詞-普通名詞-助数詞可能', 'Danh từ có thể làm lượng từ', '助数詞可能名詞', 'Danh từ dùng kèm số để đếm (ví dụ: 箇, 冊)'),
    (gen_random_uuid(), '名詞-普通名詞-形状詞可能', 'Danh từ có thể làm tính từ な', '形状詞可能名詞', 'Danh từ có thể đi với な (ví dụ: 自由, 平和)'),
    (gen_random_uuid(), '形容詞-一般', 'Tính từ đuôi い', 'い形容詞', 'Tính từ i thuần túy (ví dụ: 美しい, 高い, 寒い)'),
    (gen_random_uuid(), '形容詞-非自立可能', 'Tính từ đuôi い phụ thuộc', '非自立可能形容詞', 'Tính từ đuôi i làm bổ ngữ phụ thuộc (ví dụ: ない, よい, ほしい)'),
    (gen_random_uuid(), '形状詞-タリ', 'Tính từ hình thái たり', 'タリ形状詞', 'Tính từ cổ/văn viết kết thúc bằng たり (ví dụ: 堂々, 悠々)'),
    (gen_random_uuid(), '形状詞-一般', 'Tính từ đuôi な (Hình trạng từ)', 'な形容詞（形状詞）', 'Tính từ na thông dụng (ví dụ: 丁寧, 静か, 便利, きれい)'),
    (gen_random_uuid(), '形状詞-助動詞語幹', 'Gốc hình trạng từ đi kèm trợ động từ', '形状詞助動詞語幹', 'Gốc từ tính từ ghép trợ từ'),
    (gen_random_uuid(), '感動詞-フィラー', 'Từ đệm ngập ngừng', 'フィラー', 'Âm thanh đệm khi suy nghĩ trong hội thoại (ví dụ: ええと, あのー, うーん)'),
    (gen_random_uuid(), '感動詞-一般', 'Thán từ chung', '感動詞（一般）', 'Tiếng chào, kêu gọi, cảm thán (ví dụ: はい, いいえ, ありがとう, こんにちは)'),
    (gen_random_uuid(), '接尾辞-動詞的', 'Hậu tố tạo động từ', '動詞的接尾辞', 'Hậu tố gắn sau tạo động từ (ví dụ: ぶる, づける)'),
    (gen_random_uuid(), '接尾辞-名詞的-サ変可能', 'Hậu tố tạo danh từ ghép する', 'サ変可能名詞的接尾辞', 'Hậu tố biến thành danh từ sahen (ví dụ: 化, 視)'),
    (gen_random_uuid(), '接尾辞-名詞的-一般', 'Hậu tố tạo danh từ chung', '名詞的接尾辞（一般）', 'Hậu tố chỉ người, vật, tình trạng (ví dụ: たち, 達, 性)'),
    (gen_random_uuid(), '接尾辞-名詞的-副詞可能', 'Hậu tố tạo phó từ', '副詞可能名詞的接尾辞', 'Hậu tố tạo từ chỉ thời gian, tần suất (ví dụ: ごと, 毎)'),
    (gen_random_uuid(), '接尾辞-名詞的-助数詞', 'Hậu tố lượng từ (Đơn vị đếm)', '助数詞', 'Đơn vị đếm đứng sau số từ (ví dụ: 個, 本, 人, 匹, 歳)'),
    (gen_random_uuid(), '接尾辞-形容詞的', 'Hậu tố tạo tính từ đuôi い', '形容詞的接尾辞', 'Hậu tố biến từ thành tính từ đuôi i (ví dụ: ぽい, らしい)'),
    (gen_random_uuid(), '接尾辞-形状詞的', 'Hậu tố tạo tính từ đuôi な', '形状詞的接尾辞', 'Hậu tố biến từ thành tính từ đuôi na (ví dụ: 的, 風)'),
    (gen_random_uuid(), '接続詞', 'Liên từ', '接続詞', 'Từ nối câu độc lập (ví dụ: しかし, だから, また, そして)'),
    (gen_random_uuid(), '接頭辞', 'Tiếp đầu ngữ (Tiền tố)', '接頭辞', 'Đứng trước danh từ/động từ để lịch sự hoặc bổ nghĩa (ví dụ: お, ご, 御, 第, 超)'),
    (gen_random_uuid(), '空白', 'Khoảng trắng', '空白', 'Khoảng trống giữa các từ hoặc thụt lề'),
    (gen_random_uuid(), '補助記号-一般', 'Ký hiệu phụ trợ chung', '補助記号（一般）', 'Các ký tự biểu tượng, dấu chấm lửng (ví dụ: …, ・)'),
    (gen_random_uuid(), '補助記号-句点', 'Dấu chấm hết câu', '句点', 'Dấu chấm tròn tiếng Nhật (。)'),
    (gen_random_uuid(), '補助記号-括弧閉', 'Dấu đóng ngoặc', '閉じ括弧', 'Ngoặc đóng (ví dụ: 」, ）, 】)'),
    (gen_random_uuid(), '補助記号-括弧開', 'Dấu mở ngoặc', '開き括弧', 'Ngoặc mở (ví dụ: 「, （, 【)'),
    (gen_random_uuid(), '補助記号-読点', 'Dấu phẩy ngắt nhịp', '読点', 'Dấu phẩy tiếng Nhật (、)'),
    (gen_random_uuid(), '補助記号-ＡＡ-一般', 'Ký hiệu ASCII Art', 'アスキーアート', 'Hình vẽ tạo từ ký tự text'),
    (gen_random_uuid(), '補助記号-ＡＡ-顔文字', 'Ký hiệu mặt cười Kaomoji', '顔文字', 'Biểu tượng cảm xúc Nhật (ví dụ: (^^), (T_T))'),
    (gen_random_uuid(), '記号-一般', 'Ký hiệu chung', '記号（一般）', 'Ký hiệu văn bản thông dụng'),
    (gen_random_uuid(), '記号-文字', 'Ký hiệu chữ cái đặc biệt', '文字記号', 'Ký hiệu như chữ La Tinh, chữ cái Hy Lạp'),
    (gen_random_uuid(), '連体詞', 'Liên thể từ', '連体詞', 'Từ chỉ định không biến đổi đứng trước danh từ (ví dụ: この, その, あの, ある, いわゆる)');

-- ----------------------------------------------------------------------------------------------------
-- 2. DEPENDENCY RELATIONSHIPS (Universal Dependencies - 29 tags)
-- ----------------------------------------------------------------------------------------------------
INSERT INTO knowledge.dependency_relationships (id, code, name, japanese_name, description)
VALUES
(
        gen_random_uuid(),
        'dep',
        'Quan hệ phụ thuộc tổng quát',
        '依存関係',
        'Quan hệ phụ thuộc tổng quát được mô hình phân tích cú pháp xác định'
    ),
    (
        gen_random_uuid(),
        'expl',
        'Thành phần expletive',
        '虚辞',
        'Thành phần ngữ pháp đặc biệt trong cấu trúc câu'
    ),
    (
        gen_random_uuid(),
        'parataxis',
        'Quan hệ mệnh đề song song',
        '並列的な節の関係',
        'Quan hệ giữa các mệnh đề hoặc phát ngôn đặt cạnh nhau mà không có liên kết cú pháp phụ thuộc thông thường'
    ),
    (
        gen_random_uuid(),
        'orphan',
        'Thành phần trong cấu trúc lược bỏ',
        '孤立成分',
        'Thành phần còn lại trong cấu trúc cú pháp có thành phần bị lược bỏ'
    ),
    (
        gen_random_uuid(),
        'reparandum',
        'Thành phần bị sửa lại trong lời nói',
        '言い直しの対象',
        'Thành phần người nói ngắt hoặc sửa lại trong lời nói tự nhiên'
    ),
    (
        gen_random_uuid(),
        'goeswith',
        'Thành phần được phân tích đi cùng nhau',
        '結合成分',
        'Quan hệ giữa các thành phần được phân tích là đi cùng nhau'
    ),
    (
        gen_random_uuid(),
        'preconj',
        'Thành phần kết hợp đứng trước',
        '前置等位接続要素',
        'Thành phần kết hợp đứng trước trong cấu trúc đẳng lập'
    ),
    (
        gen_random_uuid(),
        'meta',
        'Thành phần siêu ngôn ngữ',
        'メタ言語要素',
        'Thành phần dùng để diễn đạt nhận xét hoặc thông tin siêu ngôn ngữ'
    ),
    (gen_random_uuid(), 'root', 'Vị ngữ trung tâm (Gốc câu)', '文の根（ルート）', 'Động từ/danh từ vị ngữ đóng vai trò trung tâm của cả câu'),
    (gen_random_uuid(), 'nsubj', 'Chủ ngữ danh từ', '名詞主語', 'Chủ ngữ của câu hoặc của mệnh đề phụ (đi với が hoặc は)'),
    (gen_random_uuid(), 'obj', 'Tân ngữ trực tiếp', '直接目的語', 'Tân ngữ nhận tác động trực tiếp của động từ (đi với を)'),
    (gen_random_uuid(), 'iobj', 'Tân ngữ gián tiếp', '間接目的語', 'Đối tượng tiếp nhận hành động (thường đi với に)'),
    (gen_random_uuid(), 'obl', 'Trạng ngữ danh từ', '斜格名詞', 'Thành phần danh ngữ chỉ nơi chốn, thời gian, công cụ, đối tác (đi với で, に, から, について)'),
    (gen_random_uuid(), 'advmod', 'Bổ ngữ phó từ', '副詞修飾語', 'Phó từ bổ nghĩa cho động từ, tính từ hoặc câu (ví dụ: 昨日, とても)'),
    (gen_random_uuid(), 'advcl', 'Mệnh đề trạng ngữ phụ', '連用修飾節', 'Mệnh đề phụ chỉ nguyên nhân, điều kiện, tiền đề, so sánh (ví dụ: たら, て, より)'),
    (gen_random_uuid(), 'acl', 'Mệnh đề định ngữ', '連体修飾節', 'Mệnh đề bổ nghĩa cho danh từ đứng sau (ví dụ: 勉強をしている [友達], 分からない [言葉])'),
    (gen_random_uuid(), 'amod', 'Bổ ngữ tính từ', '形容詞修飾語', 'Tính từ bổ nghĩa trực tiếp cho danh từ (ví dụ: 美しい 花)'),
    (gen_random_uuid(), 'nmod', 'Bổ ngữ danh từ', '名詞修飾語', 'Danh từ bổ nghĩa cho danh từ khác qua trợ từ の (ví dụ: 試験の [勉強])'),
    (gen_random_uuid(), 'nummod', 'Bổ ngữ số lượng', '数量修飾語', 'Số lượng bổ nghĩa cho danh từ (ví dụ: 3冊の 本)'),
    (gen_random_uuid(), 'compound', 'Từ ghép phức hợp', '複合語', 'Thành phần tạo nên danh từ ghép phức hợp (ví dụ: 日本語 + 能力 + 試験, 接頭辞 ご + 一緒)'),
    (gen_random_uuid(), 'case', 'Trợ từ cách', '格助詞', 'Trợ từ gắn với danh từ để thể hiện vai trò cách cú pháp (ví dụ: の, を, に, で, より)'),
    (gen_random_uuid(), 'mark', 'Trợ từ kết nối mệnh đề', '節標識（マーカー）', 'Trợ từ hoặc liên từ đánh dấu kết thúc mệnh đề phụ (ví dụ: て, から, ので)'),
    (gen_random_uuid(), 'aux', 'Trợ từ/Trợ động từ bổ trợ', '助動詞', 'Trợ động từ gắn kèm vị ngữ chỉ thì, thể, phủ định, kính ngữ (ví dụ: た, ます, ない, だ)'),
    (gen_random_uuid(), 'cop', 'Hệ từ liên kết', 'コピュラ（繋合詞）', 'Từ nối định danh quan hệ A là B (ví dụ: だ, である)'),
    (gen_random_uuid(), 'fixed', 'Cụm từ ngữ cố định', '固定表現', 'Cụm từ nhiều thành phần kết hợp thành một khối ngữ pháp (ví dụ: に+つい+て, て+みる, て+いる, て+くれる)'),
    (gen_random_uuid(), 'flat', 'Cụm tên ngang hàng', '並列名称（フラット）', 'Các từ ghép tên người, danh hiệu không có quan hệ phân cấp (ví dụ: 田中 太郎)'),
    (gen_random_uuid(), 'conj', 'Thành phần đẳng lập', '等位項', 'Thành phần được liên kết đẳng lập với từ phía trước (ví dụ: リンゴ と バナナ)'),
    (gen_random_uuid(), 'cc', 'Liên từ kết hợp', '等位接続詞', 'Liên từ liên kết thành phần đẳng lập (ví dụ: そして, また)'),
    (gen_random_uuid(), 'csubj', 'Mệnh đề làm chủ ngữ', '節主語', 'Mệnh đề đóng vai trò chủ ngữ của câu'),
    (gen_random_uuid(), 'ccomp', 'Mệnh đề bổ ngữ nội dung', '補文節', 'Mệnh đề trích dẫn hoặc làm bổ ngữ cho động từ nhận thức (ví dụ: と 思う)'),
    (gen_random_uuid(), 'det', 'Từ chỉ định', '限定詞（連体詞）', 'Từ chỉ định đứng trước danh từ (ví dụ: この, その)'),
    (gen_random_uuid(), 'discourse', 'Thành phần đệm/giao tiếp', '談話要素', 'Thán từ hoặc thành phần tương tác đối thoại'),
    (gen_random_uuid(), 'dislocated', 'Thành phần chủ đề tách rời', '転位要素', 'Thành phần chuyển dịch chủ đề ở đầu câu đi với は'),
    (gen_random_uuid(), 'clf', 'Lượng từ / Đơn vị đếm', '助数詞', 'Từ đếm đi liền số lượng'),
    (gen_random_uuid(), 'appos', 'Đồng vị ngữ', '同格', 'Danh từ giải thích làm rõ cho danh từ đứng trước'),
    (gen_random_uuid(), 'vocative', 'Hô ngữ', '呼びかけ', 'Từ dùng để gọi đáp'),
    (gen_random_uuid(), 'punct', 'Dấu câu', '句読点', 'Dấu chấm, phẩy, ngoặc đơn');

-- ============================================
-- Seed data: Proficiency Levels - JLPT
-- ============================================
INSERT INTO knowledge.proficiency_levels (
    id,
    code,
    name,
    japanese_name,
    sort_order,
    description,
    is_active,
    created_at,
    updated_at
)
VALUES
    (
        '20000000-0000-0000-0000-000000000101',
        'N5',
        'Sơ cấp',
        '初級',
        1,
        'The ability to understand some basic Japanese.',
        true,
        NOW(),
        NOW()
    ),
    (
        '20000000-0000-0000-0000-000000000102',
        'N4',
        'Sơ trung cấp',
        '初中級',
        2,
        'The ability to understand basic Japanese.',
        true,
        NOW(),
        NOW()
    ),
    (
        '20000000-0000-0000-0000-000000000103',
        'N3',
        'Trung cấp',
        '中級',
        3,
        'The ability to understand Japanese used in everyday situations to a certain degree.',
        true,
        NOW(),
        NOW()
    ),
    (
        '20000000-0000-0000-0000-000000000104',
        'N2',
        'Trung cao cấp',
        '中上級',
        4,
        'The ability to understand Japanese used in everyday situations and in a variety of circumstances to a certain degree.',
        true,
        NOW(),
        NOW()
    ),
    (
        '20000000-0000-0000-0000-000000000105',
        'N1',
        'Cao cấp',
        '上級',
        5,
        'The ability to understand Japanese used in a variety of circumstances.',
        true,
        NOW(),
        NOW()
    );


-- ============================================
-- Seed data: Content Categories
-- Hierarchical structure
-- ============================================
INSERT INTO knowledge.content_categories (
    id,
    slug,
    code,
    name,
    japanese_name,
    description,
    display_order,
    theme_color,
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
        '学習と開発',
        'Learning and personal development',
        1,
        '#FF5733',
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000002',
        'entertainment',
        'ENTERTAINMENT',
        'Entertainment & Media',
        'エンターテイメントとメディア',
        'Entertainment, movies, music, and media',
        2,
        '#FF5733',
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000003',
        'business-finance',
        'BUSINESS_FINANCE',
        'Business & Finance',
        'ビジネスとファイナンス',
        'Business, finance, and career topics',
        3,
        '#FF5733',
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000004',
        'health-wellness',
        'HEALTH_WELLNESS',
        'Health & Wellness',
        '健康とウェルネス',
        'Health, sports, and wellness topics',
        4,
        '#FF5733',
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000005',
        'culture-society',
        'CULTURE_SOCIETY',
        'Culture & Society',
        '文化と社会',
        'Culture, history, politics, and social topics',
        5,
        '#FF5733',
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000006',
        'science-nature',
        'SCIENCE_NATURE',
        'Science & Nature',
        '科学と自然',
        'Science, technology, and environment',
        6,
        '#FF5733',
        TRUE,
        NOW(),
        NOW()
    ),
    (
        '50000000-0000-0000-0000-000000000701',
        'travel',
        'TRAVEL',
        'Travel & Tourism',
        '旅行と観光',
        'Travel, tourism, and geographical exploration',
        7,
        '#FF5733',
        TRUE,
        NOW(),
        NOW()
    );


-- ============================================
-- Seed data: Roles
-- ============================================
INSERT INTO identity.roles (
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
VALUES
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        'System Administrator',
        'admin@rakushu.com',
        'rakushu@1',
        '11111111-1111-1111-1111-111111111111',
        'Active',
        NOW(),
        NOW()
    ),
    (
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        'Linguistic Curator',
        'curator@rakushu.com',
        'rakushu@1',
        '22222222-2222-2222-2222-222222222222',
        'Active',
        NOW(),
        NOW()
    ),
    (
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        'Learner One',
        'learner1@rakushu.com',
        'rakushu@1',
        '33333333-3333-3333-3333-333333333333',
        'Active',
        NOW(),
        NOW()
    ),
    (
        'dddddddd-dddd-dddd-dddd-dddddddddddd',
        'Learner Two',
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
VALUES
	(
		'60000000-0000-0000-0000-000000000003',
		'cccccccc-cccc-cccc-cccc-cccccccccccc',
		NULL,
		'20000000-0000-0000-0000-000000000101',
		90,
		45,
		NOW(),
		NOW()
	),
	(
		'60000000-0000-0000-0000-000000000004',
		'dddddddd-dddd-dddd-dddd-dddddddddddd',
		NULL,
		'20000000-0000-0000-0000-000000000101',
		75,
		40,
		NOW(),
		NOW()
	);


-- ============================================
-- Seed data: User Interests
-- ============================================
INSERT INTO identity.interests (
	id,
	profile_id,
	content_category_id,
	priority
)
VALUES
	-- Learner One - Interests in Travel, Learning, Entertainment
	(
		'70000000-0000-0000-0000-000000000001',
		'60000000-0000-0000-0000-000000000003',
		'50000000-0000-0000-0000-000000000701',
		3
	),
	(
		'70000000-0000-0000-0000-000000000002',
		'60000000-0000-0000-0000-000000000003',
		'50000000-0000-0000-0000-000000000001',
		2
	),
	(
		'70000000-0000-0000-0000-000000000003',
		'60000000-0000-0000-0000-000000000003',
		'50000000-0000-0000-0000-000000000002',
		1
	),

	-- Learner Two - Interests in Learning, Science, Business
	(
		'70000000-0000-0000-0000-000000000004',
		'60000000-0000-0000-0000-000000000004',
		'50000000-0000-0000-0000-000000000001',
		3
	),
	(
		'70000000-0000-0000-0000-000000000005',
		'60000000-0000-0000-0000-000000000004',
		'50000000-0000-0000-0000-000000000006',
		2
	),
	(
		'70000000-0000-0000-0000-000000000006',
		'60000000-0000-0000-0000-000000000004',
		'50000000-0000-0000-0000-000000000003',
		1
	);
COMMIT;

BEGIN;

-- =========================================================
-- RAKUSHU SUBSCRIPTION / PAYMENT SEED
-- Correct insertion order:
-- features -> plans -> entitlements -> payments -> transactions
-- -> subscriptions -> subscription_usages
-- =========================================================

-- =========================================================
-- FEATURES
-- =========================================================

INSERT INTO subscription.features
(id, code, name, description, is_active, created_at, updated_at)
VALUES
('11111111-1111-1111-1111-111111111111','VIDEO','Video','Create and learn from Japanese learning videos with interactive subtitle.',true,NOW(),NOW()),
('22222222-2222-2222-2222-222222222222','CHAT','AI Chat','Chat with AI for Japanese learning assistance.',true,NOW(),NOW()),
('33333333-3333-3333-3333-333333333333','QUIZ','Quiz','Practice Japanese through SRS quizzes.',true,NOW(),NOW()),
('44444444-4444-4444-4444-444444444444','CHARACTER','Character','Practice speaking Japanese by imitating characters.',true,NOW(),NOW());

-- =========================================================
-- PLANS
-- =========================================================

INSERT INTO subscription.plans
(id, code, name, japanese_name, description, price, currency, billing_cycle, is_active, created_at, updated_at)
VALUES
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa','HAJIME','Hajime','初め','A free plan for learners who are just getting started.',0.00,'VND','Weekly',true,NOW(),NOW()),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb','MANABU','Manabu','学ぶ','A learning plan for learners who want to practice Japanese regularly.',99000.00,'VND','Monthly',true,NOW(),NOW()),
('cccccccc-cccc-cccc-cccc-cccccccccccc','JOTATSU','Jōtatsu','上達','An advanced plan for serious Japanese learners.',199000.00,'VND','Monthly',true,NOW(),NOW());

-- =========================================================
-- ENTITLEMENTS
-- =========================================================

INSERT INTO subscription.entitlements
(id, plan_id, feature_id, is_enabled, limit_value, limit_unit, limit_period)
VALUES
-- HAJIME
('a1111111-1111-1111-1111-111111111111','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa','11111111-1111-1111-1111-111111111111',true,3,'Video','Total'),
('a2222222-2222-2222-2222-222222222222','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa','22222222-2222-2222-2222-222222222222',true,30,'Chat','Total'),
('a3333333-3333-3333-3333-333333333333','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa','33333333-3333-3333-3333-333333333333',true,3,'Quiz','Total'),
('a4444444-4444-4444-4444-444444444444','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa','44444444-4444-4444-4444-444444444444',true,1,'Character','Total'),

-- MANABU
('b1111111-1111-1111-1111-111111111111','bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb','11111111-1111-1111-1111-111111111111',true,5,'Video','Day'),
('b2222222-2222-2222-2222-222222222222','bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb','22222222-2222-2222-2222-222222222222',true,100,'Chat','Day'),
('b3333333-3333-3333-3333-333333333333','bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb','33333333-3333-3333-3333-333333333333',true,5,'Quiz','Day'),
('b4444444-4444-4444-4444-444444444444','bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb','44444444-4444-4444-4444-444444444444',true,5,'Character','Day'),

-- JOTATSU
('c1111111-1111-1111-1111-111111111111','cccccccc-cccc-cccc-cccc-cccccccccccc','11111111-1111-1111-1111-111111111111',true,10,'Video','Day'),
('c2222222-2222-2222-2222-222222222222','cccccccc-cccc-cccc-cccc-cccccccccccc','22222222-2222-2222-2222-222222222222',true,500,'Chat','Day'),
('c3333333-3333-3333-3333-333333333333','cccccccc-cccc-cccc-cccc-cccccccccccc','33333333-3333-3333-3333-333333333333',true,10,'Quiz','Day'),
('c4444444-4444-4444-4444-444444444444','cccccccc-cccc-cccc-cccc-cccccccccccc','44444444-4444-4444-4444-444444444444',true,10,'Character','Day');

-- =========================================================
-- PAYMENTS
-- =========================================================

INSERT INTO subscription.payments
(id, user_id, plan_id, amount, currency, status, expired_at, created_at, updated_at)
VALUES
('11111111-0001-0001-0001-000000000001','cccccccc-cccc-cccc-cccc-cccccccccccc','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',0.00,'VND','Completed',NOW()-INTERVAL '4 days 23 hours 45 minutes',NOW()-INTERVAL '5 days',NOW()-INTERVAL '5 days'),
('11111111-0002-0002-0002-000000000002','cccccccc-cccc-cccc-cccc-cccccccccccc','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',0.00,'VND','Completed',NOW()-INTERVAL '3 days 23 hours 45 minutes',NOW()-INTERVAL '4 days',NOW()-INTERVAL '4 days'),
('11111111-0003-0003-0003-000000000003','cccccccc-cccc-cccc-cccc-cccccccccccc','bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',99000.00,'VND','Pending',NOW()+INTERVAL '10 minutes',NOW()-INTERVAL '5 minutes',NOW()-INTERVAL '1 hour'),
('11111111-0004-0004-0004-000000000004','cccccccc-cccc-cccc-cccc-cccccccccccc','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',0.00,'VND','Cancelled',NOW()-INTERVAL '1 day 23 hours 45 minutes',NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'),
('11111111-0005-0005-0005-000000000005','cccccccc-cccc-cccc-cccc-cccccccccccc','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',0.00,'VND','Cancelled',NOW()-INTERVAL '2 days 23 hours 45 minutes',NOW()-INTERVAL '3 days',NOW()-INTERVAL '3 days'),
('11111111-0006-0006-0006-000000000006','dddddddd-dddd-dddd-dddd-dddddddddddd','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',0.00,'VND','Completed',NOW()-INTERVAL '5 days 23 hours 45 minutes',NOW()-INTERVAL '6 days',NOW()-INTERVAL '6 days'),
('11111111-0007-0007-0007-000000000007','dddddddd-dddd-dddd-dddd-dddddddddddd','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',0.00,'VND','Expired',NOW()-INTERVAL '1 minute',NOW()-INTERVAL '30 minutes',NOW()-INTERVAL '30 minutes'),
('11111111-0008-0008-0008-000000000008','dddddddd-dddd-dddd-dddd-dddddddddddd','bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',99000.00,'VND','Completed',NOW()-INTERVAL '6 days 23 hours 45 minutes',NOW()-INTERVAL '7 days',NOW()-INTERVAL '7 days'),
('11111111-0009-0009-0009-000000000009','dddddddd-dddd-dddd-dddd-dddddddddddd','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',0.00,'VND','Pending',NOW()+INTERVAL '10 minutes',NOW()-INTERVAL '5 minutes',NOW()-INTERVAL '30 minutes'),
('11111111-0010-0010-0010-000000000010','cccccccc-cccc-cccc-cccc-cccccccccccc','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',0.00,'VND','Completed',NOW()-INTERVAL '7 days 23 hours 45 minutes',NOW()-INTERVAL '8 days',NOW()-INTERVAL '8 days'),

('11111111-0011-0011-0011-000000000011','cccccccc-cccc-cccc-cccc-cccccccccccc','bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',99000.00,'VND','Completed',NOW()-INTERVAL '3 hours 45 minutes',NOW()-INTERVAL '4 hours',NOW()-INTERVAL '4 hours'),
('11111111-0012-0012-0012-000000000012','cccccccc-cccc-cccc-cccc-cccccccccccc','cccccccc-cccc-cccc-cccc-cccccccccccc',199000.00,'VND','Completed',NOW()-INTERVAL '4 hours 45 minutes',NOW()-INTERVAL '5 hours',NOW()-INTERVAL '5 hours'),
('11111111-0013-0013-0013-000000000013','dddddddd-dddd-dddd-dddd-dddddddddddd','cccccccc-cccc-cccc-cccc-cccccccccccc',199000.00,'VND','Completed',NOW()-INTERVAL '5 hours 45 minutes',NOW()-INTERVAL '6 hours',NOW()-INTERVAL '6 hours'),
('11111111-0014-0014-0014-000000000014','cccccccc-cccc-cccc-cccc-cccccccccccc','cccccccc-cccc-cccc-cccc-cccccccccccc',199000.00,'VND','Completed',NOW()-INTERVAL '40 days'+INTERVAL '15 minutes',NOW()-INTERVAL '40 days',NOW()-INTERVAL '40 days'),
('11111111-0015-0015-0015-000000000015','dddddddd-dddd-dddd-dddd-dddddddddddd','cccccccc-cccc-cccc-cccc-cccccccccccc',199000.00,'VND','Completed',NOW()-INTERVAL '2 days 45 minutes',NOW()-INTERVAL '3 days',NOW()-INTERVAL '3 days');

-- =========================================================
-- TRANSACTIONS
-- =========================================================

INSERT INTO subscription.transactions
(id, payment_id, provider, amount, currency, txn_ref, url, transaction_no, raw_response_payload, status, expired_at, created_at, updated_at)
VALUES
('22222222-0001-0001-0001-000000000001','11111111-0001-0001-0001-000000000001','VNPAY',0.00,'VND','TXNREF_00001_20250125','https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00001_20250125','0000000001','{"responseCode":"00","message":"Success"}','Successful',NOW()-INTERVAL '4 days 23 hours 45 minutes',NOW()-INTERVAL '5 days',NOW()-INTERVAL '5 days'),
('22222222-0002-0002-0002-000000000002','11111111-0002-0002-0002-000000000002','SEPAY',0.00,'VND','TXNREF_00002_20250126','https://sepay.vn/payment?transactionId=TXNREF_00002_20250126','0000000002','{"status":"success","code":0}','Successful',NOW()-INTERVAL '3 days 23 hours 45 minutes',NOW()-INTERVAL '4 days',NOW()-INTERVAL '4 days'),
('22222222-0003-0003-0003-000000000003','11111111-0003-0003-0003-000000000003','VNPAY',99000.00,'VND','TXNREF_00003_20250127','https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00003_20250127',NULL,NULL,'Pending',NOW()+INTERVAL '14 minutes',NOW()-INTERVAL '5 minutes',NOW()-INTERVAL '1 hour'),
('22222222-0003-0003-0003-000000000004','11111111-0003-0003-0003-000000000003','SEPAY',99000.00,'VND','TXNREF_00003B_20250127','https://sepay.vn/payment?transactionId=TXNREF_00003B_20250127',NULL,NULL,'Pending',NOW()+INTERVAL '14 minutes',NOW()-INTERVAL '1 hour',NOW()-INTERVAL '1 hour'),
('22222222-0004-0004-0004-000000000005','11111111-0004-0004-0004-000000000004','VNPAY',0.00,'VND','TXNREF_00004_20250128','https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00004_20250128',NULL,'{"responseCode":"24","message":"Cancelled"}','Cancelled',NOW()-INTERVAL '1 day 23 hours 45 minutes',NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'),
('22222222-0005-0005-0005-000000000006','11111111-0005-0005-0005-000000000005','SEPAY',0.00,'VND','TXNREF_00005_20250129','https://sepay.vn/payment?transactionId=TXNREF_00005_20250129',NULL,'{"status":"cancelled","code":1}','Cancelled',NOW()-INTERVAL '2 days 23 hours 45 minutes',NOW()-INTERVAL '3 days',NOW()-INTERVAL '3 days'),
('22222222-0006-0006-0006-000000000007','11111111-0006-0006-0006-000000000006','VNPAY',0.00,'VND','TXNREF_00006_20250130','https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00006_20250130','0000000006','{"responseCode":"00","message":"Success"}','Successful',NOW()-INTERVAL '5 days 23 hours 45 minutes',NOW()-INTERVAL '6 days',NOW()-INTERVAL '6 days'),
('22222222-0007-0007-0007-000000000008','11111111-0007-0007-0007-000000000007','SEPAY',0.00,'VND','TXNREF_00007_20250131','https://sepay.vn/payment?transactionId=TXNREF_00007_20250131',NULL,NULL,'Expired',NOW()-INTERVAL '1 minute',NOW()-INTERVAL '30 minutes',NOW()-INTERVAL '30 minutes'),
('22222222-0008-0008-0008-000000000009','11111111-0008-0008-0008-000000000008','VNPAY',99000.00,'VND','TXNREF_00008_20250201','https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00008_20250201','0000000008','{"responseCode":"00","message":"Success"}','Successful',NOW()-INTERVAL '6 days 23 hours 45 minutes',NOW()-INTERVAL '7 days',NOW()-INTERVAL '7 days'),
('22222222-0008-0008-0008-000000000010','11111111-0008-0008-0008-000000000008','SEPAY',99000.00,'VND','TXNREF_00008B_20250201','https://sepay.vn/payment?transactionId=TXNREF_00008B_20250201',NULL,NULL,'Successful',NOW()+INTERVAL '15 minutes',NOW()-INTERVAL '7 days',NOW()-INTERVAL '7 days'),
('22222222-0009-0009-0009-000000000011','11111111-0009-0009-0009-000000000009','VNPAY',0.00,'VND','TXNREF_00009_20250202','https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00009_20250202',NULL,NULL,'Pending',NOW()+INTERVAL '10 minutes',NOW()-INTERVAL '5 minutes',NOW()-INTERVAL '30 minutes'),
('22222222-0009-0009-0009-000000000012','11111111-0009-0009-0009-000000000009','SEPAY',0.00,'VND','TXNREF_00009B_20250202','https://sepay.vn/payment?transactionId=TXNREF_00009B_20250202',NULL,NULL,'Pending',NOW()+INTERVAL '15 minutes',NOW()-INTERVAL '30 minutes',NOW()-INTERVAL '30 minutes'),
('22222222-0010-0010-0010-000000000013','11111111-0010-0010-0010-000000000010','VNPAY',0.00,'VND','TXNREF_00010_20250203','https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00010_20250203','0000000010','{"responseCode":"00","message":"Success"}','Successful',NOW()-INTERVAL '7 days 23 hours 45 minutes',NOW()-INTERVAL '8 days',NOW()-INTERVAL '8 days'),
('22222222-0010-0010-0010-000000000014','11111111-0010-0010-0010-000000000010','SEPAY',0.00,'VND','TXNREF_00010B_20250203','https://sepay.vn/payment?transactionId=TXNREF_00010B_20250203','0000000010B','{"status":"success","code":0}','Successful',NOW()+INTERVAL '15 minutes',NOW()-INTERVAL '8 days',NOW()-INTERVAL '8 days'),
('22222222-0011-0011-0011-000000000015','11111111-0011-0011-0011-000000000011','VNPAY',99000.00,'VND','TXNREF_00011_20250204','https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00011_20250204','0000000011','{"responseCode":"00","message":"Success"}','Successful',NOW()-INTERVAL '3 hours 45 minutes',NOW()-INTERVAL '4 hours',NOW()-INTERVAL '4 hours'),
('22222222-0012-0012-0012-000000000016','11111111-0012-0012-0012-000000000012','SEPAY',199000.00,'VND','TXNREF_00012_20250205','https://sepay.vn/payment?transactionId=TXNREF_00012_20250205','0000000012','{"status":"success","code":0}','Successful',NOW()-INTERVAL '4 hours 45 minutes',NOW()-INTERVAL '5 hours',NOW()-INTERVAL '5 hours'),
('22222222-0013-0013-0013-000000000017','11111111-0013-0013-0013-000000000013','VNPAY',199000.00,'VND','TXNREF_00013_20250206','https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00013_20250206','0000000013','{"responseCode":"00","message":"Success"}','Successful',NOW()-INTERVAL '5 hours 45 minutes',NOW()-INTERVAL '6 hours',NOW()-INTERVAL '6 hours'),
('22222222-0014-0014-0014-000000000018','11111111-0014-0014-0014-000000000014','VNPAY',199000.00,'VND','TXNREF_00014_20250828','https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00014_20250828','0000000014','{"responseCode":"00","message":"Success"}','Successful',NOW()-INTERVAL '40 days'+INTERVAL '15 minutes',NOW()-INTERVAL '40 days',NOW()-INTERVAL '40 days'),
('22222222-0015-0015-0015-000000000019','11111111-0015-0015-0015-000000000015','SEPAY',199000.00,'VND','TXNREF_00015_20250829','https://sepay.vn/payment?transactionId=TXNREF_00015_20250829','0000000015','{"status":"success","code":0}','Successful',NOW()-INTERVAL '2 days 45 minutes',NOW()-INTERVAL '3 days',NOW()-INTERVAL '3 days');

-- =========================================================
-- SUBSCRIPTIONS
-- MUST COME BEFORE subscription_usages
-- =========================================================

INSERT INTO subscription.subscriptions
(id, user_id, payment_id, plan_id, status, start_at, end_at)
VALUES
('55555555-0001-0001-0001-000000000001','cccccccc-cccc-cccc-cccc-cccccccccccc',NULL,'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa','Active',NOW()-INTERVAL '5 days',NOW()-INTERVAL '5 days'+INTERVAL '7 days'),
('55555555-0002-0002-0002-000000000002','dddddddd-dddd-dddd-dddd-dddddddddddd',NULL,'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa','Active',NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'+INTERVAL '7 days'),
('55555555-0003-0003-0003-000000000003','cccccccc-cccc-cccc-cccc-cccccccccccc','11111111-0011-0011-0011-000000000011','bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb','Active',NOW()-INTERVAL '3 hours',NOW()-INTERVAL '3 hours'+INTERVAL '1 month'),
('55555555-0004-0004-0004-000000000004','dddddddd-dddd-dddd-dddd-dddddddddddd','11111111-0008-0008-0008-000000000008','bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb','Active',NOW()-INTERVAL '2 hours',NOW()-INTERVAL '2 hours'+INTERVAL '1 month'),
('55555555-0005-0005-0005-000000000005','cccccccc-cccc-cccc-cccc-cccccccccccc','11111111-0012-0012-0012-000000000012','cccccccc-cccc-cccc-cccc-cccccccccccc','Active',NOW()-INTERVAL '5 hours',NOW()-INTERVAL '5 hours'+INTERVAL '1 month'),
('55555555-0006-0006-0006-000000000006','dddddddd-dddd-dddd-dddd-dddddddddddd','11111111-0013-0013-0013-000000000013','cccccccc-cccc-cccc-cccc-cccccccccccc','Active',NOW()-INTERVAL '6 hours',NOW()-INTERVAL '6 hours'+INTERVAL '1 month'),
('55555555-0007-0007-0007-000000000007','cccccccc-cccc-cccc-cccc-cccccccccccc','11111111-0014-0014-0014-000000000014','cccccccc-cccc-cccc-cccc-cccccccccccc','Expired',NOW()-INTERVAL '40 days',NOW()-INTERVAL '40 days'+INTERVAL '1 month'),
('55555555-0008-0008-0008-000000000008','dddddddd-dddd-dddd-dddd-dddddddddddd','11111111-0015-0015-0015-000000000015','cccccccc-cccc-cccc-cccc-cccccccccccc','Canceled',NOW()-INTERVAL '3 days',NOW()-INTERVAL '3 days'+INTERVAL '1 month');

-- =========================================================
-- SUBSCRIPTION USAGES
-- MUST BE LAST
-- =========================================================

INSERT INTO subscription.subscription_usages
(id, subscription_id, feature_id, period_start, period_end, max_value, used_value, is_over_limit, is_expired, is_canceled, created_at, updated_at)
VALUES
-- Subscription 1 - Hajime Active: 1/3, 10/30, 2/3, 1/1
('66666666-0001-0001-0001-000000000001','55555555-0001-0001-0001-000000000001','11111111-1111-1111-1111-111111111111',NOW()-INTERVAL '5 days',NOW()-INTERVAL '5 days'+INTERVAL '7 days', 3,1,false,false,false,NOW()-INTERVAL '5 days',NOW()-INTERVAL '1 day'),
('66666666-0002-0002-0002-000000000002','55555555-0001-0001-0001-000000000001','22222222-2222-2222-2222-222222222222',NOW()-INTERVAL '5 days',NOW()-INTERVAL '5 days'+INTERVAL '7 days', 30, 10,false,false,false,NOW()-INTERVAL '5 days',NOW()-INTERVAL '1 day'),
('66666666-0003-0003-0003-000000000003','55555555-0001-0001-0001-000000000001','33333333-3333-3333-3333-333333333333',NOW()-INTERVAL '5 days',NOW()-INTERVAL '5 days'+INTERVAL '7 days', 3, 2,false,false,false,NOW()-INTERVAL '5 days',NOW()-INTERVAL '1 day'),
('66666666-0004-0004-0004-000000000004','55555555-0001-0001-0001-000000000001','44444444-4444-4444-4444-444444444444',NOW()-INTERVAL '5 days',NOW()-INTERVAL '5 days'+INTERVAL '7 days', 1,1,false,false,false,NOW()-INTERVAL '5 days',NOW()-INTERVAL '1 day'),

-- Subscription 2 - Hajime Active: unused
('66666666-0011-0011-0011-000000000011','55555555-0002-0002-0002-000000000002','11111111-1111-1111-1111-111111111111',NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'+INTERVAL '7 days', 3,0,false,false,false,NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'),
('66666666-0012-0012-0012-000000000012','55555555-0002-0002-0002-000000000002','22222222-2222-2222-2222-222222222222',NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'+INTERVAL '7 days', 30,0,false,false,false,NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'),
('66666666-0013-0013-0013-000000000013','55555555-0002-0002-0002-000000000002','33333333-3333-3333-3333-333333333333',NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'+INTERVAL '7 days', 3,0,false,false,false,NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'),
('66666666-0014-0014-0014-000000000014','55555555-0002-0002-0002-000000000002','44444444-4444-4444-4444-444444444444',NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'+INTERVAL '7 days', 1,0,false,false,false,NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'),

-- Subscription 3 - Manabu Active: 4/5, 70/100, 5/5, 3/5
('66666666-0021-0021-0021-000000000021','55555555-0003-0003-0003-000000000003','11111111-1111-1111-1111-111111111111',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second', 5,4,false,false,false,NOW()-INTERVAL '3 hours',NOW()-INTERVAL '30 minutes'),
('66666666-0022-0022-0022-000000000022','55555555-0003-0003-0003-000000000003','22222222-2222-2222-2222-222222222222',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',100,70,false,false,false,NOW()-INTERVAL '3 hours',NOW()-INTERVAL '30 minutes'),
('66666666-0023-0023-0023-000000000023','55555555-0003-0003-0003-000000000003','33333333-3333-3333-3333-333333333333',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',5,5,false,false,false,NOW()-INTERVAL '3 hours',NOW()-INTERVAL '30 minutes'),
('66666666-0024-0024-0024-000000000024','55555555-0003-0003-0003-000000000003','44444444-4444-4444-4444-444444444444',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',5,3,false,false,false,NOW()-INTERVAL '3 hours',NOW()-INTERVAL '30 minutes'),

-- Subscription 4 - Manabu Active: over limit
('66666666-0031-0031-0031-000000000031','55555555-0004-0004-0004-000000000004','11111111-1111-1111-1111-111111111111',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',5,5,true,false,false,NOW()-INTERVAL '2 hours',NOW()),
('66666666-0032-0032-0032-000000000032','55555555-0004-0004-0004-000000000004','22222222-2222-2222-2222-222222222222',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',100,100,true,false,false,NOW()-INTERVAL '2 hours',NOW()),
('66666666-0033-0033-0033-000000000033','55555555-0004-0004-0004-000000000004','33333333-3333-3333-3333-333333333333',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',5,5,false,false,false,NOW()-INTERVAL '2 hours',NOW()),
('66666666-0034-0034-0034-000000000034','55555555-0004-0004-0004-000000000004','44444444-4444-4444-4444-444444444444',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',5,5,false,false,false,NOW()-INTERVAL '2 hours',NOW()),

-- Subscription 5 - Jotatsu Active
('66666666-0041-0041-0041-000000000041','55555555-0005-0005-0005-000000000005','11111111-1111-1111-1111-111111111111',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',10,8,false,false,false,NOW()-INTERVAL '5 hours',NOW()-INTERVAL '1 hour'),
('66666666-0042-0042-0042-000000000042','55555555-0005-0005-0005-000000000005','22222222-2222-2222-2222-222222222222',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',500,450,false,false,false,NOW()-INTERVAL '5 hours',NOW()-INTERVAL '1 hour'),
('66666666-0043-0043-0043-000000000043','55555555-0005-0005-0005-000000000005','33333333-3333-3333-3333-333333333333',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',10,9,false,false,false,NOW()-INTERVAL '5 hours',NOW()-INTERVAL '1 hour'),
('66666666-0044-0044-0044-000000000044','55555555-0005-0005-0005-000000000005','44444444-4444-4444-4444-444444444444',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',10,7,false,false,false,NOW()-INTERVAL '5 hours',NOW()-INTERVAL '1 hour'),

-- Subscription 6 - Jotatsu Active: limit reached
('66666666-0051-0051-0051-000000000051','55555555-0006-0006-0006-000000000006','11111111-1111-1111-1111-111111111111',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',10,10,false,false,false,NOW()-INTERVAL '6 hours',NOW()),
('66666666-0052-0052-0052-000000000052','55555555-0006-0006-0006-000000000006','22222222-2222-2222-2222-222222222222',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',500,500,false,false,false,NOW()-INTERVAL '6 hours',NOW()),
('66666666-0053-0053-0053-000000000053','55555555-0006-0006-0006-000000000006','33333333-3333-3333-3333-333333333333',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',10,10,false,false,false,NOW()-INTERVAL '6 hours',NOW()),
('66666666-0054-0054-0054-000000000054','55555555-0006-0006-0006-000000000006','44444444-4444-4444-4444-444444444444',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',10,10,false,false,false,NOW()-INTERVAL '6 hours',NOW()),

-- Subscription 7 - Jotatsu Expired
('66666666-0061-0061-0061-000000000061','55555555-0007-0007-0007-000000000007','11111111-1111-1111-1111-111111111111',DATE_TRUNC('day',NOW()-INTERVAL '11 days'),DATE_TRUNC('day',NOW()-INTERVAL '10 days')+INTERVAL '1 day'-INTERVAL '1 second',10,7,false,true,false,NOW()-INTERVAL '2 days',NOW()-INTERVAL '1 day'),
('66666666-0062-0062-0062-000000000062','55555555-0007-0007-0007-000000000007','22222222-2222-2222-2222-222222222222',DATE_TRUNC('day',NOW()-INTERVAL '11 days'),DATE_TRUNC('day',NOW()-INTERVAL '10 days')+INTERVAL '1 day'-INTERVAL '1 second',500,300,false,true,false,NOW()-INTERVAL '2 days',NOW()-INTERVAL '1 day'),

-- Subscription 8 - Jotatsu Canceled
('66666666-0071-0071-0071-000000000071','55555555-0008-0008-0008-000000000008','11111111-1111-1111-1111-111111111111',CURRENT_DATE-INTERVAL '3 days',CURRENT_DATE-INTERVAL '2 days',10,3,false,false,true,NOW()-INTERVAL '3 days',NOW()-INTERVAL '2 days'),
('66666666-0072-0072-0072-000000000072','55555555-0008-0008-0008-000000000008','22222222-2222-2222-2222-222222222222',CURRENT_DATE-INTERVAL '3 days',CURRENT_DATE-INTERVAL '2 days',500,80,false,false,true,NOW()-INTERVAL '3 days',NOW()-INTERVAL '2 days');


-- =========================================================
-- 1. SERIES
-- Schema: material
-- =========================================================

INSERT INTO material.series
(
    id,
    slug,
    name,
    japanese_name,
    description,
    display_order,
    source_type,
    user_id,
    is_active,
    thumbnail_key,
    created_at,
    updated_at
)
VALUES
(
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    'software-development-project',
    'ソフトウェア開発プロジェクト',
    'ソフトウェア開発プロジェクト',
    '<p>サカイ教育サービス（SES）は、大学生や社会人向けの動画学習サービスをはじめとする教育サービスを提供する日本の会社です。教育分野でのIT導入が進んでいるため、扱うプロジェクトの数も増えています。</p>
     <p>DKHソリューションズはベトナムのIT企業で、ベトナム国内のみならず、日本企業をはじめとする海外企業からの受託開発プロジェクトも行っています。</p>
     <p>Đây là series về dự án phát triển phần mềm giữa SES, một công ty dịch vụ giáo dục tại Nhật Bản, và DKH Solutions, một công ty IT tại Việt Nam.</p>',
    1,
    'Curated',
    NULL,
    TRUE,
    'images/ソフトウェア開発プロジェクト.png',
    NOW(),
    NOW()
)
ON CONFLICT (id) DO NOTHING;


-- =========================================================
-- 2. SPEAKERS
-- Schema: shadowing
-- =========================================================

INSERT INTO shadowing.speakers
(
    id,
    code,
    name,
    japanese_name,
    avatar_key,
    description,
    theme_color,
    created_at,
    updated_at
)
VALUES
(
    '51000000-0000-0000-0000-000000000001',
    'KOMAKI',
    'Komaki',
    '小巻',
    NULL,
    '今回のプロジェクトのPMを務める。社内では、IT部門でIT関連のプロジェクトを管理している。| Làm PM của dự án lần này. Phụ trách quản lý các dự án liên quan đến IT của bộ phận IT trong nội bộ công ty.',
    '#3B82F6',
    NOW(),
    NOW()
),
(
    '51000000-0000-0000-0000-000000000002',
    'YAMAMOTO',
    'Yamamoto',
    '山本',
    NULL,
    'SESでIT部門に勤務している。入社3年目で、今回のプロジェクトはDKH社との初のプロジェクト。| Làm việc tại phòng IT ở công ty SES. Vào công ty được 3 năm. Dự án lần này là dự án đầu tiên làm việc với công ty DKH.',
    '#10B981',
    NOW(),
    NOW()
),
(
    '51000000-0000-0000-0000-000000000003',
    'AN',
    'An',
    'アン',
    NULL,
    'ベトナムのプロジェクトのPMを務める。情報工学を学び、日本向けシステムを開発するDKHソリューションズに8年勤務。| Làm PM của dự án phía Việt Nam. Học ngành Khoa học thông tin. Làm việc được 8 năm cho DKH Solutions, công ty chuyên phát triển các hệ thống cho Nhật Bản.',
    '#F59E0B',
    NOW(),
    NOW()
),
(
    '51000000-0000-0000-0000-000000000004',
    'LY',
    'Lý',
    'リー',
    NULL,
    '今回のプロジェクトでは、SEとしてオンサイトする。工学部出身だが、お客様とコミュニケーションが取れるように日本語も習得した。DKHに入社して10年。| Đi onsite với vai trò SE trong dự án lần này. Học khối tự nhiên và học tiếng Nhật để giao tiếp với khách hàng. Vào công ty DKH được 10 năm.',
    '#8B5CF6',
    NOW(),
    NOW()
),
(
    '51000000-0000-0000-0000-000000000005',
    'HOAN',
    'Hoàn',
    'ホアン',
    NULL,
    '今回のプロジェクトのベトナム側統括責任者。ベトナムで情報科学と日本語を学び、日本に留学。日本企業向けシステム開発部門長をしている。| Phụ trách chung phía Việt Nam trong dự án lần này. Học CNTT và tiếng Nhật tại Việt Nam, sau đó du học Nhật Bản. Hiện là trưởng bộ phận phát triển hệ thống cho doanh nghiệp Nhật.',
    '#EF4444',
    NOW(),
    NOW()
)
ON CONFLICT (id) DO NOTHING;


-- =========================================================
-- 3. VIDEOS
-- Schema: material
-- SeriesId: 2dfa8e9a-0e46-412c-9bae-1ef7d719d72d
-- ContentCategoryId: 50000000-0000-0000-0000-000000000001
-- Status: Draft
-- SourceType: Curated
-- UserId: NULL
-- =========================================================

INSERT INTO material.videos
(
    id,
    slug,
    title,
    description,
    duration,
    content_category_id,
    series_id,
    sort_order,
    source_type,
    source_url,
    status,
    learner_id,
    created_at,
    updated_at
)
VALUES
(
    '52000000-0000-0000-0000-000000000001',
    'shokai-homon-no-aisatsu',
    '第1課 初回訪問のあいさつ',
    '日本のSES社とベトナムのDKH社がプロジェクトを開始します。SES社のプロジェクト責任者が2日間ベトナムに出張し、キックオフ会議を行います。本日は1日目で、名刺交換のあとに両社があいさつします。',
    INTERVAL '00:01:53',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    1,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000002',
    'kickoff-meeting-project-no-mokuteki-to-gaiyo',
    '第2課 キックオフミーティング プロジェクトの目的と概要',
    'E-Learningプロジェクトのキックオフミーティングを行います。お客様がプロジェクトの目的をメンバーに説明します。PMのアンさんがプロジェクトの概要を紹介します。',
    INTERVAL '00:02:11',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    2,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000003',
    'kickoff-meeting-schedule-to-susumekata',
    '第3課 キックオフミーティング スケジュールと進め方',
    'E-Learningプロジェクトのキックオフミーティングが続いています。PMのアンさんがプロジェクトのマスタースケジュールと開発手法を紹介します。両社で進め方について相談します。',
    INTERVAL '00:02:19',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    3,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000004',
    'kickoff-meeting-project-taisei-no-setsumei',
    '第4課 キックオフミーティング プロジェクト体制の説明',
    'キックオフミーティングで、引き続きPMが体制について紹介します。images/体制.png',
    INTERVAL '00:03:05',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    4,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000005',
    'onsite-shonichi-no-aisatsu',
    '第5課 オンサイト初日のあいさつ',
    'DKHのSEであるリーさんがオンサイトで日本に来ました。初日は、SESのメンバーの前でリーさんが簡単に自己紹介します。リーさんがおみやげを渡した後、お客様が会社のルールについて説明します。',
    INTERVAL '00:02:33',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    5,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000006',
    'okyakusama-no-kaisha-no-rule',
    '第6課 お客様の会社のルール',
    'リーさんのオンサイト初日であいさつが終わりました。お客様が会社を案内しながら、リーさんにルールを説明してくれます。その後、リーさんは席に戻ってパソコンの設定をします。',
    INTERVAL '00:02:39',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    6,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000007',
    'yoken-no-setsumei',
    '第7課 要件の説明',
    'テレビ会議で、お客様が要件について説明します。プロジェクトチームはベトナムから、お客様とリーさんは日本から参加します。プロジェクトチームは説明を聞いて、不明点を確認します。',
    INTERVAL '00:02:29',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    7,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000008',
    'teirei-kaigi-shinchoku-no-hokoku',
    '第8課 定例会議 進捗の報告',
    'プロジェクトは実装工程に入りました。両社はオンラインミーティングで定例会議を開き、進捗を確認します。プロジェクトチームはベトナムから、お客様とリーさんは日本から参加します。',
    INTERVAL '00:03:01',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    8,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000009',
    'teirei-kaigi-kadai-no-hokoku',
    '第9課 定例会議 課題の報告',
    '日本とベトナムの間でオンラインの定例会議が続いています。進捗報告が終わり、PMがお客様にプロジェクトの課題を報告します。お客様が課題と対策の内容について確認します。',
    INTERVAL '00:02:14',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    9,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000010',
    'teirei-kaigi-qa-no-kakunin',
    '第10課 定例会議 Q&Aの確認',
    '日本とベトナムの間でオンラインの定例会議が続いています。進捗と課題の確認が終わり、少し時間が残っています。そこで、PMがお客様にQ&Aの確認をします。',
    INTERVAL '00:02:59',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    10,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000011',
    'teirei-kaigi-shinchoku-no-hokoku-2',
    '第11課 定例会議 進捗の報告',
    'プロジェクトの進捗状況を定例会議で報告します。',
    INTERVAL '00:02:30',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    11,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000012',
    'teirei-kaigi-kadai-no-hokoku-2',
    '第12課 定例会議 課題の報告',
    'プロジェクトで発生した課題を報告し、対応方法について相談します。',
    INTERVAL '00:02:45',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    12,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000013',
    'teirei-kaigi-qa-no-kakunin-2',
    '第13課 定例会議 Q&Aの確認',
    '定例会議で質問と回答の内容を確認し、認識を合わせます。',
    INTERVAL '00:02:35',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    13,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000014',
    'kaihatsu-kotei-no-shinchoku-kanri',
    '第14課 開発工程の進捗管理',
    '開発工程の進捗状況を確認し、計画との差異や今後の対応について話し合います。',
    INTERVAL '00:02:40',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    14,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
),
(
    '52000000-0000-0000-0000-000000000015',
    'project-kankyo-no-settei',
    '第15課 プロジェクト環境の設定',
    '開発に必要な環境を設定し、チームメンバーが作業を開始できる状態にします。',
    INTERVAL '00:02:50',
    '50000000-0000-0000-0000-000000000001',
    '2dfa8e9a-0e46-412c-9bae-1ef7d719d72d',
    15,
    'Curated',
    NULL,
    'Processing',
    NULL,
    NOW(),
    NOW()
)
ON CONFLICT (id) DO NOTHING;


-- =========================================================
-- 4. MEDIA ASSETS
-- Two assets per video: Video + Thumbnail
-- Schema: material
-- =========================================================

INSERT INTO material.media_assets
(
    id,
    video_id,
    asset_type,
    storage_key,
    content_type,
    file_name,
    file_size_bytes
)
VALUES
-- 第1課
(
    '53000000-0000-0000-0000-000000000001',
    '52000000-0000-0000-0000-000000000001',
    'Video',
    'media-assets/videos/第1課　初回訪問のあいさつ.mp4',
    'video/mp4',
    '第1課　初回訪問のあいさつ.mp4',
    91000000
),
(
    '53000000-0000-0000-0000-000000000002',
    '52000000-0000-0000-0000-000000000001',
    'Thumbnail',
    'images/第1課　初回訪問のあいさつ.png',
    'image/png',
    '第1課　初回訪問のあいさつ.png',
    184320
),

-- 第2課
(
    '53000000-0000-0000-0000-000000000003',
    '52000000-0000-0000-0000-000000000002',
    'Video',
    'media-assets/videos/第2課　キックオフミーティング　プロジェクトの目的と概要.mp4',
    'video/mp4',
    '第2課　キックオフミーティング　プロジェクトの目的と概要.mp4',
    102500000
),
(
    '53000000-0000-0000-0000-000000000004',
    '52000000-0000-0000-0000-000000000002',
    'Thumbnail',
    'images/第2課　キックオフミーティング　プロジェクトの目的と概要.png',
    'image/png',
    '第2課　キックオフミーティング　プロジェクトの目的と概要.png',
    276480
),

-- 第3課
(
    '53000000-0000-0000-0000-000000000005',
    '52000000-0000-0000-0000-000000000003',
    'Video',
    'media-assets/videos/第3課　キックオフミーティング　スケジュールと進め方.mp4',
    'video/mp4',
    '第3課　キックオフミーティング　スケジュールと進め方.mp4',
    108600000
),
(
    '53000000-0000-0000-0000-000000000006',
    '52000000-0000-0000-0000-000000000003',
    'Thumbnail',
    'images/第3課　キックオフミーティング　スケジュールと進め方.png',
    'image/png',
    '第3課　キックオフミーティング　スケジュールと進め方.png',
    327680
),

-- 第4課
(
    '53000000-0000-0000-0000-000000000007',
    '52000000-0000-0000-0000-000000000004',
    'Video',
    'media-assets/videos/第4課　キックオフミーティング　プロジェクト体制の説明.mp4',
    'video/mp4',
    '第4課　キックオフミーティング　プロジェクト体制の説明.mp4',
    144400000
),
(
    '53000000-0000-0000-0000-000000000008',
    '52000000-0000-0000-0000-000000000004',
    'Thumbnail',
    'images/第4課　キックオフミーティング　プロジェクト体制の説明.png',
    'image/png',
    '第4課　キックオフミーティング　プロジェクト体制の説明.png',
    409600
),

-- 第5課
(
    '53000000-0000-0000-0000-000000000009',
    '52000000-0000-0000-0000-000000000005',
    'Video',
    'media-assets/videos/第5課　オンサイト初日のあいさつ.mp4',
    'video/mp4',
    '第5課　オンサイト初日のあいさつ.mp4',
    119700000
),
(
    '53000000-0000-0000-0000-000000000010',
    '52000000-0000-0000-0000-000000000005',
    'Thumbnail',
    'images/第5課　オンサイト初日のあいさつ.png',
    'image/png',
    '第5課　オンサイト初日のあいさつ.png',
    153600
),

-- 第6課
(
    '53000000-0000-0000-0000-000000000011',
    '52000000-0000-0000-0000-000000000006',
    'Video',
    'media-assets/videos/第6課　お客様の会社のルール.mp4',
    'video/mp4',
    '第6課　お客様の会社のルール.mp4',
    123900000
),
(
    '53000000-0000-0000-0000-000000000012',
    '52000000-0000-0000-0000-000000000006',
    'Thumbnail',
    'images/第6課　お客様の会社のルール.png',
    'image/png',
    '第6課　お客様の会社のルール.png',
    245760
),

-- 第7課
(
    '53000000-0000-0000-0000-000000000013',
    '52000000-0000-0000-0000-000000000007',
    'Video',
    'media-assets/videos/第7課　要件の説明.mp4',
    'video/mp4',
    '第7課　要件の説明.mp4',
    116900000
),
(
    '53000000-0000-0000-0000-000000000014',
    '52000000-0000-0000-0000-000000000007',
    'Thumbnail',
    'images/第7課　要件の説明.png',
    'image/png',
    '第7課　要件の説明.png',
    358400
),

-- 第8課
(
    '53000000-0000-0000-0000-000000000015',
    '52000000-0000-0000-0000-000000000008',
    'Video',
    'media-assets/videos/第8課　定例会議　進捗の報告.mp4',
    'video/mp4',
    '第8課　定例会議　進捗の報告.mp4',
    141300000
),
(
    '53000000-0000-0000-0000-000000000016',
    '52000000-0000-0000-0000-000000000008',
    'Thumbnail',
    'images/第8課　定例会議　進捗の報告.png',
    'image/png',
    '第8課　定例会議　進捗の報告.png',
    204800
),

-- 第9課
(
    '53000000-0000-0000-0000-000000000017',
    '52000000-0000-0000-0000-000000000009',
    'Video',
    'media-assets/videos/第9課　定例会議　課題の報告.mp4',
    'video/mp4',
    '第9課　定例会議　課題の報告.mp4',
    104600000
),
(
    '53000000-0000-0000-0000-000000000018',
    '52000000-0000-0000-0000-000000000009',
    'Thumbnail',
    'images/第9課　定例会議　課題の報告.png',
    'image/png',
    '第9課　定例会議　課題の報告.png',
    286720
),

-- 第10課
(
    '53000000-0000-0000-0000-000000000019',
    '52000000-0000-0000-0000-000000000010',
    'Video',
    'media-assets/videos/第10課　定例会議　Q&Aの確認.mp4',
    'video/mp4',
    '第10課　定例会議　Q&Aの確認.mp4',
    140300000
),
(
    '53000000-0000-0000-0000-000000000020',
    '52000000-0000-0000-0000-000000000010',
    'Thumbnail',
    'images/第10課　定例会議　Q&Aの確認.png',
    'image/png',
    '第10課　定例会議　Q&Aの確認.png',
    174080
),

    (
    'c1000000-0000-4000-8000-000000000021',
    '52000000-0000-0000-0000-000000000011',
    'Video',
    'media-assets/videos/第11課　製品デモ.mp4',
    'video/mp4',
    '第11課　製品デモ.mp4',
    117000000
),
(
    'c1000000-0000-4000-8000-000000000022',
    '52000000-0000-0000-0000-000000000011',
    'Thumbnail',
    'images/第11課　製品デモ.png',
    'image/png',
    '第11課　製品デモ.png',
    184320
),
(
    'c1000000-0000-4000-8000-000000000023',
    '52000000-0000-0000-0000-000000000012',
    'Video',
    'media-assets/videos/第12課　課題解決の相談.mp4',
    'video/mp4',
    '第12課　課題解決の相談.mp4',
    128000000
),
(
    'c1000000-0000-4000-8000-000000000024',
    '52000000-0000-0000-0000-000000000012',
    'Thumbnail',
    'images/第12課　課題解決の相談.png',
    'image/png',
    '第12課　課題解決の相談.png',
    245760
),
(
    'c1000000-0000-4000-8000-000000000025',
    '52000000-0000-0000-0000-000000000013',
    'Video',
    'media-assets/videos/第13課　納品とリリースノート.mp4',
    'video/mp4',
    '第13課　納品とリリースノート.mp4',
    121000000
),
(
    'c1000000-0000-4000-8000-000000000026',
    '52000000-0000-0000-0000-000000000013',
    'Thumbnail',
    'images/第13課　納品とリリースノート.png',
    'image/png',
    '第13課　納品とリリースノート.png',
    327680
),
(
    'c1000000-0000-4000-8000-000000000027',
    '52000000-0000-0000-0000-000000000014',
    'Video',
    'media-assets/videos/第14課　UATサポートタスクの確認.mp4',
    'video/mp4',
    '第14課　UATサポートタスクの確認.mp4',
    132000000
),
(
    'c1000000-0000-4000-8000-000000000028',
    '52000000-0000-0000-0000-000000000014',
    'Thumbnail',
    'images/第14課　UATサポートタスクの確認.png',
    'image/png',
    '第14課　UATサポートタスクの確認.png',
    409600
),
(
    'c1000000-0000-4000-8000-000000000029',
    '52000000-0000-0000-0000-000000000015',
    'Video',
    'media-assets/videos/第15課　オンサイト終了のあいさつ.mp4',
    'video/mp4',
    '第15課　オンサイト終了のあいさつ.mp4',
    136000000
),
(
    'c1000000-0000-4000-8000-000000000030',
    '52000000-0000-0000-0000-000000000015',
    'Thumbnail',
    'images/第15課　オンサイト終了のあいさつ.png',
    'image/png',
    '第15課　オンサイト終了のあいさつ.png',
    204800
)
ON CONFLICT (id) DO NOTHING;


-- =========================================================
-- END
-- =========================================================


COMMIT;

-- =====================================================================
-- Seed data: knowledge collections / units / meanings (chỉ INSERT dữ liệu)
-- Dùng ID cố định nên chạy lại nhiều lần vẫn an toàn (ON CONFLICT DO NOTHING).
-- proficiency_level_id lấy theo ID cố định của N5..N1 (20000000-...0101 -> 0105).
-- =====================================================================

-- 1. Collections
INSERT INTO knowledge.knowledge_collections
    (id, code, name, description, collection_type, scope, is_active, created_at, updated_at)
VALUES
    ('30000000-0000-0000-0000-000000000001', 'IT_ABBREVIATIONS', 'IT・略語', 'Các từ viết tắt thường gặp trong lĩnh vực IT và phát triển phần mềm.', 'Terminology', 'Domain', TRUE, NOW(), NOW()),
    ('30000000-0000-0000-0000-000000000002', 'IT_TERMINOLOGY', 'IT・専門用語', 'Các thuật ngữ chuyên ngành IT, kiểm thử và phát triển phần mềm.', 'Terminology', 'Domain', TRUE, NOW(), NOW()),
    ('30000000-0000-0000-0000-000000000003', 'BUSINESS_JAPANESE', 'ビジネス日本語', 'Từ vựng tiếng Nhật sử dụng trong môi trường doanh nghiệp và dự án.', 'Vocabulary', 'Domain', TRUE, NOW(), NOW()),
    ('30000000-0000-0000-0000-000000000004', 'JAPANESE_HONORIFIC', '敬語', 'Các mẫu 丁寧語, 尊敬語 và 謙譲語 dùng trong giao tiếp công việc.', 'Mixed', 'General', TRUE, NOW(), NOW())
ON CONFLICT DO NOTHING;

-- 2. Knowledge units
INSERT INTO knowledge.knowledge_units
    (id, knowledge_type, knowledge_collection_id, proficiency_level_id, expression, reading, source_type, is_active, created_at, updated_at)
VALUES
    ('40000000-0000-0000-0000-000000000001', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000102', 'AI', 'エーアイ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000002', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000103', 'API', 'エーピーアイ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000003', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000103', 'AWS', 'エーダブリューエス', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000004', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000103', 'BA', 'ビーエー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000005', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000102', 'DB', 'ディービー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000006', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000102', 'GB', 'ギガバイト', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000007', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000103', 'GL', 'ジーエル', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000008', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000102', 'IT', 'アイティー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000009', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000102', 'MB', 'メガバイト', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000010', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000102', 'OS', 'オーエス', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000011', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000102', 'PC', 'ピーシー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000012', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000102', 'PDF', 'ピーディーエフ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000013', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000103', 'PM', 'ピーエム', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000014', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000103', 'POC', 'ピーオーシー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000015', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000102', 'Q&A', 'キューアンドエー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000016', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000102', 'RAM', 'ラム', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000017', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000103', 'SE', 'エスイー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000018', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000102', 'SNS', 'エスエヌエス', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000019', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000103', 'SRS', 'エスアールエス', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000020', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000103', 'UAT', 'ユーエーティー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000021', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000102', 'UI', 'ユーアイ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000022', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000103', 'UT', 'ユーティー', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000023', 'Terminology', '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000103', 'WBS', 'ダブリュービーエス', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000024', 'Terminology', '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000103', '結合テスト', 'けつごうテスト', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000025', 'Terminology', '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000103', '単体テスト', 'たんたいテスト', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000026', 'Terminology', '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000104', '要件定義', 'ようけんていぎ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000027', 'Terminology', '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000103', 'システム開発', 'システムかいはつ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000028', 'Terminology', '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000103', 'データ分析', 'データぶんせき', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000029', 'Terminology', '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000104', '人事管理', 'じんじかんり', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000030', 'Terminology', '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000103', '経理', 'けいり', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000031', 'Terminology', '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000103', '人事', 'じんじ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000032', 'Terminology', '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000104', 'ソフトウェア要求仕様書', 'ソフトウェアようきゅうしようしょ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000033', 'Terminology', '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000104', '作業分解構造', 'さぎょうぶんかいこうぞう', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000034', 'Terminology', '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000104', '概念実証', 'がいねんじっしょう', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000035', 'Terminology', '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000104', '実証実験', 'じっしょうじっけん', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000036', 'Terminology', '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000103', '確認事項', 'かくにんじこう', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000037', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', '責任者', 'せきにんしゃ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000038', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', '出張', 'しゅっちょう', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000039', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', 'キックオフ会議', 'キックオフかいぎ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000040', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', 'キックオフミーティング', 'キックオフミーティング', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000041', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', '名刺交換', 'めいしこうかん', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000042', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', '光栄', 'こうえい', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000043', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', 'システム開発', 'システムかいはつ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000044', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', '部門', 'ぶもん', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000045', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000102', '勤める', 'つとめる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000046', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', '訪問', 'ほうもん', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000047', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', '弊社', 'へいしゃ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000048', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', '対面', 'たいめん', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000049', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000102', '帰国', 'きこく', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000050', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', '要件定義', 'ようけんていぎ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000051', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', '工程', 'こうてい', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000052', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', 'オンサイト', 'オンサイト', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000053', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', '分析', 'ぶんせき', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000054', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', '参考', 'さんこう', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000055', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', '部署', 'ぶしょ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000056', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000105', '受託', 'じゅたく', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000057', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', '人材派遣', 'じんざいはけん', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000058', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', '各種', 'かくしゅ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000059', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', '人事管理', 'じんじかんり', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000060', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', '滞在', 'たいざい', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000061', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', '多忙', 'たぼう', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000062', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', 'ご訪問', 'ごほうもん', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000063', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', 'ご来社', 'ごらいしゃ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000064', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', 'ご来訪', 'ごらいほう', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000065', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', 'この度', 'このたび', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000066', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', '担当する', 'たんとうする', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000067', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', '管理する', 'かんりする', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000068', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000102', '紹介する', 'しょうかいする', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000069', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', '申す', 'もうす', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000070', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', '参る', 'まいる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000071', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', '伺う', 'うかがう', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000072', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000104', 'お手元', 'おてもと', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000073', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', 'お気軽', 'おきがる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000074', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', 'ご説明', 'ごせつめい', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000075', 'Vocabulary', '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000103', 'ご質問', 'ごしつもん', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000076', 'Grammar', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000102', '丁寧語', 'ていねいご', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000077', 'Grammar', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', '尊敬語', 'そんけいご', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000078', 'Grammar', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', '謙譲語', 'けんじょうご', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000079', 'Grammar', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', 'お～になる', 'お～になる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000080', 'Grammar', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', 'ご～になる', 'ご～になる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000081', 'Grammar', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', 'お～する', 'お～する', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000082', 'Grammar', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', 'ご～する', 'ご～する', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000083', 'Grammar', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', 'お～いたす', 'お～いたす', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000084', 'Grammar', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000101', 'です・ます体', 'です・ますたい', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000085', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000102', 'お食事', 'おしょくじ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000086', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000102', 'お時間', 'おじかん', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000087', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000102', 'ごゆっくり', 'ごゆっくり', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000088', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000105', 'ごもっとも', 'ごもっとも', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000089', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', 'いらっしゃる', 'いらっしゃる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000090', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', 'おっしゃる', 'おっしゃる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000091', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', 'なさる', 'なさる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000092', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', '召し上がる', 'めしあがる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000093', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', 'ご覧になる', 'ごらんになる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000094', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', 'お越しになる', 'おこしになる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000095', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000104', 'おいでになる', 'おいでになる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000096', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', 'お帰りになる', 'おかえりになる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000097', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000104', 'ご存知', 'ごぞんじ', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000098', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', '申す', 'もうす', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000099', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', '申し上げる', 'もうしあげる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000100', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', '拝見する', 'はいけんする', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000101', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000104', '拝聴する', 'はいちょうする', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000102', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000104', '拝読する', 'はいどくする', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000103', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', '伺う', 'うかがう', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000104', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', '参る', 'まいる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000105', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000103', 'お目にかかる', 'おめにかかる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000106', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000104', '存じ上げる', 'ぞんじあげる', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000107', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000102', 'いただく', 'いただく', 'Defined', TRUE, NOW(), NOW()),
    ('40000000-0000-0000-0000-000000000108', 'Vocabulary', '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000104', '頂戴する', 'ちょうだいする', 'Defined', TRUE, NOW(), NOW())
ON CONFLICT DO NOTHING;

-- 3. Knowledge meanings
INSERT INTO knowledge.knowledge_meanings
    (id, sort_order, meaning, japanese_meaning, description, knowledge_unit_id)
VALUES
    ('50000000-0000-0000-0000-000000000001', 1, 'Trí tuệ nhân tạo', '人間の知的な能力をコンピュータで再現する技術。人工知能。', 'Artificial Intelligence. Công nghệ mô phỏng các năng lực trí tuệ của con người. Ví dụ: AIを活用して業務を効率化する。(Tận dụng AI để làm việc hiệu quả hơn.)', '40000000-0000-0000-0000-000000000001'),
    ('50000000-0000-0000-0000-000000000002', 1, 'Giao diện lập trình ứng dụng', 'ソフトウェア同士が機能やデータをやり取りするための接続の仕組み。', 'Application Programming Interface. Giao diện cho phép các phần mềm giao tiếp với nhau. Ví dụ: APIを使って外部システムと連携する。(Dùng API để liên kết với hệ thống bên ngoài.)', '40000000-0000-0000-0000-000000000002'),
    ('50000000-0000-0000-0000-000000000003', 1, 'Dịch vụ web Amazon', 'Amazonが提供するクラウドコンピューティングサービス。', 'Amazon Web Services. Nền tảng dịch vụ điện toán đám mây. Ví dụ: システムをAWS上に構築する。(Xây dựng hệ thống trên AWS.)', '40000000-0000-0000-0000-000000000003'),
    ('50000000-0000-0000-0000-000000000004', 1, 'Người phân tích nghiệp vụ', '業務や要件を分析し、システムに必要な要求を整理する担当者。ビジネスアナリスト。', 'Business Analyst. Người phân tích yêu cầu và nghiệp vụ của hệ thống. Ví dụ: BAが顧客の要望を整理する。(BA sắp xếp lại các yêu cầu của khách hàng.)', '40000000-0000-0000-0000-000000000004'),
    ('50000000-0000-0000-0000-000000000005', 1, 'Cơ sở dữ liệu', 'データを整理して保存・管理する仕組み。データベース。', 'Database. Hệ thống lưu trữ và quản lý dữ liệu. Ví dụ: DBに顧客データを保存する。(Lưu dữ liệu khách hàng vào DB.)', '40000000-0000-0000-0000-000000000005'),
    ('50000000-0000-0000-0000-000000000006', 1, 'Gigabyte', 'データ容量を表す単位。ギガバイト。', 'Đơn vị đo dung lượng dữ liệu (1GB = 1.024MB theo cách tính nhị phân). Ví dụ: このファイルは2GBあります。(Tệp này nặng 2GB.)', '40000000-0000-0000-0000-000000000006'),
    ('50000000-0000-0000-0000-000000000007', 1, 'Trưởng nhóm', 'チームやグループをまとめるリーダー。グループリーダー。', 'Group Leader. Người dẫn dắt một nhóm nhỏ trong dự án. Ví dụ: GLがチームの進捗を確認する。(Trưởng nhóm kiểm tra tiến độ của nhóm.)', '40000000-0000-0000-0000-000000000007'),
    ('50000000-0000-0000-0000-000000000008', 1, 'Công nghệ thông tin', '情報の処理や通信に関する技術。情報技術。', 'Information Technology. Công nghệ liên quan đến xử lý và quản lý thông tin. Ví dụ: IT業界で働いています。(Tôi đang làm việc trong ngành IT.)', '40000000-0000-0000-0000-000000000008'),
    ('50000000-0000-0000-0000-000000000009', 1, 'Megabyte', 'データ容量を表す単位。メガバイト。', 'Đơn vị đo dung lượng dữ liệu, nhỏ hơn GB. Ví dụ: 画像のサイズは5MBです。(Kích thước ảnh là 5MB.)', '40000000-0000-0000-0000-000000000009'),
    ('50000000-0000-0000-0000-000000000010', 1, 'Hệ điều hành', 'コンピュータのハードウェアやソフトウェアを管理する基本ソフト。オペレーティングシステム。', 'Operating System. Phần mềm quản lý tài nguyên phần cứng và phần mềm. Ví dụ: 最新のOSにアップデートする。(Cập nhật lên hệ điều hành mới nhất.)', '40000000-0000-0000-0000-000000000010'),
    ('50000000-0000-0000-0000-000000000011', 1, 'Máy tính cá nhân', '個人が使うために作られたコンピュータ。パーソナルコンピュータ。', 'Personal Computer. Máy tính được thiết kế chủ yếu cho một người sử dụng. Ví dụ: 会議にPCを持っていく。(Mang máy tính đến cuộc họp.)', '40000000-0000-0000-0000-000000000011'),
    ('50000000-0000-0000-0000-000000000012', 1, 'Định dạng tài liệu di động', '環境に関係なく同じ見た目で文書を表示できるファイル形式。', 'Portable Document Format. Định dạng tệp dùng để trình bày tài liệu. Ví dụ: 資料をPDFで送ってください。(Hãy gửi tài liệu dưới dạng PDF.)', '40000000-0000-0000-0000-000000000012'),
    ('50000000-0000-0000-0000-000000000013', 1, 'Quản lý dự án', 'プロジェクト全体の進行・品質・予算などを管理する責任者。プロジェクトマネージャー。', 'Project Manager (người quản lý dự án) hoặc Project Management (hoạt động quản lý dự án). Ví dụ: PMがプロジェクトの進捗を管理する。(PM quản lý tiến độ dự án.)', '40000000-0000-0000-0000-000000000013'),
    ('50000000-0000-0000-0000-000000000014', 1, 'Chứng minh tính khả thi', '新しい技術やアイデアが実現可能かを確かめる試験的な取り組み。概念実証。', 'Proof of Concept. Thử nghiệm nhằm xác minh tính khả thi của một ý tưởng. Ví dụ: 本番導入の前にPOCを実施する。(Thực hiện POC trước khi triển khai chính thức.)', '40000000-0000-0000-0000-000000000014'),
    ('50000000-0000-0000-0000-000000000015', 1, 'Hỏi đáp', '質問とそれに対する回答。', 'Questions & Answers. Hình thức trao đổi câu hỏi và câu trả lời. Ví dụ: 会議の最後にQ&Aの時間があります。(Cuối buổi họp có thời gian hỏi đáp.)', '40000000-0000-0000-0000-000000000015'),
    ('50000000-0000-0000-0000-000000000016', 1, 'Bộ nhớ truy cập ngẫu nhiên', 'コンピュータが動作中にデータを一時的に記憶する装置。', 'Random-Access Memory. Bộ nhớ được sử dụng để lưu dữ liệu khi máy tính đang hoạt động. Ví dụ: RAMが足りなくて動作が遅い。(Thiếu RAM nên máy chạy chậm.)', '40000000-0000-0000-0000-000000000016'),
    ('50000000-0000-0000-0000-000000000017', 1, 'Kỹ sư hệ thống', 'システムの設計や開発を担当する技術者。システムエンジニア。', 'System Engineer. Kỹ sư tham gia thiết kế, phát triển hoặc triển khai hệ thống. Ví dụ: SEとして銀行のシステムを開発している。(Tôi đang phát triển hệ thống ngân hàng với vai trò SE.)', '40000000-0000-0000-0000-000000000017'),
    ('50000000-0000-0000-0000-000000000018', 1, 'Dịch vụ mạng xã hội', '人と人がインターネット上でつながり、交流できるサービス。', 'Social Networking Service. Dịch vụ kết nối và tương tác giữa người dùng. Ví dụ: SNSで新製品を紹介する。(Giới thiệu sản phẩm mới trên mạng xã hội.)', '40000000-0000-0000-0000-000000000018'),
    ('50000000-0000-0000-0000-000000000019', 1, 'Đặc tả yêu cầu phần mềm', 'ソフトウェアに求められる要件をまとめた文書。ソフトウェア要求仕様書。', 'Software Requirements Specification. Tài liệu mô tả các yêu cầu của phần mềm. Ví dụ: SRSに基づいて設計を行う。(Thiết kế dựa trên SRS.)', '40000000-0000-0000-0000-000000000019'),
    ('50000000-0000-0000-0000-000000000020', 1, 'Kiểm thử nghiệm thu người dùng', '利用者が、システムが要求どおりに使えるかを確認するテスト。ユーザー受け入れテスト。', 'User Acceptance Testing. Kiểm thử để xác nhận hệ thống đáp ứng yêu cầu sử dụng. Ví dụ: お客様がUATを実施します。(Khách hàng thực hiện UAT.)', '40000000-0000-0000-0000-000000000020'),
    ('50000000-0000-0000-0000-000000000021', 1, 'Giao diện người dùng', '利用者がシステムを操作するときに目にする画面や操作部分。ユーザーインターフェース。', 'User Interface. Phần giao diện cho phép người dùng tương tác với hệ thống. Ví dụ: UIを分かりやすく改善する。(Cải thiện UI cho dễ hiểu hơn.)', '40000000-0000-0000-0000-000000000021'),
    ('50000000-0000-0000-0000-000000000022', 1, 'Kiểm thử đơn vị', 'プログラムの最小単位ごとに行うテスト。単体テスト。', 'Unit Testing. Kiểm thử từng đơn vị nhỏ của phần mềm (tương đương 単体テスト). Ví dụ: コーディングの後にUTを実施する。(Thực hiện UT sau khi viết code.)', '40000000-0000-0000-0000-000000000022'),
    ('50000000-0000-0000-0000-000000000023', 1, 'Cấu trúc phân chia công việc', 'プロジェクトの作業を細かい項目に分解して整理した構成。作業分解構造。', 'Work Breakdown Structure. Cấu trúc phân rã phạm vi công việc của dự án. Ví dụ: WBSを作成して作業を洗い出す。(Lập WBS để liệt kê các đầu việc.)', '40000000-0000-0000-0000-000000000023'),
    ('50000000-0000-0000-0000-000000000024', 1, 'Kiểm thử tích hợp', '単体テストを終えたモジュールを組み合わせ、連携が正しく動作するか確認するテスト。', 'Integration Testing. Kiểm thử sự phối hợp giữa các thành phần hoặc module. Ví dụ: 単体テストが終わったら結合テストを行います。(Khi xong kiểm thử đơn vị thì sẽ làm kiểm thử tích hợp.)', '40000000-0000-0000-0000-000000000024'),
    ('50000000-0000-0000-0000-000000000025', 1, 'Kiểm thử đơn vị', 'プログラムの最小単位が単独で正しく動作するか確認するテスト。', 'Unit Testing. Kiểm thử một đơn vị phần mềm độc lập. Ví dụ: 各機能の単体テストを実施しました。(Đã thực hiện kiểm thử đơn vị cho từng chức năng.)', '40000000-0000-0000-0000-000000000025'),
    ('50000000-0000-0000-0000-000000000026', 1, 'Định nghĩa yêu cầu', 'システムに必要な機能や条件を明確にして決める工程。', 'Công đoạn làm rõ và xác định yêu cầu của hệ thống. Ví dụ: お客様と要件定義を進めています。(Đang tiến hành định nghĩa yêu cầu cùng khách hàng.)', '40000000-0000-0000-0000-000000000026'),
    ('50000000-0000-0000-0000-000000000027', 1, 'Phát triển hệ thống', 'システムを企画・設計・開発・導入する一連の作業。', 'Hoạt động phân tích, thiết kế, xây dựng và triển khai hệ thống. Ví dụ: 大手企業のシステム開発に携わっています。(Tôi tham gia phát triển hệ thống cho doanh nghiệp lớn.)', '40000000-0000-0000-0000-000000000027'),
    ('50000000-0000-0000-0000-000000000028', 1, 'Phân tích dữ liệu', 'データを調べて、役に立つ情報や傾向を見つけ出すこと。', 'Hoạt động phân tích dữ liệu nhằm tìm ra thông tin và xu hướng có ích. Ví dụ: データ分析の結果を報告します。(Tôi xin báo cáo kết quả phân tích dữ liệu.)', '40000000-0000-0000-0000-000000000028'),
    ('50000000-0000-0000-0000-000000000029', 1, 'Quản lý nhân sự', '従業員の採用・配置・評価・勤怠などを管理すること。', 'Hoạt động quản lý nhân viên và các quy trình nhân sự. Ví dụ: 人事管理システムを導入する。(Đưa vào sử dụng hệ thống quản lý nhân sự.)', '40000000-0000-0000-0000-000000000029'),
    ('50000000-0000-0000-0000-000000000030', 1, 'Kế toán', '会社のお金の出入りを記録・計算・管理する仕事。', 'Công việc liên quan đến ghi chép và quản lý các giao dịch tài chính. Ví dụ: 経理部に請求書を提出してください。(Hãy nộp hóa đơn cho phòng kế toán.)', '40000000-0000-0000-0000-000000000030'),
    ('50000000-0000-0000-0000-000000000031', 1, 'Nhân sự', '従業員の採用・配置・評価などを担当する部署や仕事。', 'Bộ phận hoặc nghiệp vụ quản lý nhân viên trong doanh nghiệp. Ví dụ: 人事部が新入社員の研修を担当します。(Phòng nhân sự phụ trách đào tạo nhân viên mới.)', '40000000-0000-0000-0000-000000000031'),
    ('50000000-0000-0000-0000-000000000032', 1, 'Đặc tả yêu cầu phần mềm', 'ソフトウェアに求められる機能や条件を記述した文書。', 'Software Requirements Specification (SRS). Tài liệu mô tả yêu cầu phần mềm. Ví dụ: ソフトウェア要求仕様書をレビューしてください。(Hãy review tài liệu đặc tả yêu cầu phần mềm.)', '40000000-0000-0000-0000-000000000032'),
    ('50000000-0000-0000-0000-000000000033', 1, 'Cấu trúc phân chia công việc', 'プロジェクトの作業を階層的に細かく分解して整理したもの。', 'Work Breakdown Structure (WBS). Cấu trúc thể hiện việc phân rã công việc của dự án thành các hạng mục nhỏ. Ví dụ: 作業分解構造に基づいてスケジュールを作成する。(Lập lịch trình dựa trên cấu trúc phân chia công việc.)', '40000000-0000-0000-0000-000000000033'),
    ('50000000-0000-0000-0000-000000000034', 1, 'Chứng minh khái niệm', '新しいアイデアや技術が実現できるかを、小規模に試して確かめること。', 'Proof of Concept. Hoạt động xác minh ý tưởng hoặc tính khả thi. Ví dụ: 新技術の概念実証を行う。(Thực hiện chứng minh khái niệm cho công nghệ mới.)', '40000000-0000-0000-0000-000000000034'),
    ('50000000-0000-0000-0000-000000000035', 1, 'Thử nghiệm chứng minh tính khả thi', '実際の環境で、技術や方法の有効性を確かめる実験。', 'Thử nghiệm thực tế nhằm xác minh một công nghệ hoặc phương pháp. Ví dụ: 来月から実証実験を開始します。(Từ tháng sau sẽ bắt đầu thử nghiệm thực tế.)', '40000000-0000-0000-0000-000000000035'),
    ('50000000-0000-0000-0000-000000000036', 1, 'Hạng mục cần xác nhận', '相手や関係者に確かめる必要のある事柄。', 'Các nội dung cần xác nhận với khách hàng hoặc các bên liên quan trong dự án. Ví dụ: 確認事項を一覧にまとめました。(Tôi đã tổng hợp các hạng mục cần xác nhận thành danh sách.)', '40000000-0000-0000-0000-000000000036'),
    ('50000000-0000-0000-0000-000000000037', 1, 'Người phụ trách', '仕事や組織に対して責任を負う人。', 'Người chịu trách nhiệm cho công việc hoặc một bộ phận. Ví dụ: プロジェクトの責任者は田中さんです。(Người phụ trách dự án là anh Tanaka.)', '40000000-0000-0000-0000-000000000037'),
    ('50000000-0000-0000-0000-000000000038', 1, 'Công tác', '仕事のために、ふだんの勤務地とは別の場所へ行くこと。', 'Đi đến địa điểm khác để thực hiện công việc. Ví dụ: 来週、大阪へ出張します。(Tuần sau tôi đi công tác Osaka.)', '40000000-0000-0000-0000-000000000038'),
    ('50000000-0000-0000-0000-000000000039', 1, 'Họp khởi động dự án', 'プロジェクトの開始時に、目的や体制を共有するために開く会議。', 'Cuộc họp mở đầu dự án. Ví dụ: 明日の午前10時からキックオフ会議があります。(10 giờ sáng mai có họp khởi động dự án.)', '40000000-0000-0000-0000-000000000039'),
    ('50000000-0000-0000-0000-000000000040', 1, 'Họp khởi động dự án', 'プロジェクトの開始時に、目的や体制を共有するために開く会議。', 'Cuộc họp mở đầu dự án (cách nói khác của キックオフ会議). Ví dụ: キックオフミーティングで体制を紹介します。(Tôi sẽ giới thiệu cơ cấu nhân sự tại buổi họp khởi động.)', '40000000-0000-0000-0000-000000000040'),
    ('50000000-0000-0000-0000-000000000041', 1, 'Trao đổi danh thiếp', '初対面のあいさつとして、お互いに名刺を渡し合うこと。', 'Trao đổi danh thiếp khi gặp gỡ đối tác. Ví dụ: 初めてお会いしたので、名刺交換をしました。(Vì lần đầu gặp nên chúng tôi đã trao đổi danh thiếp.)', '40000000-0000-0000-0000-000000000041'),
    ('50000000-0000-0000-0000-000000000042', 1, 'Vinh dự', '名誉に思うこと。ありがたく誇らしいこと。', 'Cách diễn đạt sự vinh dự hoặc hân hạnh. Ví dụ: ご一緒できて光栄です。(Thật vinh dự khi được làm việc cùng quý vị.)', '40000000-0000-0000-0000-000000000042'),
    ('50000000-0000-0000-0000-000000000043', 1, 'Phát triển hệ thống', 'システムを設計し、作り上げる仕事。', 'Hoạt động xây dựng hệ thống phần mềm. Ví dụ: 弊社はシステム開発を行っています。(Công ty chúng tôi thực hiện phát triển hệ thống.)', '40000000-0000-0000-0000-000000000043'),
    ('50000000-0000-0000-0000-000000000044', 1, 'Phòng hoặc bộ phận', '組織を仕事の内容によって分けた区分。', 'Đơn vị tổ chức trong doanh nghiệp. Ví dụ: 開発部門の山田です。(Tôi là Yamada của bộ phận phát triển.)', '40000000-0000-0000-0000-000000000044'),
    ('50000000-0000-0000-0000-000000000045', 1, 'Làm việc tại', '会社や組織に所属して仕事をする。', 'Làm việc cho một tổ chức hoặc công ty. Ví dụ: 私はIT企業に勤めています。(Tôi làm việc tại một công ty IT.)', '40000000-0000-0000-0000-000000000045'),
    ('50000000-0000-0000-0000-000000000046', 1, 'Thăm hoặc đến thăm', '仕事や用事で、人や会社を訪れること。', 'Đến một công ty hoặc địa điểm để gặp gỡ. Ví dụ: 来週、御社を訪問したいと思います。(Tuần sau tôi muốn đến thăm quý công ty.)', '40000000-0000-0000-0000-000000000046'),
    ('50000000-0000-0000-0000-000000000047', 1, 'Công ty chúng tôi', '自分の会社をへりくだって言う言葉。', 'Cách nói khiêm nhường dùng để chỉ công ty của người nói. Ví dụ: 弊社の製品をご紹介します。(Xin giới thiệu sản phẩm của công ty chúng tôi.)', '40000000-0000-0000-0000-000000000047'),
    ('50000000-0000-0000-0000-000000000048', 1, 'Gặp trực tiếp', '直接顔を合わせること。', 'Gặp mặt trực tiếp thay vì giao tiếp từ xa. Ví dụ: 今回は対面で打ち合わせをしたいです。(Lần này tôi muốn họp trực tiếp.)', '40000000-0000-0000-0000-000000000048'),
    ('50000000-0000-0000-0000-000000000049', 1, 'Về nước', '外国から自分の国に帰ること。', 'Trở về đất nước của mình. Ví dụ: 来月、一時帰国します。(Tháng sau tôi sẽ về nước tạm thời.)', '40000000-0000-0000-0000-000000000049'),
    ('50000000-0000-0000-0000-000000000050', 1, 'Định nghĩa yêu cầu', 'システムに必要な要求を整理して決める工程。', 'Công đoạn làm rõ yêu cầu trong dự án. Ví dụ: 要件定義の工程が終わりました。(Công đoạn định nghĩa yêu cầu đã kết thúc.)', '40000000-0000-0000-0000-000000000050'),
    ('50000000-0000-0000-0000-000000000051', 1, 'Công đoạn hoặc quy trình', '仕事を進める上での各段階。', 'Một giai đoạn trong quy trình thực hiện công việc. Ví dụ: 設計の工程は予定通り進んでいます。(Công đoạn thiết kế đang tiến hành đúng kế hoạch.)', '40000000-0000-0000-0000-000000000051'),
    ('50000000-0000-0000-0000-000000000052', 1, 'Onsite', '顧客先など、現場で作業を行うこと。', 'Làm việc tại địa điểm của khách hàng hoặc địa điểm triển khai. Ví dụ: 来月からオンサイトで作業します。(Từ tháng sau tôi sẽ làm việc onsite.)', '40000000-0000-0000-0000-000000000052'),
    ('50000000-0000-0000-0000-000000000053', 1, 'Phân tích', '物事を細かく分けて、その内容や原因を明らかにすること。', 'Phân tích dữ liệu, yêu cầu hoặc vấn đề. Ví dụ: 原因を分析して報告します。(Tôi sẽ phân tích nguyên nhân rồi báo cáo.)', '40000000-0000-0000-0000-000000000053'),
    ('50000000-0000-0000-0000-000000000054', 1, 'Tham khảo', '考えや判断の助けとして、他の資料や意見を見ること。', 'Dùng làm nguồn thông tin để xem xét hoặc đánh giá. Ví dụ: こちらの資料をご参考にしてください。(Xin hãy tham khảo tài liệu này.)', '40000000-0000-0000-0000-000000000054'),
    ('50000000-0000-0000-0000-000000000055', 1, 'Bộ phận', '会社などで仕事を分担する部門や課。', 'Đơn vị tổ chức trong công ty. Ví dụ: どちらの部署にお勤めですか。(Anh/chị làm ở bộ phận nào ạ?)', '40000000-0000-0000-0000-000000000055'),
    ('50000000-0000-0000-0000-000000000056', 1, 'Nhận ủy thác', '他者から依頼を受けて、仕事を引き受けること。', 'Nhận thực hiện công việc hoặc dự án theo hợp đồng. Ví dụ: 弊社はソフトウェアの受託開発を行っています。(Công ty chúng tôi nhận phát triển phần mềm theo hợp đồng.)', '40000000-0000-0000-0000-000000000056'),
    ('50000000-0000-0000-0000-000000000057', 1, 'Phái cử nhân sự', '人材を必要とする企業へ、働く人を送って働いてもらう仕組み。', 'Cung cấp hoặc điều phối nhân sự đến đơn vị khác làm việc. Ví dụ: 人材派遣の会社に登録しました。(Tôi đã đăng ký với công ty phái cử nhân sự.)', '40000000-0000-0000-0000-000000000057'),
    ('50000000-0000-0000-0000-000000000058', 1, 'Các loại', 'いろいろな種類。', 'Chỉ nhiều loại hoặc nhiều dạng khác nhau. Ví dụ: 各種資料をご用意しました。(Chúng tôi đã chuẩn bị các loại tài liệu.)', '40000000-0000-0000-0000-000000000058'),
    ('50000000-0000-0000-0000-000000000059', 1, 'Quản lý nhân sự', '従業員の採用・配置・評価などを管理すること。', 'Quản lý nhân viên và các nghiệp vụ nhân sự. Ví dụ: 人事管理の業務を担当しています。(Tôi phụ trách công việc quản lý nhân sự.)', '40000000-0000-0000-0000-000000000059'),
    ('50000000-0000-0000-0000-000000000060', 1, 'Lưu trú', '一定の期間、ある場所にとどまること。', 'Ở lại một địa điểm trong một khoảng thời gian. Ví dụ: 東京に一週間滞在する予定です。(Tôi dự định lưu lại Tokyo một tuần.)', '40000000-0000-0000-0000-000000000060'),
    ('50000000-0000-0000-0000-000000000061', 1, 'Bận rộn', '非常に忙しいこと。', 'Có nhiều công việc cần xử lý. Ví dụ: ご多忙のところ恐れ入ります。(Xin lỗi đã làm phiền khi anh/chị đang bận rộn.)', '40000000-0000-0000-0000-000000000061'),
    ('50000000-0000-0000-0000-000000000062', 1, 'Việc đến thăm', '相手が訪れることを敬って言う言葉。', 'Cách diễn đạt lịch sự về việc thăm hoặc đến thăm (thường nói về hành động của người khác). Ví dụ: ご訪問いただき、ありがとうございます。(Cảm ơn quý vị đã đến thăm.)', '40000000-0000-0000-0000-000000000062'),
    ('50000000-0000-0000-0000-000000000063', 1, 'Đến công ty chúng tôi', '相手が会社に来ることを敬って言う言葉。', 'Cách diễn đạt lịch sự khi khách đến công ty. Ví dụ: ご来社の際は、受付にお声がけください。(Khi đến công ty, xin hãy báo với quầy lễ tân.)', '40000000-0000-0000-0000-000000000063'),
    ('50000000-0000-0000-0000-000000000064', 1, 'Đến thăm', '相手が訪ねて来ることを敬って言う言葉。', 'Cách diễn đạt lịch sự về việc khách đến thăm. Ví dụ: ご来訪をお待ちしております。(Chúng tôi rất mong được đón tiếp quý vị.)', '40000000-0000-0000-0000-000000000064'),
    ('50000000-0000-0000-0000-000000000065', 1, 'Lần này hoặc dịp này', '今回。改まった場面で使う言い方。', 'Cách diễn đạt lịch sự thường dùng trong thư từ và giao tiếp công việc. Ví dụ: この度はお世話になります。(Lần này xin được nhờ anh/chị giúp đỡ.)', '40000000-0000-0000-0000-000000000065'),
    ('50000000-0000-0000-0000-000000000066', 1, 'Phụ trách', '仕事や役割を受け持つ。', 'Chịu trách nhiệm về một nhiệm vụ hoặc lĩnh vực. Ví dụ: 私がこの案件を担当します。(Tôi sẽ phụ trách dự án này.)', '40000000-0000-0000-0000-000000000066'),
    ('50000000-0000-0000-0000-000000000067', 1, 'Quản lý', '物事が適切に行われるように、取りまとめたり世話をしたりする。', 'Theo dõi và điều phối công việc hoặc tài nguyên. Ví dụ: スケジュールを管理するのが私の仕事です。(Công việc của tôi là quản lý lịch trình.)', '40000000-0000-0000-0000-000000000067'),
    ('50000000-0000-0000-0000-000000000068', 1, 'Giới thiệu', '人や物事を他の人に知らせる。', 'Giới thiệu người, bộ phận hoặc nội dung. Ví dụ: チームのメンバーを紹介します。(Tôi xin giới thiệu các thành viên trong nhóm.)', '40000000-0000-0000-0000-000000000068'),
    ('50000000-0000-0000-0000-000000000069', 1, 'Thưa hoặc tên là', '「言う」の謙譲語。', 'Động từ khiêm nhường dùng thay cho 言う trong một số ngữ cảnh. Ví dụ: 私はベトナム会社のグエンと申します。(Tôi là Nguyen của công ty Việt Nam.)', '40000000-0000-0000-0000-000000000069'),
    ('50000000-0000-0000-0000-000000000070', 1, 'Đến hoặc đi', '「行く」「来る」の謙譲語。', 'Động từ khiêm nhường thường dùng trong giao tiếp lịch sự. Ví dụ: 明日、御社へ参ります。(Ngày mai tôi sẽ đến quý công ty.)', '40000000-0000-0000-0000-000000000070'),
    ('50000000-0000-0000-0000-000000000071', 1, 'Hỏi hoặc đến thăm', '「聞く」「訪ねる」の謙譲語。', 'Động từ khiêm nhường; nghĩa cụ thể phụ thuộc ngữ cảnh. Ví dụ: 明日の午後に伺います。(Chiều mai tôi sẽ đến thăm.) / いくつか伺ってもよろしいでしょうか。(Tôi xin phép hỏi vài điều được không ạ?)', '40000000-0000-0000-0000-000000000071'),
    ('50000000-0000-0000-0000-000000000072', 1, 'Trong tay hoặc bên cạnh quý vị', '相手の手の届くあたり。', 'Cách diễn đạt lịch sự chỉ vật ở gần người nghe. Ví dụ: お手元の資料をご覧ください。(Xin quý vị xem tài liệu đang có trong tay.)', '40000000-0000-0000-0000-000000000072'),
    ('50000000-0000-0000-0000-000000000073', 1, 'Thoải mái hoặc đừng ngần ngại', '遠慮や気遣いをせず、気楽にするさま。', 'Thường dùng trong lời mời lịch sự. Ví dụ: お気軽にご質問ください。(Xin cứ thoải mái đặt câu hỏi.)', '40000000-0000-0000-0000-000000000073'),
    ('50000000-0000-0000-0000-000000000074', 1, 'Giải thích', '物事の内容をわかりやすく話すことを丁寧に言う言葉。', 'Cách diễn đạt lịch sự cho việc giải thích. Ví dụ: 担当者からご説明いたします。(Người phụ trách sẽ giải thích ạ.)', '40000000-0000-0000-0000-000000000074'),
    ('50000000-0000-0000-0000-000000000075', 1, 'Câu hỏi', '疑問点をたずねることを丁寧に言う言葉。', 'Cách diễn đạt lịch sự cho câu hỏi. Ví dụ: ご質問があればおっしゃってください。(Nếu có câu hỏi xin cứ nói ạ.)', '40000000-0000-0000-0000-000000000075'),
    ('50000000-0000-0000-0000-000000000076', 1, 'Lịch sự ngữ', '聞き手に対して丁寧に述べる言葉づかい。「です」「ます」など。', 'Cách diễn đạt lịch sự, thường dùng です và ます. Ví dụ: 明日は会議があります。(Ngày mai có cuộc họp.)', '40000000-0000-0000-0000-000000000076'),
    ('50000000-0000-0000-0000-000000000077', 1, 'Tôn kính ngữ', '相手や話題の人の行為・状態を高めて、敬意を表す言葉づかい。', 'Cách nói nâng cao hành động của người nghe hoặc người được nói đến để thể hiện sự kính trọng. Ví dụ: 社長がおっしゃいました。(Giám đốc đã nói.)', '40000000-0000-0000-0000-000000000077'),
    ('50000000-0000-0000-0000-000000000078', 1, 'Khiêm nhường ngữ', '自分や身内の行為をへりくだって言い、相手に敬意を表す言葉づかい。', 'Cách nói hạ thấp hành động của bản thân hoặc nhóm mình để thể hiện sự khiêm tốn. Ví dụ: 私が資料をお送りします。(Tôi xin gửi tài liệu.)', '40000000-0000-0000-0000-000000000078'),
    ('50000000-0000-0000-0000-000000000079', 1, 'Mẫu tôn kính ngữ', '相手の動作を高めて言う尊敬表現の型。', 'Mẫu tôn kính thường được dùng với động từ phù hợp, chẳng hạn お帰りになる. Ví dụ: 部長はもうお帰りになりました。(Trưởng phòng đã ra về rồi ạ.)', '40000000-0000-0000-0000-000000000079'),
    ('50000000-0000-0000-0000-000000000080', 1, 'Mẫu tôn kính ngữ', '相手の動作を高めて言う尊敬表現の型。漢語の動作名詞に付く。', 'Mẫu tôn kính thường được dùng với một số động từ gốc Hán, chẳng hạn ご利用になる. Ví dụ: こちらのサービスをご利用になりますか。(Quý khách có dùng dịch vụ này không ạ?)', '40000000-0000-0000-0000-000000000080'),
    ('50000000-0000-0000-0000-000000000081', 1, 'Mẫu khiêm nhường ngữ', '自分の動作をへりくだって言う謙譲表現の型。', 'Mẫu khiêm nhường, chẳng hạn お話しする hoặc お持ちする, tùy động từ. Ví dụ: 荷物をお持ちします。(Tôi xin mang hành lý giúp ạ.)', '40000000-0000-0000-0000-000000000081'),
    ('50000000-0000-0000-0000-000000000082', 1, 'Mẫu khiêm nhường ngữ', '自分の動作をへりくだって言う謙譲表現の型。漢語の動作名詞に付く。', 'Mẫu khiêm nhường, chẳng hạn ご説明する. Ví dụ: 会議の内容をご連絡します。(Tôi xin thông báo nội dung cuộc họp.)', '40000000-0000-0000-0000-000000000082'),
    ('50000000-0000-0000-0000-000000000083', 1, 'Mẫu khiêm nhường ngữ', '「お～する」よりさらに丁寧な謙譲表現の型。', 'Dạng khiêm nhường trang trọng hơn お～する, chẳng hạn お届けいたします hoặc お持ちいたします (với động từ gốc Hán dùng ご～いたす như ご説明いたします). Ví dụ: 明日、資料をお届けいたします。(Ngày mai chúng tôi xin gửi tài liệu đến ạ.)', '40000000-0000-0000-0000-000000000083'),
    ('50000000-0000-0000-0000-000000000084', 1, 'Lối nói lịch sự', '文末に「です」「ます」を用いる丁寧な文体。', 'Lối diễn đạt lịch sự dùng です và ます. Ví dụ: 私は学生です。毎日勉強します。(Tôi là sinh viên. Hằng ngày tôi học bài.)', '40000000-0000-0000-0000-000000000084'),
    ('50000000-0000-0000-0000-000000000085', 1, 'Bữa ăn', '「食事」を丁寧に言う言葉。', 'Cách diễn đạt lịch sự với tiền tố お. Ví dụ: お食事はもう済みましたか。(Anh/chị đã dùng bữa chưa ạ?)', '40000000-0000-0000-0000-000000000085'),
    ('50000000-0000-0000-0000-000000000086', 1, 'Thời gian', '「時間」を丁寧に言う言葉。', 'Cách diễn đạt lịch sự với tiền tố お. Ví dụ: お時間をいただき、ありがとうございます。(Cảm ơn anh/chị đã dành thời gian.)', '40000000-0000-0000-0000-000000000086'),
    ('50000000-0000-0000-0000-000000000087', 1, 'Cứ thong thả', '急がず、のんびりするよう相手にすすめる丁寧な言い方。', 'Cách nói lịch sự thường dùng trong lời mời hoặc lời hướng dẫn. Ví dụ: どうぞごゆっくりお過ごしください。(Xin quý khách cứ thong thả nghỉ ngơi.)', '40000000-0000-0000-0000-000000000087'),
    ('50000000-0000-0000-0000-000000000088', 1, 'Hoàn toàn có lý', '相手の言うことが道理にかなっていると認める丁寧な言い方。', 'Cách diễn đạt lịch sự để thừa nhận ý kiến của người khác. Ví dụ: おっしゃることはごもっともです。(Điều anh/chị nói hoàn toàn có lý.)', '40000000-0000-0000-0000-000000000088'),
    ('50000000-0000-0000-0000-000000000089', 1, 'Có mặt, đi hoặc đến', '「いる」「行く」「来る」の尊敬語。', 'Động từ tôn kính tương ứng với いる, 行く hoặc 来る tùy ngữ cảnh. Ví dụ: 部長は今、席にいらっしゃいません。(Hiện trưởng phòng không có ở chỗ ngồi.)', '40000000-0000-0000-0000-000000000089'),
    ('50000000-0000-0000-0000-000000000090', 1, 'Nói', '「言う」の尊敬語。', 'Động từ tôn kính tương ứng với 言う. Ví dụ: 先生がおっしゃったとおりです。(Đúng như thầy/cô đã nói.)', '40000000-0000-0000-0000-000000000090'),
    ('50000000-0000-0000-0000-000000000091', 1, 'Làm', '「する」の尊敬語。', 'Động từ tôn kính tương ứng với する. Ví dụ: 週末は何をなさいますか。(Cuối tuần anh/chị làm gì ạ?)', '40000000-0000-0000-0000-000000000091'),
    ('50000000-0000-0000-0000-000000000092', 1, 'Ăn hoặc uống', '「食べる」「飲む」の尊敬語。', 'Động từ tôn kính tương ứng với 食べる hoặc 飲む. Ví dụ: どうぞ召し上がってください。(Xin mời anh/chị dùng.)', '40000000-0000-0000-0000-000000000092'),
    ('50000000-0000-0000-0000-000000000093', 1, 'Xem hoặc nhìn', '「見る」の尊敬語。', 'Động từ tôn kính tương ứng với 見る. Ví dụ: こちらの資料をご覧になりましたか。(Anh/chị đã xem tài liệu này chưa ạ?)', '40000000-0000-0000-0000-000000000093'),
    ('50000000-0000-0000-0000-000000000094', 1, 'Đến', '「来る」「行く」の尊敬表現。', 'Cách diễn đạt tôn kính khi nói về việc người khác đến. Ví dụ: 明日は何時にお越しになりますか。(Ngày mai anh/chị sẽ đến lúc mấy giờ ạ?)', '40000000-0000-0000-0000-000000000094'),
    ('50000000-0000-0000-0000-000000000095', 1, 'Có mặt, đi hoặc đến', '「いる」「行く」「来る」の尊敬語。', 'Động từ tôn kính, nghĩa phụ thuộc ngữ cảnh. Ví dụ: 社長は会議室においでになります。(Giám đốc đang ở phòng họp.)', '40000000-0000-0000-0000-000000000095'),
    ('50000000-0000-0000-0000-000000000096', 1, 'Trở về', '「帰る」の尊敬表現。', 'Mẫu tôn kính diễn đạt hành động trở về của người khác. Ví dụ: 先生は先ほどお帰りになりました。(Thầy/cô vừa mới ra về.)', '40000000-0000-0000-0000-000000000096'),
    ('50000000-0000-0000-0000-000000000097', 1, 'Biết', '「知っている」の尊敬表現。', 'Cách diễn đạt tôn kính liên quan đến 知る. Ví dụ: この件についてご存知ですか。(Anh/chị có biết về việc này không ạ?)', '40000000-0000-0000-0000-000000000097'),
    ('50000000-0000-0000-0000-000000000098', 1, 'Nói hoặc tự giới thiệu tên', '「言う」の謙譲語。', 'Động từ khiêm nhường tương ứng với 言う. Ví dụ: 山田と申します。(Tôi tên là Yamada.)', '40000000-0000-0000-0000-000000000098'),
    ('50000000-0000-0000-0000-000000000099', 1, 'Thưa hoặc trình bày', '「言う」の謙譲語。目上の人に述べるときに使う。', 'Động từ khiêm nhường dùng trong cách diễn đạt trang trọng. Ví dụ: 心よりお礼申し上げます。(Tôi xin chân thành cảm ơn.)', '40000000-0000-0000-0000-000000000099'),
    ('50000000-0000-0000-0000-000000000100', 1, 'Xem', '「見る」の謙譲語。', 'Động từ khiêm nhường tương ứng với 見る. Ví dụ: 資料を拝見しました。(Tôi đã xem tài liệu.)', '40000000-0000-0000-0000-000000000100'),
    ('50000000-0000-0000-0000-000000000101', 1, 'Nghe', '「聞く」の謙譲語。', 'Động từ khiêm nhường tương ứng với 聞く. Ví dụ: 先生のご講演を拝聴しました。(Tôi đã được nghe bài giảng của thầy/cô.)', '40000000-0000-0000-0000-000000000101'),
    ('50000000-0000-0000-0000-000000000102', 1, 'Đọc', '「読む」の謙譲語。', 'Động từ khiêm nhường tương ứng với 読む. Ví dụ: メールを拝読いたしました。(Tôi đã đọc email của anh/chị.)', '40000000-0000-0000-0000-000000000102'),
    ('50000000-0000-0000-0000-000000000103', 1, 'Hỏi hoặc đến thăm', '「聞く」「訪ねる」「たずねる」の謙譲語。', 'Động từ khiêm nhường, nghĩa phụ thuộc ngữ cảnh. Ví dụ: 来週、御社に伺います。(Tuần sau tôi sẽ đến thăm quý công ty.) / 一つ伺ってもいいですか。(Tôi hỏi một điều được không ạ?)', '40000000-0000-0000-0000-000000000103'),
    ('50000000-0000-0000-0000-000000000104', 1, 'Đi hoặc đến', '「行く」「来る」の謙譲語。', 'Động từ khiêm nhường thường dùng trong giao tiếp trang trọng. Ví dụ: すぐにそちらへ参ります。(Tôi sẽ đến chỗ anh/chị ngay.)', '40000000-0000-0000-0000-000000000104'),
    ('50000000-0000-0000-0000-000000000105', 1, 'Gặp', '「会う」の謙譲語。', 'Động từ khiêm nhường tương ứng với 会う. Ví dụ: 今日はお目にかかれて嬉しいです。(Hôm nay được gặp anh/chị tôi rất vui.)', '40000000-0000-0000-0000-000000000105'),
    ('50000000-0000-0000-0000-000000000106', 1, 'Biết một người', '「知る」の謙譲語。人を知っている場合に使う。', 'Cách nói khiêm nhường liên quan đến 知る, dùng khi biết một người. Ví dụ: 田中様のことは存じ上げております。(Tôi có biết ngài Tanaka.)', '40000000-0000-0000-0000-000000000106'),
    ('50000000-0000-0000-0000-000000000107', 1, 'Ăn, uống hoặc nhận', '「もらう」「食べる」「飲む」の謙譲語。', 'Động từ khiêm nhường được dùng trong nhiều ngữ cảnh. Ví dụ: お土産をいただきました。(Tôi đã nhận được quà.) / 遠慮なくいただきます。(Tôi xin phép dùng ạ.)', '40000000-0000-0000-0000-000000000107'),
    ('50000000-0000-0000-0000-000000000108', 1, 'Nhận', '「もらう」の謙譲語。「食べる」「飲む」にも使う。', 'Động từ khiêm nhường, thường dùng trong giao tiếp trang trọng. Ví dụ: ご連絡を頂戴し、ありがとうございます。(Cảm ơn anh/chị đã liên hệ.)', '40000000-0000-0000-0000-000000000108')
ON CONFLICT DO NOTHING;

-- ====================================================================================================
-- Rakushu SRS & Quiz Flow Seed Script

-- Dedicated SRS Learner Account
INSERT INTO identity.users (id, full_name, email, password_hash, role_id, status, created_at, updated_at) VALUES ('c1111111-1111-1111-1111-111111111111', 'SRS Dedicated Learner', 'srs.learner@rakushu.com', '52EEF92085E496C50A456A45AB6AF67E536D4A3D8F85CBDD725D76EB287AD343:BB0C48EDEE22C7C5D20DC5CAED72C2DC:100000:SHA512', '33333333-3333-3333-3333-333333333333', 'Active', NOW(), NOW()) ON CONFLICT (id) DO NOTHING;
INSERT INTO identity.profiles (id, user_id, avatar_key, level_id, daily_learning_minutes, session_duration_minutes, created_at, updated_at) VALUES ('61111111-1111-1111-1111-111111111111', 'c1111111-1111-1111-1111-111111111111', NULL, '20000000-0000-0000-0000-000000000101', 20, 10, NOW(), NOW()) ON CONFLICT (id) DO NOTHING;

-- ----------------------------------------------------------------------------------------------------
-- 1. QUESTION TYPES
-- ----------------------------------------------------------------------------------------------------
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


-- ----------------------------------------------------------------------------------------------------
-- 2. BANK QUESTIONS (20 câu hỏi tương ứng 20 Knowledge Meanings)
-- ----------------------------------------------------------------------------------------------------
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
    -- 01. AI (人工知能)
    (
        'f0000000-0000-0000-0000-000000000001',
        '50000000-0000-0000-0000-000000000001',
        'e0000000-0000-0000-0000-000000000001',
        '「AI」の意味として最も適切なものを選択してください。',
        NULL,
        '[{"id":"A","text":"Trí tuệ nhân tạo"},{"id":"B","text":"Cơ sở dữ liệu"},{"id":"C","text":"Giao diện lập trình"},{"id":"D","text":"Hệ điều hành"}]'::jsonb,
        'A',
        'AI (Artificial Intelligence) nghĩa là Trí tuệ nhân tạo (人工知能).',
        'Medium',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 02. API
    (
        'f0000000-0000-0000-0000-000000000002',
        '50000000-0000-0000-0000-000000000002',
        'e0000000-0000-0000-0000-000000000001',
        '「API」の役割として正しい説明を選んでください。',
        NULL,
        '[{"id":"A","text":"Giao diện kết nối và trao đổi dữ liệu giữa các phần mềm"},{"id":"B","text":"Thiết bị lưu trữ dữ liệu tạm thời"},{"id":"C","text":"Bộ vi xử lý trung tâm của máy chủ"},{"id":"D","text":"Quy trình kiểm thử chấp nhận người dùng"}]'::jsonb,
        'A',
        'API (Application Programming Interface) là giao diện cho phép các phần mềm giao tiếp với nhau.',
        'Medium',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 03. AWS
    (
        'f0000000-0000-0000-0000-000000000003',
        '50000000-0000-0000-0000-000000000003',
        'e0000000-0000-0000-0000-000000000001',
        '「AWS」は何の略称ですか？',
        NULL,
        '[{"id":"A","text":"Amazon Web Services - Dịch vụ điện toán đám mây của Amazon"},{"id":"B","text":"Advanced Web Security - Hệ thống bảo mật nâng cao"},{"id":"C","text":"Application Workflow Standard - Chuẩn quy trình ứng dụng"},{"id":"D","text":"Automated Web Server - Máy chủ web tự động"}]'::jsonb,
        'A',
        'AWS là viết tắt của Amazon Web Services.',
        'Medium',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 04. BA
    (
        'f0000000-0000-0000-0000-000000000004',
        '50000000-0000-0000-0000-000000000001',
        'e0000000-0000-0000-0000-000000000001',
        '「BA」が担当する主な業務は何ですか？',
        NULL,
        '[{"id":"A","text":"Phân tích nghiệp vụ và làm rõ yêu cầu khách hàng"},{"id":"B","text":"Viết mã nguồn backend"},{"id":"C","text":"Quản trị hệ thống máy chủ mạng"},{"id":"D","text":"Kiểm thử tự động hiệu năng hệ thống"}]'::jsonb,
        'A',
        'BA (Business Analyst) phụ trách phân tích nghiệp vụ và yêu cầu hệ thống.',
        'Medium',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 05. DB
    (
        'f0000000-0000-0000-0000-000000000005',
        '50000000-0000-0000-0000-000000000005',
        'e0000000-0000-0000-0000-000000000001',
        '「DB」の日本語の意味として正しいものはどれですか？',
        NULL,
        '[{"id":"A","text":"データベース (Cơ sở dữ liệu)"},{"id":"B","text":"データブロック (Khối dữ liệu)"},{"id":"C","text":"デジタルビジネス (Kinh doanh số)"},{"id":"D","text":"ドメインブランチ (Nhánh tên miền)"}]'::jsonb,
        'A',
        'DB là Database (データベース) - Hệ thống lưu trữ và quản lý dữ liệu.',
        'Easy',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 06. IT
    (
        'f0000000-0000-0000-0000-000000000006',
        '50000000-0000-0000-0000-000000000008',
        'e0000000-0000-0000-0000-000000000001',
        '「IT」は何の略称ですか？',
        NULL,
        '[{"id":"A","text":"Information Technology (Công nghệ thông tin)"},{"id":"B","text":"Internet Terminal (Thiết bị đầu cuối internet)"},{"id":"C","text":"Integrated Testing (Kiểm thử tích hợp)"},{"id":"D","text":"Internal Tool (Công cụ nội bộ)"}]'::jsonb,
        'A',
        'IT là viết tắt của Information Technology (情報技術).',
        'Easy',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 07. OS
    (
        'f0000000-0000-0000-0000-000000000007',
        '50000000-0000-0000-0000-000000000010',
        'e0000000-0000-0000-0000-000000000001',
        '「OS」に該当するものとして正しい組み合わせを選んでください。',
        NULL,
        '[{"id":"A","text":"Windows, macOS, Linux"},{"id":"B","text":"Word, Excel, PowerPoint"},{"id":"C","text":"PostgreSQL, MySQL, Oracle"},{"id":"D","text":"Chrome, Firefox, Safari"}]'::jsonb,
        'A',
        'OS (Operating System) là hệ điều hành quản lý phần cứng và ứng dụng như Windows, macOS, Linux.',
        'Medium',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 08. PM
    (
        'f0000000-0000-0000-0000-000000000008',
        '50000000-0000-0000-0000-000000000013',
        'e0000000-0000-0000-0000-000000000001',
        '「PM」の主な責任は何ですか？',
        NULL,
        '[{"id":"A","text":"Quản lý tiến độ, chất lượng và nguồn lực của dự án"},{"id":"B","text":"Trực tiếp sửa lỗi mã nguồn của lập trình viên"},{"id":"C","text":"Thiết kế giao diện mỹ thuật cho trang web"},{"id":"D","text":"Quản lý hợp đồng bảo hiểm nhân sự"}]'::jsonb,
        'A',
        'PM (Project Manager) là người quản lý dự án (プロジェクトマネージャー).',
        'Medium',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 09. Q&A
    (
        'f0000000-0000-0000-0000-000000000009',
        '50000000-0000-0000-0000-000000000015',
        'e0000000-0000-0000-0000-000000000001',
        '「Q&A」の一般的な日本語表現を選んでください。',
        NULL,
        '[{"id":"A","text":"質疑応答 (Hỏi đáp)"},{"id":"B","text":"要求仕様 (Yêu cầu đặc tả)"},{"id":"C","text":"品質保証 (Bảo đảm chất lượng)"},{"id":"D","text":"進捗報告 (Báo cáo tiến độ)"}]'::jsonb,
        'A',
        'Q&A (Questions and Answers) nghĩa là hỏi đáp hoặc chất vấn phản hồi.',
        'Easy',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 10. SE
    (
        'f0000000-0000-0000-0000-000000000010',
        '50000000-0000-0000-0000-000000000017',
        'e0000000-0000-0000-0000-000000000001',
        '「SE」の職種名として正しいものを選んでください。',
        NULL,
        '[{"id":"A","text":"Kỹ sư hệ thống (System Engineer)"},{"id":"B","text":"Kỹ sư bảo mật (Security Engineer)"},{"id":"C","text":"Kỹ sư phần mềm (Software Engineer)"},{"id":"D","text":"Kỹ sư hỗ trợ (Support Engineer)"}]'::jsonb,
        'A',
        'Tại Nhật, SE thường chỉ System Engineer (システムエンジニア) - người phụ trách thiết kế và phát triển hệ thống.',
        'Medium',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 11. UI
    (
        'f0000000-0000-0000-0000-000000000011',
        '50000000-0000-0000-0000-000000000021',
        'e0000000-0000-0000-0000-000000000001',
        '「UI」の意味として最も適切なものはどれですか？',
        NULL,
        '[{"id":"A","text":"Giao diện người dùng (User Interface)"},{"id":"B","text":"Trải nghiệm người dùng (User Experience)"},{"id":"C","text":"Mã nhận diện tài khoản (User Identifier)"},{"id":"D","text":"Quyền truy cập cá nhân (User Isolation)"}]'::jsonb,
        'A',
        'UI là User Interface (ユーザーインターフェース) - giao diện người dùng nhìn và tương tác.',
        'Easy',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 12. UT
    (
        'f0000000-0000-0000-0000-000000000012',
        '50000000-0000-0000-0000-000000000022',
        'e0000000-0000-0000-0000-000000000001',
        '「UT」の日本語のテスト工程名を選んでください。',
        NULL,
        '[{"id":"A","text":"単体テスト (Unit Testing)"},{"id":"B","text":"結合テスト (Integration Testing)"},{"id":"C","text":"総合テスト (System Testing)"},{"id":"D","text":"受入テスト (Acceptance Testing)"}]'::jsonb,
        'A',
        'UT là Unit Testing tương đương với 単体テスト trong quy trình phát triển phần mềm.',
        'Medium',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 13. 結合テスト
    (
        'f0000000-0000-0000-0000-000000000013',
        '50000000-0000-0000-0000-000000000024',
        'e0000000-0000-0000-0000-000000000002',
        '「結合テスト」の正しい読み方を選択してください。',
        NULL,
        '[{"id":"A","text":"けつごうテスト"},{"id":"B","text":"ゆいごうテスト"},{"id":"C","text":"けつあいテスト"},{"id":"D","text":"ゆうごうテスト"}]'::jsonb,
        'A',
        '結合テスト được đọc là けつごうテスト, nghĩa là Kiểm thử tích hợp (Integration Test).',
        'Medium',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 14. 単体テスト
    (
        'f0000000-0000-0000-0000-000000000014',
        '50000000-0000-0000-0000-000000000025',
        'e0000000-0000-0000-0000-000000000002',
        '「単体テスト」の読み方を選んでください。',
        NULL,
        '[{"id":"A","text":"たんたいテスト"},{"id":"B","text":"たんたい試験"},{"id":"C","text":"ひとりたいテスト"},{"id":"D","text":"たんていテスト"}]'::jsonb,
        'A',
        '単体テスト được đọc là たんたいテスト (Kiểm thử đơn vị).',
        'Easy',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 15. 要件定義
    (
        'f0000000-0000-0000-0000-000000000015',
        '50000000-0000-0000-0000-000000000026',
        'e0000000-0000-0000-0000-000000000001',
        '「要件定義」の説明として正しいものはどれですか？',
        NULL,
        '[{"id":"A","text":"Công đoạn làm rõ và xác định các yêu cầu của hệ thống"},{"id":"B","text":"Công đoạn cài đặt máy chủ và cấu hình tường lửa"},{"id":"C","text":"Công đoạn viết tài liệu hướng dẫn người dùng cuối"},{"id":"D","text":"Công đoạn thanh lý hợp đồng và bàn giao mã nguồn"}]'::jsonb,
        'A',
        '要件定義 (ようけんていぎ) là công đoạn định nghĩa và làm rõ yêu cầu.',
        'Medium',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 16. システム開発
    (
        'f0000000-0000-0000-0000-000000000016',
        '50000000-0000-0000-0000-000000000027',
        'e0000000-0000-0000-0000-000000000001',
        '「システム開発」の意味を選んでください。',
        NULL,
        '[{"id":"A","text":"Phát triển hệ thống"},{"id":"B","text":"Bảo trì máy in"},{"id":"C","text":"Đào tạo học viên"},{"id":"D","text":"Khảo sát thị trường"}]'::jsonb,
        'A',
        'システム開発 (システムかいはつ) nghĩa là phát triển hệ thống phần mềm.',
        'Easy',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 17. データ分析
    (
        'f0000000-0000-0000-0000-000000000017',
        '50000000-0000-0000-0000-000000000028',
        'e0000000-0000-0000-0000-000000000001',
        '「データ分析」の正しい意味はどれですか？',
        NULL,
        '[{"id":"A","text":"Phân tích dữ liệu"},{"id":"B","text":"Sao lưu dữ liệu"},{"id":"C","text":"Nhập liệu bảng tính"},{"id":"D","text":"Xóa dữ liệu trùng lặp"}]'::jsonb,
        'A',
        'データ分析 (データぶんせき) nghĩa là phân tích dữ liệu nhằm tìm ra thông tin hữu ích.',
        'Easy',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 18. 出張
    (
        'f0000000-0000-0000-0000-000000000018',
        '50000000-0000-0000-0000-000000000038',
        'e0000000-0000-0000-0000-000000000002',
        '「出張」の読み方と意味として正しいものを選んでください。',
        NULL,
        '[{"id":"A","text":"しゅっちょう (Đi công tác)"},{"id":"B","text":"でばり (Mở rộng phạm vi)"},{"id":"C","text":"しゅつば (Ra ứng cử)"},{"id":"D","text":"でちょう (Sổ theo dõi)"}]'::jsonb,
        'A',
        '出張 (しゅっちょう) nghĩa là đi công tác vì công việc.',
        'Easy',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 19. 名刺交換
    (
        'f0000000-0000-0000-0000-000000000019',
        '50000000-0000-0000-0000-000000000041',
        'e0000000-0000-0000-0000-000000000001',
        'ビジネスシーンにおける「名刺交換」の意味を選んでください。',
        NULL,
        '[{"id":"A","text":"Trao đổi danh thiếp khi lần đầu gặp gỡ"},{"id":"B","text":"Ký hợp đồng dịch vụ"},{"id":"C","text":"Bàn giao con dấu công ty"},{"id":"D","text":"Thanh toán tiền công tác phí"}]'::jsonb,
        'A',
        '名刺交換 (めいしこうかん) là nét văn hóa trao đổi danh thiếp trong kinh doanh Nhật Bản.',
        'Medium',
        TRUE,
        NOW(),
        NOW()
    ),
    -- 20. 弊社
    (
        'f0000000-0000-0000-0000-000000000020',
        '50000000-0000-0000-0000-000000000047',
        'e0000000-0000-0000-0000-000000000001',
        '「弊社」の読み方とビジネスでの使い方の説明を選んでください。',
        NULL,
        '[{"id":"A","text":"へいしゃ - Từ khiêm nhường để chỉ công ty của chính mình"},{"id":"B","text":"ごしゃ - Từ tôn kính để chỉ công ty đối tác"},{"id":"C","text":"へいしゃ - Từ chỉ công ty đã phá sản"},{"id":"D","text":"きしゃ - Từ dùng trong văn viết gửi thư cho đối tác"}]'::jsonb,
        'A',
        '弊社 (へいしゃ) là từ khiêm nhường (謙譲語) người nói dùng để gọi công ty mình trước đối tác.',
        'Medium',
        TRUE,
        NOW(),
        NOW()
    )
ON CONFLICT (id) DO NOTHING;


-- ----------------------------------------------------------------------------------------------------
-- 3. SAVED ITEMS (20 từ vựng được lưu vào sổ tay của Learner One)
-- Target User: 'c1111111-1111-1111-1111-111111111111'
-- ----------------------------------------------------------------------------------------------------
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


-- ----------------------------------------------------------------------------------------------------
-- 4. SRS SCHEDULES (Đa dạng 6 trạng thái: Due_today, Overdue, Learning, Review, Suspended, Ignored)
-- ----------------------------------------------------------------------------------------------------
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


-- ----------------------------------------------------------------------------------------------------
-- 5. SAMPLE QUIZ, QUESTIONS & COMPLETED ATTEMPT (Sẵn sàng test GET result & POST retry)
-- Quiz gồm 3 câu: Item 1 (AI), Item 2 (API), Item 3 (AWS)
-- ----------------------------------------------------------------------------------------------------
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


-- ----------------------------------------------------------------------------------------------------
-- 6. SRS ITEM DETAILS (Lịch sử ôn tập chi tiết theo thuật toán SM-2)
-- ----------------------------------------------------------------------------------------------------
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
    -- Schedule 01: Item 1 (AI) - Review 1 (từ quiz attempt mẫu ở trên)
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

    -- Schedule 03: Item 3 (AWS) - Trả lời sai trong attempt mẫu (Quality 2)
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

    -- Schedule 05: Item 5 (DB) - Overdue, 1 review trước đó
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

    -- Schedule 06: Item 6 (IT) - Overdue, 1 review trước đó
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

    -- Schedule 07: Item 7 (OS) - Overdue, 2 reviews trước đó
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

    -- Schedule 12: Item 12 (UT) - Learning, 2 reviews (từng sai ở review 1)
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
