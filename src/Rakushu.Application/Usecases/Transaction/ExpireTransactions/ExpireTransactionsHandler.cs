using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Payment.ExpirePayments;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Payment.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Transaction.ExpireTransactions;

internal sealed class ExpireTransactionsHandler : IRequestHandler<ExpireTransactionsCommand, Result>
{
	// DAOS
	private readonly ITransactionRepository _transactionRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;

	public ExpireTransactionsHandler(
		ITransactionRepository transactionRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_transactionRepository = transactionRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(ExpireTransactionsCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var transactions = await _transactionRepository.GetAllAsync(cancellationToken);

			var expiredTransactions = transactions.Where(t => t.Status == TransactionStatus.Pending && t.ExpiredAt <= now).ToList();

			foreach (var transaction in expiredTransactions)
			{
				var result = transaction.Expire(now);

				if (result.IsFailure)
				{
					return result;
				}
			}

			return Result.Success();
		}, cancellationToken);
	}
}