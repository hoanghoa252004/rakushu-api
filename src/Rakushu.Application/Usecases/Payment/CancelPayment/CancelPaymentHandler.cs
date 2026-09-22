using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Payment.CancelPayment;

public sealed class CancelPaymentHandler : IRequestHandler<CancelPaymentCommand, Result>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IPaymentRepository _paymentRepository;
	private readonly IUserRepository _userRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CancelPaymentHandler(
		ICurrentUserContext currentUserContext,
		IPaymentRepository paymentRepository,
		IUserRepository userRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_currentUserContext = currentUserContext;
		_paymentRepository = paymentRepository;
		_userRepository = userRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId == null)
		{
			return Result.Failure(UserError.NotFound);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var payment = await _paymentRepository.GetByIdWithTransactionsAsync(PaymentId.From(request.PaymentId), cancellationToken);
			if (payment == null)
			{
				return Result.Failure(PaymentErrors.NotFound);
			}

			if (payment.UserId != userId)
			{
				return Result.Failure(UserError.UnauthorizedResourceAccess);
			}

			var now = _systemClock.UtcNow;

			var cancelResult = payment.Cancel(now);
			if (cancelResult.IsFailure)
			{
				return cancelResult;
			}

			if (payment.SubscriptionId != null)
			{
				var user = await _userRepository.GetByIdWithSubscriptionsAsync(userId, cancellationToken);
				user?.MarkSubscriptionFailed(payment.SubscriptionId, now);
			}

			return Result.Success();
		}, cancellationToken);
	}
}
