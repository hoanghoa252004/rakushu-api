using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Payment;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Application.Usecases.Transaction.GetTransactionsByPayment;

public sealed class GetTransactionsByPaymentIdHandler : IRequestHandler<GetTransactionsByPaymentIdQuery, Result<IReadOnlyCollection<TransactionDto>>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IPaymentRepository _paymentRepository;
	private readonly ITransactionQuery _transactionQuery;

	public GetTransactionsByPaymentIdHandler(
		ICurrentUserContext currentUserContext,
		IPaymentRepository paymentRepository,
		ITransactionQuery transactionQuery)
	{
		_currentUserContext = currentUserContext;
		_paymentRepository = paymentRepository;
		_transactionQuery = transactionQuery;
	}

	public async Task<Result<IReadOnlyCollection<TransactionDto>>> Handle(GetTransactionsByPaymentIdQuery request, CancellationToken cancellationToken)
	{
		var paymentId = PaymentId.From(request.PaymentId);

		var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);

		if (payment == null)
		{
			return Result.Failure<IReadOnlyCollection<TransactionDto>>(PaymentError.PaymentNotFound);
		}

		// Check authorization: if learner, must own the payment
		var currentUserId = _currentUserContext.UserId;

		var currentRole = _currentUserContext.Role;

		if (currentRole != DefaultSystemRoles.SystemAdministrator.ToString())
		{
			if (currentUserId == null || payment.UserId != currentUserId)
			{
				return Result.Failure<IReadOnlyCollection<TransactionDto>>(PaymentError.PaymentNotBelong);
			}
		}

		// Validate status if provided
		if (!string.IsNullOrWhiteSpace(request.Status) && 
			!Enum.TryParse<Domain.Entities.Payment.Transaction.TransactionStatus>(request.Status, true, out _))
		{
			return Result.Failure<IReadOnlyCollection<TransactionDto>>(PaymentError.InvalidStatus);
		}

		// Validate provider if provided
		if (!string.IsNullOrWhiteSpace(request.Provider) && 
			!Enum.TryParse<Provider>(request.Provider, true, out _))
		{
			return Result.Failure<IReadOnlyCollection<TransactionDto>>(PaymentError.InvalidProvider);
		}

		var transactions = await _transactionQuery.GetTransactionsByPaymentAsync(request, cancellationToken);

		// Hide RawResponsePayload for learners
		var isAdmin = currentRole == DefaultSystemRoles.SystemAdministrator.ToString();
		if (!isAdmin)
		{
			transactions = transactions.Select(t => t with { RawResponsePayload = null }).ToList();
		}

		return Result.Success(transactions);
	}
}
