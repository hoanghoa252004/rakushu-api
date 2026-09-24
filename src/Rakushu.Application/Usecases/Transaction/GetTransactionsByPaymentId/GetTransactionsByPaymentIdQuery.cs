using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Transaction.GetTransactionsByPayment;

public sealed record GetTransactionsByPaymentIdQuery(
	Guid PaymentId,
	string? Status = null,
	string? Provider = null
) : IRequest<Result<IReadOnlyCollection<TransactionDto>>>;
