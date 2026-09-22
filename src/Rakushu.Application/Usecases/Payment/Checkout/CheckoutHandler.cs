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
using System.Security.Cryptography;

namespace Rakushu.Application.Usecases.Payment.Checkout;

public sealed class CheckoutHandler : IRequestHandler<CheckoutCommand, Result<CheckoutResponse>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IPlanRepository _planRepository;
	private readonly IUserRepository _userRepository;
	private readonly IPaymentRepository _paymentRepository;
	private readonly ISepayService _sepayService;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CheckoutHandler(
		ICurrentUserContext currentUserContext,
		IPlanRepository planRepository,
		IUserRepository userRepository,
		IPaymentRepository paymentRepository,
		ISepayService sepayService,
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
			var timeoutMinutes = _sepayService.PaymentTimeoutInMinutes;
			var expiresAt = now.AddMinutes(timeoutMinutes);

			// 1. Create Pending Subscription via User Aggregate Root
			var subscriptionResult = user.CreatePendingSubscription(plan.Id, now);
			if (subscriptionResult.IsFailure)
			{
				return Result.Failure<CheckoutResponse>(subscriptionResult.Error);
			}
			var subscription = subscriptionResult.Value;

			// 2. Generate friendly OrderCode: RK + timestamp + random 3 digits (e.g. RK2609212359123)
			var randomSuffix = RandomNumberGenerator.GetInt32(100, 1000);
			var orderCode = $"RK{now:yyMMddHHmmss}{randomSuffix}";

			// 3. Generate SePay VietQR URL
			var qrCodeUrl = _sepayService.GenerateVietQrUrl(orderCode, plan.Price, $"Dang ky {plan.Name}");

			// 4. Create Payment (Pending)
			var paymentResult = Domain.Entities.Payment.Payment.Create(
				userId,
				plan.Id,
				subscription.Id,
				orderCode,
				plan.Price,
				expiresAt,
				now,
				"VND",
				$"Thanh toan goi {plan.Name}",
				qrCodeUrl);

			if (paymentResult.IsFailure)
			{
				return Result.Failure<CheckoutResponse>(paymentResult.Error);
			}

			var payment = paymentResult.Value;
			_paymentRepository.Add(payment);

			var response = new CheckoutResponse(
				payment.Id.Value,
				subscription.Id.Value,
				payment.OrderCode,
				payment.Amount,
				payment.Currency,
				payment.QrCodeUrl,
				payment.ExpiresAt,
				plan.Name,
				_sepayService.BankName,
				_sepayService.AccountNumber);

			return Result.Success(response);
		}, cancellationToken);
	}
}
