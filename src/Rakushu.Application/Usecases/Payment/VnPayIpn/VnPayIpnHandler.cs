using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.PaymentGateway;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Payment.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Payment.VnPayIpn;


internal sealed class VnPayIpnHandler : IRequestHandler<VnPayIpnCommand, Result>
{
	// DAOS
	private readonly IPaymentRepository _paymentRepository;
	private readonly ITransactionRepository _transactionRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;
	private readonly IPaymentGatewayFactory _paymentGatewayFactory;

	public VnPayIpnHandler(
		IPaymentRepository paymentRepository,
		ITransactionRepository transactionRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock,
		IPaymentGatewayFactory paymentGatewayFactory)
	{
		_paymentRepository = paymentRepository;
		_transactionRepository = transactionRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
		_paymentGatewayFactory = paymentGatewayFactory;
	}

	public async Task<Result> Handle(VnPayIpnCommand request, CancellationToken cancellationToken)
	{
		var gatewayService = _paymentGatewayFactory.GetPaymentService(Provider.VNPAY);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// Check if query parameters exist
			if (request.Parameters.Count == 0)
				return Result.Failure(Error.Failure("99", "Input data required"));

			// Extract and validate signature
			var vnp_SecureHash = gatewayService.GetResponseData(request.Parameters, "vnp_SecureHash");
			if (string.IsNullOrEmpty(vnp_SecureHash.Value))
				return Result.Failure(Error.Failure("99", "Secure hash is required"));

			// Validate signature
			var checkSignature = gatewayService.ValidateSignature(request.Parameters, vnp_SecureHash.Value);
			if (checkSignature.IsFailure)
				return Result.Failure(Error.Failure("97", "Invalid signature"));

			// Parse response data
			var txnRefStr = gatewayService.GetResponseData(request.Parameters, "vnp_TxnRef");
			var transactionNoStr = gatewayService.GetResponseData(request.Parameters, "vnp_TransactionNo");
			var amountStr = gatewayService.GetResponseData(request.Parameters, "vnp_Amount");
			var vnp_ResponseCode = gatewayService.GetResponseData(request.Parameters, "vnp_ResponseCode");
			var vnp_TransactionStatus = gatewayService.GetResponseData(request.Parameters, "vnp_TransactionStatus");

			if (!long.TryParse(txnRefStr.Value, out var transactionRef) ||
				!long.TryParse(transactionNoStr.Value, out var vnpayTranId) ||
				!long.TryParse(amountStr.Value, out var vnp_AmountLong))
				return Result.Failure(Error.Failure("99", "Invalid response data"));

			long vnp_Amount = vnp_AmountLong / 100;

			// Find transaction by Code (transactionRef)
			var transaction = await _transactionRepository.GetByTxnRef(transactionRef.ToString(), cancellationToken);

			if (transaction == null)
				return Result.Failure(Error.Failure("01", "Transaction not found"));

			// Check if transaction already confirmed
			if (transaction.Status != TransactionStatus.Pending)
				return Result.Failure(Error.Failure("02", "Transaction has already ended"));

			// Validate amount
			if (transaction.Amount != vnp_Amount)
				return Result.Failure(Error.Failure("04", "Invalid amount"));

			// Update transaction based on VNPAY response
			var now = _systemClock.UtcNow;

			if (vnp_ResponseCode.Value == "00" && vnp_TransactionStatus.Value == "00")
			{
				// Transaction successful
				var transactionSuccess = transaction.Success(transactionNoStr.Value, request.Parameters, now);

				if (transactionSuccess.IsFailure)
					return Result.Failure(Error.Failure("99", "Failed to update transaction"));

				// Payment completed
				var paymentComplete = transaction.Payment.Complete(now);

				if (paymentComplete.IsFailure)
					return Result.Failure(Error.Failure("99", "Failed to update payment status"));
			}
			else
			{
				// Payment failed/cancelled/expired - Update transaction status only
				// Do NOT update payment status as transaction is just one attempt to pay
				// User can create multiple transactions to retry payment
				Result updateResult;
				string failureReason;

				if (vnp_ResponseCode.Value == "15" || vnp_ResponseCode.Value == "11")
				{
					// Transaction expired
					updateResult = transaction.Expire(now);
					failureReason = "Transaction expired";
				}
				else if (vnp_ResponseCode.Value == "24")
				{
					// Transaction cancelled
					updateResult = transaction.Cancel(now);
					failureReason = "Transaction cancelled";
				}
				else
				{
					// Other codes - treat as failed
					updateResult = transaction.Fail(now);
					failureReason = $"Transaction failed with response code {vnp_ResponseCode}";
				}

				if (updateResult.IsFailure)
				{
					return Result.Failure(Error.Failure("99", failureReason));
				}
			}
			return Result.Success();
		}, cancellationToken);
	}
}

