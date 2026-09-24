using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Payment.ExpirePendingPayments;

public sealed class ExpirePendingPaymentsHandler : IRequestHandler<ExpirePendingPaymentsCommand, Result<int>>
{
	private readonly IPaymentRepository _paymentRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public ExpirePendingPaymentsHandler(
		IPaymentRepository paymentRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_paymentRepository = paymentRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<int>> Handle(ExpirePendingPaymentsCommand request, CancellationToken cancellationToken)
	{
		var now = _systemClock.UtcNow;

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var expiredPayments = await _paymentRepository.GetExpiredPendingPaymentsAsync(now, cancellationToken);
			if (expiredPayments.Count == 0)
			{
				return Result.Success(0);
			}

			foreach (var payment in expiredPayments)
			{
				payment.Expire(now);
			}

			return Result.Success(expiredPayments.Count);
		}, cancellationToken);
	}
}
