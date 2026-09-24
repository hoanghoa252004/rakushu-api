using MediatR;
using Rakushu.Application.Usecases.Payment.GetPaymentById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.GetPaymentTransactions;

public sealed record GetPaymentTransactionsQuery(
	Guid PaymentId
) : IRequest<Result<IReadOnlyCollection<PaymentTransactionDto>>>;
