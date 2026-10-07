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
(id, subscription_id, feature_id, period_start, period_end, used_value, is_over_limit, is_expired, is_canceled, created_at, updated_at)
VALUES
-- Subscription 1 - Hajime Active: 1/3, 10/30, 2/3, 1/1
('66666666-0001-0001-0001-000000000001','55555555-0001-0001-0001-000000000001','11111111-1111-1111-1111-111111111111',NOW()-INTERVAL '5 days',NOW()-INTERVAL '5 days'+INTERVAL '7 days',1,false,false,false,NOW()-INTERVAL '5 days',NOW()-INTERVAL '1 day'),
('66666666-0002-0002-0002-000000000002','55555555-0001-0001-0001-000000000001','22222222-2222-2222-2222-222222222222',NOW()-INTERVAL '5 days',NOW()-INTERVAL '5 days'+INTERVAL '7 days',10,false,false,false,NOW()-INTERVAL '5 days',NOW()-INTERVAL '1 day'),
('66666666-0003-0003-0003-000000000003','55555555-0001-0001-0001-000000000001','33333333-3333-3333-3333-333333333333',NOW()-INTERVAL '5 days',NOW()-INTERVAL '5 days'+INTERVAL '7 days',2,false,false,false,NOW()-INTERVAL '5 days',NOW()-INTERVAL '1 day'),
('66666666-0004-0004-0004-000000000004','55555555-0001-0001-0001-000000000001','44444444-4444-4444-4444-444444444444',NOW()-INTERVAL '5 days',NOW()-INTERVAL '5 days'+INTERVAL '7 days',1,false,false,false,NOW()-INTERVAL '5 days',NOW()-INTERVAL '1 day'),

