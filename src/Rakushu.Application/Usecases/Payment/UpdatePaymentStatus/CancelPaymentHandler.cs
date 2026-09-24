using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;

namespace Rakushu.Application.Usecases.Payment.UpdatePaymentStatus;

public sealed class CancelPaymentHandler : IRequestHandler<CancelPaymentCommand, Result>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IPaymentRepository _paymentRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public CancelPaymentHandler(
		ICurrentUserContext currentUserContext,
		IPaymentRepository paymentRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_currentUserContext = currentUserContext;
		_paymentRepository = paymentRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;

		if (userId == null)
		{
			return Result.Failure(PaymentError.InvalidUserId);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var paymentId = PaymentId.From(request.PaymentId);

			var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);

			if (payment == null)
			{
				return Result.Failure(PaymentError.PaymentNotFound);
			}

			// Check if payment belongs to the current user
			if (payment.UserId != userId)
			{
				return Result.Failure(PaymentError.PaymentNotBelong);
			}

			// Update payment status to Cancelled
			var updateResult = payment.CancelPayment(_systemClock.UtcNow);

			if (updateResult.IsFailure)
			{
				return updateResult;
			}

			_paymentRepository.Update(payment);

			return Result.Success();
		}, cancellationToken);
	}
}
