BEGIN;
-- ====================================================================================================
-- Rakushu Linguistic Metadata Seed Script (PostgreSQL)
-- Tables:
--   1. universal_part_of_speeches (17 UPOS tags)
--   2. japanese_part_of_speeches  (UniDic/Sudachi XPOS tags)
--   3. dependency_relationships   (29 Universal Dependency tags)
-- Columns: id, code, name (tiếng Việt), japanese_name (tiếng Nhật), description
-- Script có thể chạy lại nhiều lần (idempotent) nhờ ON CONFLICT (code).
-- ====================================================================================================
-- ----------------------------------------------------------------------------------------------------
-- 1. UNIVERSAL PART OF SPEECH (UPOS - 17 tags)
-- ----------------------------------------------------------------------------------------------------
INSERT INTO universal_part_of_speeches (id, code, name, japanese_name, description)
VALUES
    (gen_random_uuid(), 'ADJ', 'Tính từ', '形容詞', 'Từ mô tả thuộc tính, tính chất hoặc trạng thái của danh từ (ví dụ: big, old, green)'),
    (gen_random_uuid(), 'ADP', 'Giới từ / Trợ từ quan hệ', '接置詞', 'Giới từ hoặc hậu từ liên kết với danh từ/đại từ để chỉ quan hệ ngữ pháp (ví dụ: in, to, of)'),
    (gen_random_uuid(), 'ADV', 'Phó từ / Trạng từ', '副詞', 'Từ bổ nghĩa cho động từ, tính từ, phó từ khác hoặc toàn bộ mệnh đề (ví dụ: very, well, slowly)'),
    (gen_random_uuid(), 'AUX', 'Trợ động từ', '助動詞', 'Từ hỗ trợ động từ chính biểu thị thời, thể, thể thức hoặc thức giả định (ví dụ: is, has, will, can)'),
    (gen_random_uuid(), 'CCONJ', 'Liên từ kết hợp', '等位接続詞', 'Liên từ liên kết các từ, cụm từ hoặc mệnh đề đẳng lập ngang hàng (ví dụ: and, or, but)'),
    (gen_random_uuid(), 'DET', 'Từ chỉ định / Hạn định từ', '限定詞', 'Từ đứng trước danh từ để xác định hoặc hạn định đối tượng tham chiếu (ví dụ: a, the, this, that)'),
    (gen_random_uuid(), 'INTJ', 'Thán từ', '感動詞', 'Từ biểu lộ cảm xúc trực tiếp hoặc tiếng hô gọi, chào đón, hồi đáp (ví dụ: oh, wow, hey, yes)'),
    (gen_random_uuid(), 'NOUN', 'Danh từ', '名詞', 'Từ chỉ người, sự vật, con vật, địa điểm, hiện tượng hoặc khái niệm trừu tượng (ví dụ: water, cat, teacher)'),
    (gen_random_uuid(), 'NUM', 'Số từ', '数詞', 'Chữ số hoặc từ ngữ chỉ số lượng, số thứ tự (ví dụ: 1, 2, first, second)'),
    (gen_random_uuid(), 'PART', 'Tiểu từ / Trợ từ', '助詞', 'Hư từ ngữ pháp thực hiện chức năng ngữ pháp đặc thù mà không xếp vào từ loại khác (ví dụ: not, ''s)'),
    (gen_random_uuid(), 'PRON', 'Đại từ', '代名詞', 'Từ dùng để xưng hô hoặc thay thế cho danh từ/cụm danh từ (ví dụ: I, you, he, she, they)'),
    (gen_random_uuid(), 'PROPN', 'Danh từ riêng', '固有名詞', 'Tên riêng chỉ người, địa danh, tổ chức, nhãn hiệu cụ thể (ví dụ: Tokyo, Vietnam, Google)'),
    (gen_random_uuid(), 'PUNCT', 'Dấu câu', '句読点', 'Ký hiệu ngắt nghỉ hoặc kết thúc phân đoạn văn bản (ví dụ: ., ,, ?, !, :)'),
    (gen_random_uuid(), 'SCONJ', 'Liên từ phụ thuộc', '従属接続詞', 'Liên từ nối mệnh đề phụ với mệnh đề chính (ví dụ: because, although, if, since)'),
    (gen_random_uuid(), 'SYM', 'Ký hiệu', '記号', 'Ký hiệu đặc biệt, toán học, đơn vị tiền tệ hoặc biểu tượng (ví dụ: $, %, +, =, &)'),
    (gen_random_uuid(), 'VERB', 'Động từ', '動詞', 'Từ biểu thị hành động, trạng thái tồn tại hoặc biến đổi (ví dụ: go, eat, become, run)'),
    (gen_random_uuid(), 'X', 'Khác / Không xác định', 'その他', 'Từ loại ngoại ngữ chưa phân loại, từ viết tắt hoặc dị thể ngữ pháp');

-- ----------------------------------------------------------------------------------------------------
-- 2. JAPANESE PART OF SPEECH (JPOS / UniDic XPOS)
-- ----------------------------------------------------------------------------------------------------
INSERT INTO japanese_part_of_speeches (id, code, name, japanese_name, description)
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
-- 3. DEPENDENCY RELATIONSHIPS (Universal Dependencies - 29 tags)
-- ----------------------------------------------------------------------------------------------------
INSERT INTO dependency_relationships (id, code, name, japanese_name, description)
VALUES
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
INSERT INTO proficiency_levels (
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
INSERT INTO content_categories (
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

INSERT INTO profiles (
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
INSERT INTO interests (
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

INSERT INTO features
(id, code, name, description, is_active, created_at, updated_at)
VALUES
('11111111-1111-1111-1111-111111111111','VIDEO','Video','Create and learn from Japanese learning videos with interactive subtitle.',true,NOW(),NOW()),
('22222222-2222-2222-2222-222222222222','CHAT','AI Chat','Chat with AI for Japanese learning assistance.',true,NOW(),NOW()),
('33333333-3333-3333-3333-333333333333','QUIZ','Quiz','Practice Japanese through SRS quizzes.',true,NOW(),NOW()),
('44444444-4444-4444-4444-444444444444','CHARACTER','Character','Practice speaking Japanese by imitating characters.',true,NOW(),NOW());

-- =========================================================
-- PLANS
-- =========================================================

INSERT INTO plans
(id, code, name, japanese_name, description, price, currency, billing_cycle, is_active, created_at, updated_at)
VALUES
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa','HAJIME','Hajime','初め','A free plan for learners who are just getting started.',0.00,'VND','Weekly',true,NOW(),NOW()),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb','MANABU','Manabu','学ぶ','A learning plan for learners who want to practice Japanese regularly.',99000.00,'VND','Monthly',true,NOW(),NOW()),
('cccccccc-cccc-cccc-cccc-cccccccccccc','JOTATSU','Jōtatsu','上達','An advanced plan for serious Japanese learners.',199000.00,'VND','Monthly',true,NOW(),NOW());

-- =========================================================
-- ENTITLEMENTS
-- =========================================================

INSERT INTO entitlements
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

INSERT INTO payments
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

INSERT INTO transactions
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

INSERT INTO subscriptions
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

INSERT INTO subscription_usages
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
-- END
-- =========================================================


COMMIT;