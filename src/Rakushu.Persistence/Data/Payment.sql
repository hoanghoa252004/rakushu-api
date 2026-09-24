-- =========================================================
-- PAYMENTS
-- =========================================================

INSERT INTO payments
(
	id,
	user_id,
	plan_id,
	amount,
	currency,
	status,
	expired_at,
	created_at,
	updated_at
)
VALUES
-- Learner 1 (cccccccc-cccc-cccc-cccc-cccccccccccc) payments
(
	'11111111-0001-0001-0001-000000000001',
	'cccccccc-cccc-cccc-cccc-cccccccccccc',
	'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
	10000.00,
	'VND',
	'Completed',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '5 days',
	NOW() - INTERVAL '5 days'
),
(
	'11111111-0002-0002-0002-000000000002',
	'cccccccc-cccc-cccc-cccc-cccccccccccc',
	'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
	10000.00,
	'VND',
	'Completed',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '4 days',
	NOW() - INTERVAL '4 days'
),
(
	'11111111-0003-0003-0003-000000000003',
	'cccccccc-cccc-cccc-cccc-cccccccccccc',
	'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
	99000.00,
	'VND',
	'Pending',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '1 hour',
	NOW() - INTERVAL '1 hour'
),
(
	'11111111-0004-0004-0004-000000000004',
	'cccccccc-cccc-cccc-cccc-cccccccccccc',
	'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
	10000.00,
	'VND',
	'Failed',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '2 days',
	NOW() - INTERVAL '2 days'
),
(
	'11111111-0005-0005-0005-000000000005',
	'cccccccc-cccc-cccc-cccc-cccccccccccc',
	'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
	10000.00,
	'VND',
	'Cancelled',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '3 days',
	NOW() - INTERVAL '3 days'
),
-- Learner 2 (dddddddd-dddd-dddd-dddd-dddddddddddd) payments
(
	'11111111-0006-0006-0006-000000000006',
	'dddddddd-dddd-dddd-dddd-dddddddddddd',
	'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
	10000.00,
	'VND',
	'Completed',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '6 days',
	NOW() - INTERVAL '6 days'
),
(
	'11111111-0007-0007-0007-000000000007',
	'dddddddd-dddd-dddd-dddd-dddddddddddd',
	'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
	10000.00,
	'VND',
	'Expired',
	NOW() - INTERVAL '1 minute',
	NOW() - INTERVAL '30 minutes',
	NOW() - INTERVAL '30 minutes'
),
(
	'11111111-0008-0008-0008-000000000008',
	'dddddddd-dddd-dddd-dddd-dddddddddddd',
	'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
	99000.00,
	'VND',
	'Completed',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '7 days',
	NOW() - INTERVAL '7 days'
),
(
	'11111111-0009-0009-0009-000000000009',
	'dddddddd-dddd-dddd-dddd-dddddddddddd',
	'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
	10000.00,
	'VND',
	'Pending',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '30 minutes',
	NOW() - INTERVAL '30 minutes'
),
(
	'11111111-0010-0010-0010-000000000010',
	'cccccccc-cccc-cccc-cccc-cccccccccccc',
	'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
	10000.00,
	'VND',
	'Completed',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '8 days',
	NOW() - INTERVAL '8 days'
);


-- =========================================================
-- TRANSACTIONS
-- =========================================================

-- Transactions for Payment 1 (cccccccc, Minion plan, Completed)
INSERT INTO transactions
(
	id,
	payment_id,
	provider,
	amount,
	currency,
	txn_ref,
	url,
	transaction_no,
	raw_response_payload,
	status,
	expired_at,
	created_at,
	updated_at
)
VALUES
(
	'22222222-0001-0001-0001-000000000001',
	'11111111-0001-0001-0001-000000000001',
	'VNPAY',
	10000.00,
	'VND',
	'TXNREF_00001_20250125',
	'https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00001_20250125',
	'0000000001',
	'{"responseCode":"00","message":"Success"}',
	'Successful',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '5 days',
	NOW() - INTERVAL '5 days'
);

