using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Payment;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Payment.PaymentTransaction;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.User;
using System.Text.RegularExpressions;

namespace Rakushu.Application.Usecases.Payment.ProcessSepayWebhook;

public sealed partial class ProcessSepayWebhookHandler : IRequestHandler<ProcessSepayWebhookCommand, Result<bool>>
{
	private readonly IPaymentRepository _paymentRepository;
	private readonly IUserRepository _userRepository;
	private readonly IPaymentService _sepayService;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	[GeneratedRegex(@"RK\d+", RegexOptions.IgnoreCase)]
	private static partial Regex OrderCodeRegex();

	public ProcessSepayWebhookHandler(
		IPaymentRepository paymentRepository,
		IUserRepository userRepository,
		IPaymentService sepayService,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_paymentRepository = paymentRepository;
		_userRepository = userRepository;
		_sepayService = sepayService;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<bool>> Handle(ProcessSepayWebhookCommand request, CancellationToken cancellationToken)
	{
		// 1. Verify HMAC signature
		if (!_sepayService.ValidateWebhook(request.SignatureHeader, request.RawData, request.TimestampHeader))
		{
			return Result.Failure<bool>(Error.Unauthorized("WEBHOOK.UNAUTHORIZED", "Invalid webhook signature."));
		}

		var payload = request.Payload;

		// 2. Test ping from SePay Dashboard ("Gửi test")
		if (payload.Id == 0 || string.Equals(payload.Code, "SEPAYTEST", StringComparison.OrdinalIgnoreCase))
		{
			return Result.Success(true);
		}

		// 3. Check idempotency (Already processed this sepay_id)
		if (await _paymentRepository.HasTransactionWithSepayIdAsync(payload.Id, cancellationToken))
		{
			return Result.Success(true);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;
			var transactionDate = DateTimeOffset.TryParse(payload.TransactionDate, out var parsedDate) 
				? parsedDate.ToUniversalTime() 
				: now;

			// 3. Extract OrderCode from content or code field
			string? extractedOrderCode = null;
			if (!string.IsNullOrWhiteSpace(payload.Content))
			{
				var match = OrderCodeRegex().Match(payload.Content);
				if (match.Success)
				{
					extractedOrderCode = match.Value.ToUpperInvariant();
				}
			}

			if (string.IsNullOrWhiteSpace(extractedOrderCode) && !string.IsNullOrWhiteSpace(payload.Code))
			{
				extractedOrderCode = payload.Code.Trim().ToUpperInvariant();
			}

			if (string.IsNullOrWhiteSpace(extractedOrderCode))
			{
				return Result.Failure<bool>(PaymentErrors.OrderCodeNotFound);
			}

			var payment = await _paymentRepository.GetByOrderCodeAsync(extractedOrderCode, cancellationToken);
			if (payment == null)
			{
				return Result.Failure<bool>(PaymentErrors.OrderCodeNotFound);
			}

			// 4. Find the latest pending transaction (if any)
			var pendingTx = payment.Transactions
				.OrderByDescending(t => t.CreatedAt)
				.FirstOrDefault(t => t.Status == TransactionStatus.Pending);

			// Handle based on Payment status
			if (payment.Status == PaymentStatus.Completed)
			{
				// Already completed, record transaction as Success
				var completedTx = PaymentTransaction.Create(
					payment.Id,
					payload.Id,
					payload.Gateway ?? "Unknown",
					payload.AccountNumber ?? string.Empty,
					transactionDate,
					payload.Content ?? string.Empty,
					payload.TransferType ?? "in",
					payload.TransferAmount,
					payload.ReferenceCode,
					TransactionStatus.Success,
					request.RawData,
					payment.ExpiresAt,
					now);

				if (completedTx.IsSuccess)
				{
					payment.AddTransaction(completedTx.Value, now);
				}

				return Result.Success(true);
			}

			if (payment.Status == PaymentStatus.Expired || payment.Status == PaymentStatus.Canceled)
			{
				if (pendingTx != null)
				{
					pendingTx.MarkFailed(now);
				}
				else
				{
					var failedTx = PaymentTransaction.Create(
						payment.Id,
						payload.Id,
						payload.Gateway ?? "Unknown",
						payload.AccountNumber ?? string.Empty,
						transactionDate,
						payload.Content ?? string.Empty,
						payload.TransferType ?? "in",
						payload.TransferAmount,
						payload.ReferenceCode,
						TransactionStatus.Failed,
						request.RawData,
						payment.ExpiresAt,
						now);

					if (failedTx.IsSuccess)
					{
						payment.AddTransaction(failedTx.Value, now);
					}
				}

				return Result.Failure<bool>(PaymentErrors.Expired);
			}

			if (payment.ExpiresAt <= now)
			{
				payment.Expire(now);
				if (pendingTx != null)
				{
					pendingTx.MarkFailed(now);
				}

				return Result.Failure<bool>(PaymentErrors.Expired);
			}

			// 5. Verify transfer amount
			if (payload.TransferAmount < payment.Amount)
			{
				if (pendingTx != null)
				{
					pendingTx.MarkFailed(now);
				}
				else
				{
					var partialTx = PaymentTransaction.Create(
						payment.Id,
						payload.Id,
						payload.Gateway ?? "Unknown",
						payload.AccountNumber ?? string.Empty,
						transactionDate,
						payload.Content ?? string.Empty,
						payload.TransferType ?? "in",
						payload.TransferAmount,
						payload.ReferenceCode,
						TransactionStatus.Failed,
						request.RawData,
						payment.ExpiresAt,
						now);

					if (partialTx.IsSuccess)
					{
						payment.AddTransaction(partialTx.Value, now);
					}
				}

				payment.MarkFailed(now);
				return Result.Failure<bool>(PaymentErrors.AmountMismatch);
			}

			// 6. Complete Payment and Transaction
			if (pendingTx != null)
			{
				pendingTx.MarkSuccess(
					payload.Id,
					payload.Gateway ?? "Unknown",
					payload.AccountNumber ?? string.Empty,
					transactionDate,
					payload.Content ?? string.Empty,
					payload.TransferAmount,
					payload.ReferenceCode,
					request.RawData,
					now);
			}
			else
			{
				var successTx = PaymentTransaction.Create(
					payment.Id,
					payload.Id,
					payload.Gateway ?? "Unknown",
					payload.AccountNumber ?? string.Empty,
					transactionDate,
					payload.Content ?? string.Empty,
					payload.TransferType ?? "in",
					payload.TransferAmount,
					payload.ReferenceCode,
					TransactionStatus.Success,
					request.RawData,
					payment.ExpiresAt,
					now);

				if (successTx.IsFailure)
				{
					return Result.Failure<bool>(successTx.Error);
				}

				payment.AddTransaction(successTx.Value, now);
			}

			payment.Complete(now);

			// 7. Create Active Subscription & Generate Usages via User Aggregate Root
			var user = await _userRepository.GetByIdWithSubscriptionsAsync(payment.UserId, cancellationToken);
			if (user != null)
			{
				var plan = payment.Plan;
				var startDate = now;
				var endDate = (plan != null && plan.BillingCycle == BillingCycle.Yearly)
					? startDate.AddYears(1)
					: startDate.AddMonths(1);

				var subResult = user.CreateActiveSubscription(payment.PlanId, startDate, endDate, now);
				if (subResult.IsSuccess)
				{
					var newSubscription = subResult.Value;
					payment.AttachSubscription(newSubscription.Id, now);

					if (plan != null)
					{
						foreach (var entitlement in plan.PlanEntitlements.Where(e => e.IsEnabled))
						{
							user.AddSubscriptionUsage(newSubscription.Id, entitlement.FeatureId, startDate, endDate, now);
						}
					}
				}
			}

			return Result.Success(true);
		}, cancellationToken);
	}
}
