using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Payment.GetPaymentById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Payment.GetPaymentTransactions;

public sealed class GetPaymentTransactionsHandler : IRequestHandler<GetPaymentTransactionsQuery, Result<IReadOnlyCollection<PaymentTransactionDto>>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IPaymentQuery _paymentQuery;

	public GetPaymentTransactionsHandler(
		ICurrentUserContext currentUserContext,
		IPaymentQuery paymentQuery)
	{
		_currentUserContext = currentUserContext;
		_paymentQuery = paymentQuery;
	}

	public async Task<Result<IReadOnlyCollection<PaymentTransactionDto>>> Handle(GetPaymentTransactionsQuery request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId == null)
		{
			return Result.Failure<IReadOnlyCollection<PaymentTransactionDto>>(UserError.NotFound);
		}

		var paymentId = PaymentId.From(request.PaymentId);
		var payment = await _paymentQuery.GetByIdAsync(paymentId, cancellationToken);
		if (payment == null)
		{
			return Result.Failure<IReadOnlyCollection<PaymentTransactionDto>>(PaymentErrors.NotFound);
		}

		var isAdmin = _currentUserContext.Role == DefaultSystemRoles.SystemAdministrator.ToString();
		if (!isAdmin && payment.UserId != userId.Value)
		{
			return Result.Failure<IReadOnlyCollection<PaymentTransactionDto>>(UserError.UnauthorizedResourceAccess);
		}

		var transactions = await _paymentQuery.GetTransactionsByPaymentIdAsync(paymentId, cancellationToken);
		return Result.Success(transactions);
	}
}