-- Transactions for Payment 2 (cccccccc, Minion plan, Completed)
INSERT INTO transactions
(
	id,
	payment_id,
	provider,
	amount,
	currency,
	txn_ref,
	url,
	transaction_no,
	raw_response_payload,
	status,
	expired_at,
	created_at,
	updated_at
)
VALUES
(
	'22222222-0002-0002-0002-000000000002',
	'11111111-0002-0002-0002-000000000002',
	'SEPAY',
	10000.00,
	'VND',
	'TXNREF_00002_20250126',
	'https://sepay.vn/payment?transactionId=TXNREF_00002_20250126',
	'0000000002',
	'{"status":"success","code":0}',
	'Successful',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '4 days',
	NOW() - INTERVAL '4 days'
);

-- Transactions for Payment 3 (cccccccc, Pro plan, Pending) - 2 transactions
INSERT INTO transactions
(
	id,
	payment_id,
	provider,
	amount,
	currency,
	txn_ref,
	url,
	transaction_no,
	raw_response_payload,
	status,
	expired_at,
	created_at,
	updated_at
)
VALUES
(
	'22222222-0003-0003-0003-000000000003',
	'11111111-0003-0003-0003-000000000003',
	'VNPAY',
	99000.00,
	'VND',
	'TXNREF_00003_20250127',
	'https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00003_20250127',
	NULL,
	NULL,
	'Pending',
	NOW() + INTERVAL '14 minutes',
	NOW() - INTERVAL '1 hour',
	NOW() - INTERVAL '1 hour'
),
(
	'22222222-0003-0003-0003-000000000004',
	'11111111-0003-0003-0003-000000000003',
	'SEPAY',
	99000.00,
	'VND',
	'TXNREF_00003B_20250127',
	'https://sepay.vn/payment?transactionId=TXNREF_00003B_20250127',
	NULL,
	NULL,
	'Pending',
	NOW() + INTERVAL '14 minutes',
	NOW() - INTERVAL '1 hour',
	NOW() - INTERVAL '1 hour'
);

-- Transactions for Payment 4 (cccccccc, Minion plan, Failed)
INSERT INTO transactions
(
	id,
	payment_id,
	provider,
	amount,
	currency,
	txn_ref,
	url,
	transaction_no,
	raw_response_payload,
	status,
	expired_at,
	created_at,
	updated_at
)
VALUES
(
	'22222222-0004-0004-0004-000000000005',
	'11111111-0004-0004-0004-000000000004',
	'VNPAY',
	10000.00,
	'VND',
	'TXNREF_00004_20250128',
	'https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00004_20250128',
	NULL,
	'{"responseCode":"24","message":"Cancelled"}',
	'Failed',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '2 days',
	NOW() - INTERVAL '2 days'
);

-- Transactions for Payment 5 (cccccccc, Minion plan, Cancelled)
INSERT INTO transactions
(
	id,
	payment_id,
	provider,
	amount,
	currency,
	txn_ref,
	url,
	transaction_no,
	raw_response_payload,
	status,
	expired_at,
	created_at,
	updated_at
)
VALUES
(
	'22222222-0005-0005-0005-000000000006',
	'11111111-0005-0005-0005-000000000005',
	'SEPAY',
	10000.00,
	'VND',
	'TXNREF_00005_20250129',
	'https://sepay.vn/payment?transactionId=TXNREF_00005_20250129',
	NULL,
	'{"status":"cancelled","code":1}',
	'Cancelled',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '3 days',
	NOW() - INTERVAL '3 days'
);

-- Transactions for Payment 6 (dddddddd, Minion plan, Completed)
INSERT INTO transactions
(
	id,
	payment_id,
	provider,
	amount,
	currency,
	txn_ref,
	url,
	transaction_no,
	raw_response_payload,
	status,
	expired_at,
	created_at,
	updated_at
)
VALUES
(
	'22222222-0006-0006-0006-000000000007',
	'11111111-0006-0006-0006-000000000006',
	'VNPAY',
	10000.00,
	'VND',
	'TXNREF_00006_20250130',
	'https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00006_20250130',
	'0000000006',
	'{"responseCode":"00","message":"Success"}',
	'Successful',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '6 days',
	NOW() - INTERVAL '6 days'
);

