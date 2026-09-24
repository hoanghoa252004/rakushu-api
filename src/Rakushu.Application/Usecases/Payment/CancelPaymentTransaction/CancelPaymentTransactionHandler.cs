using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Payment.PaymentTransaction;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Payment.CancelPaymentTransaction;

public sealed class CancelPaymentTransactionHandler : IRequestHandler<CancelPaymentTransactionCommand, Result>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IPaymentRepository _paymentRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CancelPaymentTransactionHandler(
		ICurrentUserContext currentUserContext,
		IPaymentRepository paymentRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_currentUserContext = currentUserContext;
		_paymentRepository = paymentRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(CancelPaymentTransactionCommand request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId == null)
		{
			return Result.Failure(UserError.NotFound);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var payment = await _paymentRepository.GetByIdWithTransactionsAsync(
				PaymentId.From(request.PaymentId),
				cancellationToken);

			if (payment == null)
			{
				return Result.Failure(PaymentErrors.NotFound);
			}

			var isAdmin = _currentUserContext.Role == DefaultSystemRoles.SystemAdministrator.ToString();
			if (!isAdmin && payment.UserId != userId)
			{
				return Result.Failure(UserError.UnauthorizedResourceAccess);
			}

			var now = _systemClock.UtcNow;
			var transactionId = PaymentTransactionId.From(request.TransactionId);

			return payment.CancelTransaction(transactionId, now);
		}, cancellationToken);
	}
}
