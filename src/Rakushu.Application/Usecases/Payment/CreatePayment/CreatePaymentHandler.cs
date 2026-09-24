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

namespace Rakushu.Application.Usecases.Payment.CreatePayment;

public sealed class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, Result<Guid>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IPlanRepository _planRepository;
	private readonly IUserRepository _userRepository;
	private readonly IPaymentRepository _paymentRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;
	private readonly IPaymentService _paymentService;

	public CreatePaymentHandler(
		ICurrentUserContext currentUserContext,
		IPlanRepository planRepository,
		IUserRepository userRepository,
		IPaymentRepository paymentRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock,
		IPaymentService paymentService)
	{
		_currentUserContext = currentUserContext;
		_planRepository = planRepository;
		_userRepository = userRepository;
		_paymentRepository = paymentRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
		_paymentService = paymentService;
	}

	public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;

		if (userId == null)
		{
			return Result.Failure<Guid>(PaymentError.InvalidUserId);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Check if any plan exists and is active
			var planId = PlanId.From(request.PlanId);

			var plan = await _planRepository.GetByIdAsync(planId, cancellationToken);

			if (plan == null)
			{
				return Result.Failure<Guid>(PaymentError.PlanNotFound);
			}

			if (plan.Status != PlanStatus.Active)
			{
				return Result.Failure<Guid>(PaymentError.PlanNotActive);
			}

			// 2. Check if user has an active subscription with this plan
			var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

			if (user == null)
			{
				return Result.Failure<Guid>(PaymentError.InvalidUserId);
			}

			var hasActiveSubscriptionWithThisPlan = user.Subscriptions
				.Any(s => s.PlanId == planId && s.Status == SubscriptionStatus.Active);

			if (hasActiveSubscriptionWithThisPlan)
			{
				return Result.Failure<Guid>(PaymentError.ActiveSubscriptionWithSpecifiedPlanExists);
			}

			// 3. Check if user has any active subscription in used currently
			var hasActiveSubscription = user.Subscriptions
				.Any(s => s.Status == SubscriptionStatus.Active);

			if (hasActiveSubscription)
			{
				return Result.Failure<Guid>(PaymentError.ActiveSubscriptionExists);
			}

			// 4. Create payment
			var now = _systemClock.UtcNow;

			var expiredAt = _paymentService.GetPaymentExpiration();

			var paymentResult = Domain.Entities.Payment.Payment.Create(
				userId,
				planId,
				plan.Price,
				plan.Currency,
				PaymentStatus.Pending,
				expiredAt,
				now,
				now);

			if (paymentResult.IsFailure)
			{
				return Result.Failure<Guid>(paymentResult.Error);
			}

			var payment = paymentResult.Value;

			_paymentRepository.Add(payment);

			return Result.Success(payment.Id.Value);
		}, cancellationToken);
	}
}

