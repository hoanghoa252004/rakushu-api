using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Payment.ExpirePayments;

internal sealed class ExpirePaymentsHandler : IRequestHandler<ExpirePaymentsCommand, Result>
{
	// DAOS
	private readonly IPaymentRepository _paymentRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;

	public ExpirePaymentsHandler(
		IPaymentRepository paymentRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_paymentRepository = paymentRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(ExpirePaymentsCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var payments = await _paymentRepository.GetAllAsync(cancellationToken);

			var expiredPayments = payments.Where(p => p.Status == PaymentStatus.Pending && p.ExpiredAt <= now).ToList();

			foreach (var payment in expiredPayments)
			{
				var result = payment.Expire(now);

				if (result.IsFailure)
				{
					return result;
				}
			}

			return Result.Success();
		}, cancellationToken);
	}
}