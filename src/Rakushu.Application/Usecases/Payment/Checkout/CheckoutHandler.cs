using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Payment;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Subscription;
using Rakushu.Domain.Entities.Payment.PaymentTransaction;
using System.Security.Cryptography;

namespace Rakushu.Application.Usecases.Payment.Checkout;

public sealed class CheckoutHandler : IRequestHandler<CheckoutCommand, Result<CheckoutResponse>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IPlanRepository _planRepository;
	private readonly IUserRepository _userRepository;
	private readonly IPaymentRepository _paymentRepository;
	private readonly IPaymentService _sepayService;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CheckoutHandler(
		ICurrentUserContext currentUserContext,
		IPlanRepository planRepository,
		IUserRepository userRepository,
		IPaymentRepository paymentRepository,
		IPaymentService sepayService,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_currentUserContext = currentUserContext;
		_planRepository = planRepository;
		_userRepository = userRepository;
		_paymentRepository = paymentRepository;
		_sepayService = sepayService;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<CheckoutResponse>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId == null)
		{
			return Result.Failure<CheckoutResponse>(UserError.NotFound);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var user = await _userRepository.GetByIdWithSubscriptionsAsync(userId, cancellationToken);
			if (user == null)
			{
				return Result.Failure<CheckoutResponse>(UserError.NotFound);
			}

			var plan = await _planRepository.GetByIdAsync(PlanId.From(request.PlanId), cancellationToken);
			if (plan == null)
			{
				return Result.Failure<CheckoutResponse>(PlanErrors.NotFound);
			}

			if (plan.Status != PlanStatus.Active)
			{
				return Result.Failure<CheckoutResponse>(PlanErrors.PlanNotActive);
			}

			var now = _systemClock.UtcNow;
			var paymentTimeoutMinutes = _sepayService.PaymentTimeoutInMinutes; // 15
			var txTimeoutMinutes = _sepayService.TransactionTimeoutInMinutes; // 5
			var paymentExpiresAt = now.AddMinutes(paymentTimeoutMinutes);

			// 1. Ensure user does not already have an active subscription
			if (user.GetActiveSubscription() != null)
			{
				return Result.Failure<CheckoutResponse>(SubscriptionErrors.AlreadyActive);
			}

			// 2. Check if user already has an active Pending payment
			var existingPendingPayment = await _paymentRepository.GetActivePendingPaymentByUserIdAsync(
				userId,
				now,
				cancellationToken);

			if (existingPendingPayment != null)
			{
				// If checking out the same plan
				if (existingPendingPayment.PlanId == plan.Id)
				{
					var lastTx = existingPendingPayment.Transactions
						.OrderByDescending(t => t.CreatedAt)
						.FirstOrDefault();

					// Case A: Transaction is still Pending AND within its lifetime -> Reuse it!
					if (lastTx != null && lastTx.Status == TransactionStatus.Pending && lastTx.ExpiresAt > now)
					{
						var existingResponse = new CheckoutResponse(
							existingPendingPayment.Id.Value,
							null,
							existingPendingPayment.OrderCode,
							existingPendingPayment.Amount,
							existingPendingPayment.Currency,
							existingPendingPayment.QrCodeUrl,
							lastTx.ExpiresAt, // Session lifetime (5 minutes countdown)
							existingPendingPayment.Plan?.Name ?? plan.Name,
							_sepayService.BankName,
							_sepayService.AccountNumber,
							lastTx.Id.Value);

						return Result.Success(existingResponse);
					}

					// Case B: Transaction expired, but Payment still has time left
					if (lastTx != null && lastTx.Status == TransactionStatus.Pending && lastTx.ExpiresAt <= now)
					{
						lastTx.MarkExpired(now);
					}

					var remainingPaymentTime = existingPendingPayment.ExpiresAt - now;
					if (remainingPaymentTime > TimeSpan.Zero)
					{
						// Lifetime of new transaction cannot exceed remaining lifetime of Payment
						var txDuration = TimeSpan.FromMinutes(txTimeoutMinutes) < remainingPaymentTime
							? TimeSpan.FromMinutes(txTimeoutMinutes)
							: remainingPaymentTime;
						var newTxExpiresAt = now.Add(txDuration);

						var newTxResult = PaymentTransaction.CreatePending(
							existingPendingPayment.Id,
							_sepayService.BankName,
							_sepayService.AccountNumber,
							existingPendingPayment.OrderCode,
							existingPendingPayment.Amount,
							newTxExpiresAt,
							now);

						if (newTxResult.IsSuccess)
						{
							existingPendingPayment.AddTransaction(newTxResult.Value, now);

							var newResponse = new CheckoutResponse(
								existingPendingPayment.Id.Value,
								null,
								existingPendingPayment.OrderCode,
								existingPendingPayment.Amount,
								existingPendingPayment.Currency,
								existingPendingPayment.QrCodeUrl,
								newTxExpiresAt, // Session lifetime
								existingPendingPayment.Plan?.Name ?? plan.Name,
								_sepayService.BankName,
								_sepayService.AccountNumber,
								newTxResult.Value.Id.Value);

							return Result.Success(newResponse);
						}
					}
					else
					{
						existingPendingPayment.Expire(now);
					}
				}
				else
				{
					// If switching to a different plan, cancel the old pending payment
					existingPendingPayment.Cancel(now);
				}
			}

			// 3. Generate friendly OrderCode: RK + timestamp + random 3 digits (e.g. RK2609212359123)
			var randomSuffix = RandomNumberGenerator.GetInt32(100, 1000);
			var orderCode = $"RK{now:yyMMddHHmmss}{randomSuffix}";

			// 4. Generate SePay VietQR URL
			var qrCodeUrl = _sepayService.GenerateVietQrUrl(orderCode, plan.Price, $"Dang ky {plan.Name}");

			// 5. Create Payment (Pending, 15 minutes) with subscriptionId = null
			var paymentResult = Domain.Entities.Payment.Payment.Create(
				userId,
				plan.Id,
				subscriptionId: null,
				orderCode,
				plan.Price,
				paymentExpiresAt,
				now,
				"VND",
				$"Thanh toan goi {plan.Name}",
				qrCodeUrl);

			if (paymentResult.IsFailure)
			{
				return Result.Failure<CheckoutResponse>(paymentResult.Error);
			}

			var payment = paymentResult.Value;

			// 6. Concurrently create first PaymentTransaction (Pending, 5 minutes)
			var firstTxExpiresAt = now.AddMinutes(txTimeoutMinutes);
			var firstTxResult = PaymentTransaction.CreatePending(
				payment.Id,
				_sepayService.BankName,
				_sepayService.AccountNumber,
				orderCode,
				plan.Price,
				firstTxExpiresAt,
				now);

			if (firstTxResult.IsFailure)
			{
				return Result.Failure<CheckoutResponse>(firstTxResult.Error);
			}

			payment.AddTransaction(firstTxResult.Value, now);
			_paymentRepository.Add(payment);

			var response = new CheckoutResponse(
				payment.Id.Value,
				null,
				payment.OrderCode,
				payment.Amount,
				payment.Currency,
				payment.QrCodeUrl,
				firstTxExpiresAt, // Session countdown timer (5 mins)
				plan.Name,
				_sepayService.BankName,
				_sepayService.AccountNumber,
				firstTxResult.Value.Id.Value);

			return Result.Success(response);
		}, cancellationToken);
	}
}