-- Transactions for Payment 7 (dddddddd, Minion plan, Expired)
INSERT INTO transactions
(
	id,
	payment_id,
	provider,
	amount,
	currency,
	txn_ref,
	url,
	transaction_no,
	raw_response_payload,
	status,
	expired_at,
	created_at,
	updated_at
)
VALUES
(
	'22222222-0007-0007-0007-000000000008',
	'11111111-0007-0007-0007-000000000007',
	'SEPAY',
	10000.00,
	'VND',
	'TXNREF_00007_20250131',
	'https://sepay.vn/payment?transactionId=TXNREF_00007_20250131',
	NULL,
	NULL,
	'Expired',
	NOW() - INTERVAL '1 minute',
	NOW() - INTERVAL '30 minutes',
	NOW() - INTERVAL '30 minutes'
);

-- Transactions for Payment 8 (dddddddd, Pro plan, Completed) - 2 transactions
INSERT INTO transactions
(
	id,
	payment_id,
	provider,
	amount,
	currency,
	txn_ref,
	url,
	transaction_no,
	raw_response_payload,
	status,
	expired_at,
	created_at,
	updated_at
)
VALUES
(
	'22222222-0008-0008-0008-000000000009',
	'11111111-0008-0008-0008-000000000008',
	'VNPAY',
	99000.00,
	'VND',
	'TXNREF_00008_20250201',
	'https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00008_20250201',
	'0000000008',
	'{"responseCode":"00","message":"Success"}',
	'Successful',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '7 days',
	NOW() - INTERVAL '7 days'
),
(
	'22222222-0008-0008-0008-000000000010',
	'11111111-0008-0008-0008-000000000008',
	'SEPAY',
	99000.00,
	'VND',
	'TXNREF_00008B_20250201',
	'https://sepay.vn/payment?transactionId=TXNREF_00008B_20250201',
	NULL,
	NULL,
	'Successful',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '7 days',
	NOW() - INTERVAL '7 days'
);

-- Transactions for Payment 9 (dddddddd, Minion plan, Pending) - 2 transactions
INSERT INTO transactions
(
	id,
	payment_id,
	provider,
	amount,
	currency,
	txn_ref,
	url,
	transaction_no,
	raw_response_payload,
	status,
	expired_at,
	created_at,
	updated_at
)
VALUES
(
	'22222222-0009-0009-0009-000000000011',
	'11111111-0009-0009-0009-000000000009',
	'VNPAY',
	10000.00,
	'VND',
	'TXNREF_00009_20250202',
	'https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00009_20250202',
	NULL,
	NULL,
	'Pending',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '30 minutes',
	NOW() - INTERVAL '30 minutes'
),
(
	'22222222-0009-0009-0009-000000000012',
	'11111111-0009-0009-0009-000000000009',
	'SEPAY',
	10000.00,
	'VND',
	'TXNREF_00009B_20250202',
	'https://sepay.vn/payment?transactionId=TXNREF_00009B_20250202',
	NULL,
	NULL,
	'Pending',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '30 minutes',
	NOW() - INTERVAL '30 minutes'
);

-- Transactions for Payment 10 (cccccccc, Minion plan, Completed) - 2 transactions
INSERT INTO transactions
(
	id,
	payment_id,
	provider,
	amount,
	currency,
	txn_ref,
	url,
	transaction_no,
	raw_response_payload,
	status,
	expired_at,
	created_at,
	updated_at
)
VALUES
(
	'22222222-0010-0010-0010-000000000013',
	'11111111-0010-0010-0010-000000000010',
	'VNPAY',
	10000.00,
	'VND',
	'TXNREF_00010_20250203',
	'https://sandbox.vnpayment.vn/paygate/Vpayment.html?vnp_TxnRef=TXNREF_00010_20250203',
	'0000000010',
	'{"responseCode":"00","message":"Success"}',
	'Successful',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '8 days',
	NOW() - INTERVAL '8 days'
),
(
	'22222222-0010-0010-0010-000000000014',
	'11111111-0010-0010-0010-000000000010',
	'SEPAY',
	10000.00,
	'VND',
	'TXNREF_00010B_20250203',
	'https://sepay.vn/payment?transactionId=TXNREF_00010B_20250203',
	'0000000010B',
	'{"status":"success","code":0}',
	'Successful',
	NOW() + INTERVAL '15 minutes',
	NOW() - INTERVAL '8 days',
	NOW() - INTERVAL '8 days'
);
