using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Payment;
using Rakushu.Application.Abstractions.Infrastructure.PaymentGateway;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Payment.Transaction;

namespace Rakushu.Application.Usecases.Transaction.CreateTransaction;

public sealed class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, Result<Guid>>
{
	// CURRENT USER CONTEXT
	private readonly ICurrentUserContext _currentUserContext;

	// DAOs
	private readonly IPaymentGatewayFactory _gatewayFactory;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly IPaymentRepository _paymentRepository;
	private readonly ISystemClock _systemClock;

	public CreateTransactionHandler(
		ICurrentUserContext currentUserContext,
		IPaymentRepository paymentRepository,
		IPaymentGatewayFactory gatewayFactory,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock
		)
	{
		_currentUserContext = currentUserContext;
		_paymentRepository = paymentRepository;
		_gatewayFactory = gatewayFactory;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result<Guid>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;

		if (userId == null)
		{
			return Result.Failure<Guid>(PaymentError.InvalidUserId);
		}

		// 1. Validate provider
		if (!Enum.TryParse<Provider>(request.Provider, true, out var provider))
		{
			return Result.Failure<Guid>(PaymentError.InvalidProvider);
		}

		// 2. Check if payment exists
		var paymentId = PaymentId.From(request.PaymentId);

		var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);

		if (payment == null)
		{
			return Result.Failure<Guid>(PaymentError.PaymentNotFound);
		}

		// 3. Check if payment belongs to current user
		if (payment.UserId != userId)
		{
			return Result.Failure<Guid>(PaymentError.PaymentNotBelong);
		}
		return await _unitOfWork.ExecuteAsync( async () =>
		{

			// 4. Get payment gateway service based on provider
			var gatewayService = _gatewayFactory.GetPaymentService(provider);

			// 5. Create payment URL params
			var now = _systemClock.UtcNow;

			var expiredAt = gatewayService.GetTransactionExpiration();

			var transactionRef = now.Ticks.ToString();

			var paymentUrlParams = new CreatePaymentUrlParams(
				request.IpAddress,
				payment.Amount,
				$"{payment.User.Profile.FullName} subscribes {payment.Plan.Name}",
				transactionRef,
				now,
				expiredAt);

			// 6. Create payment URL
			var paymentUrlResult = gatewayService.CreatePaymentUrl(paymentUrlParams, cancellationToken);

			if (paymentUrlResult.IsFailure)
			{
				return Result.Failure<Guid>(paymentUrlResult.Error);
			}

			var paymentUrl = paymentUrlResult.Value;

			// 7. Create transaction
			var addTransactionResult = payment.CreateTransaction(
				provider,
				transactionRef,
				paymentUrl,
				expiredAt,
				now
			);

			if (addTransactionResult.IsFailure)
			{
				return Result.Failure<Guid>(addTransactionResult.Error);
			}

			return Result.Success(addTransactionResult.Value.Id.Value);
		}, cancellationToken);
	}
}