-- Subscription 2 - Hajime Active: unused
('66666666-0011-0011-0011-000000000011','55555555-0002-0002-0002-000000000002','11111111-1111-1111-1111-111111111111',NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'+INTERVAL '7 days',0,false,false,false,NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'),
('66666666-0012-0012-0012-000000000012','55555555-0002-0002-0002-000000000002','22222222-2222-2222-2222-222222222222',NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'+INTERVAL '7 days',0,false,false,false,NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'),
('66666666-0013-0013-0013-000000000013','55555555-0002-0002-0002-000000000002','33333333-3333-3333-3333-333333333333',NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'+INTERVAL '7 days',0,false,false,false,NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'),
('66666666-0014-0014-0014-000000000014','55555555-0002-0002-0002-000000000002','44444444-4444-4444-4444-444444444444',NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'+INTERVAL '7 days',0,false,false,false,NOW()-INTERVAL '2 days',NOW()-INTERVAL '2 days'),

-- Subscription 3 - Manabu Active: 4/5, 70/100, 5/5, 3/5
('66666666-0021-0021-0021-000000000021','55555555-0003-0003-0003-000000000003','11111111-1111-1111-1111-111111111111',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',4,false,false,false,NOW()-INTERVAL '3 hours',NOW()-INTERVAL '30 minutes'),
('66666666-0022-0022-0022-000000000022','55555555-0003-0003-0003-000000000003','22222222-2222-2222-2222-222222222222',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',70,false,false,false,NOW()-INTERVAL '3 hours',NOW()-INTERVAL '30 minutes'),
('66666666-0023-0023-0023-000000000023','55555555-0003-0003-0003-000000000003','33333333-3333-3333-3333-333333333333',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',5,false,false,false,NOW()-INTERVAL '3 hours',NOW()-INTERVAL '30 minutes'),
('66666666-0024-0024-0024-000000000024','55555555-0003-0003-0003-000000000003','44444444-4444-4444-4444-444444444444',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',3,false,false,false,NOW()-INTERVAL '3 hours',NOW()-INTERVAL '30 minutes'),

-- Subscription 4 - Manabu Active: over limit
('66666666-0031-0031-0031-000000000031','55555555-0004-0004-0004-000000000004','11111111-1111-1111-1111-111111111111',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',6,true,false,false,NOW()-INTERVAL '2 hours',NOW()),
('66666666-0032-0032-0032-000000000032','55555555-0004-0004-0004-000000000004','22222222-2222-2222-2222-222222222222',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',101,true,false,false,NOW()-INTERVAL '2 hours',NOW()),
('66666666-0033-0033-0033-000000000033','55555555-0004-0004-0004-000000000004','33333333-3333-3333-3333-333333333333',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',4,false,false,false,NOW()-INTERVAL '2 hours',NOW()),
('66666666-0034-0034-0034-000000000034','55555555-0004-0004-0004-000000000004','44444444-4444-4444-4444-444444444444',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',5,false,false,false,NOW()-INTERVAL '2 hours',NOW()),

-- Subscription 5 - Jotatsu Active
('66666666-0041-0041-0041-000000000041','55555555-0005-0005-0005-000000000005','11111111-1111-1111-1111-111111111111',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',8,false,false,false,NOW()-INTERVAL '5 hours',NOW()-INTERVAL '1 hour'),
('66666666-0042-0042-0042-000000000042','55555555-0005-0005-0005-000000000005','22222222-2222-2222-2222-222222222222',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',450,false,false,false,NOW()-INTERVAL '5 hours',NOW()-INTERVAL '1 hour'),
('66666666-0043-0043-0043-000000000043','55555555-0005-0005-0005-000000000005','33333333-3333-3333-3333-333333333333',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',9,false,false,false,NOW()-INTERVAL '5 hours',NOW()-INTERVAL '1 hour'),
('66666666-0044-0044-0044-000000000044','55555555-0005-0005-0005-000000000005','44444444-4444-4444-4444-444444444444',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',7,false,false,false,NOW()-INTERVAL '5 hours',NOW()-INTERVAL '1 hour'),

-- Subscription 6 - Jotatsu Active: limit reached
('66666666-0051-0051-0051-000000000051','55555555-0006-0006-0006-000000000006','11111111-1111-1111-1111-111111111111',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',10,false,false,false,NOW()-INTERVAL '6 hours',NOW()),
('66666666-0052-0052-0052-000000000052','55555555-0006-0006-0006-000000000006','22222222-2222-2222-2222-222222222222',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',500,false,false,false,NOW()-INTERVAL '6 hours',NOW()),
('66666666-0053-0053-0053-000000000053','55555555-0006-0006-0006-000000000006','33333333-3333-3333-3333-333333333333',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',10,false,false,false,NOW()-INTERVAL '6 hours',NOW()),
('66666666-0054-0054-0054-000000000054','55555555-0006-0006-0006-000000000006','44444444-4444-4444-4444-444444444444',CURRENT_DATE,CURRENT_DATE+INTERVAL '1 day'-INTERVAL '1 second',10,false,false,false,NOW()-INTERVAL '6 hours',NOW()),

-- Subscription 7 - Jotatsu Expired
('66666666-0061-0061-0061-000000000061','55555555-0007-0007-0007-000000000007','11111111-1111-1111-1111-111111111111',DATE_TRUNC('day',NOW()-INTERVAL '11 days'),DATE_TRUNC('day',NOW()-INTERVAL '10 days')+INTERVAL '1 day'-INTERVAL '1 second',7,false,true,false,NOW()-INTERVAL '2 days',NOW()-INTERVAL '1 day'),
('66666666-0062-0062-0062-000000000062','55555555-0007-0007-0007-000000000007','22222222-2222-2222-2222-222222222222',DATE_TRUNC('day',NOW()-INTERVAL '11 days'),DATE_TRUNC('day',NOW()-INTERVAL '10 days')+INTERVAL '1 day'-INTERVAL '1 second',300,false,true,false,NOW()-INTERVAL '2 days',NOW()-INTERVAL '1 day'),

-- Subscription 8 - Jotatsu Canceled
('66666666-0071-0071-0071-000000000071','55555555-0008-0008-0008-000000000008','11111111-1111-1111-1111-111111111111',CURRENT_DATE-INTERVAL '3 days',CURRENT_DATE-INTERVAL '2 days',3,false,false,true,NOW()-INTERVAL '3 days',NOW()-INTERVAL '2 days'),
('66666666-0072-0072-0072-000000000072','55555555-0008-0008-0008-000000000008','22222222-2222-2222-2222-222222222222',CURRENT_DATE-INTERVAL '3 days',CURRENT_DATE-INTERVAL '2 days',80,false,false,true,NOW()-INTERVAL '3 days',NOW()-INTERVAL '2 days');

-- =========================================================
-- END
-- =========================================================


COMMIT;