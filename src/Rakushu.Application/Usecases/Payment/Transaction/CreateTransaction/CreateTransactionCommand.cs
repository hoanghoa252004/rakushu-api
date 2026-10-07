using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.Transaction.CreateTransaction;

public sealed record CreateTransactionCommand(
	Guid PaymentId,
	string Provider,
	string IpAddress
) : IRequest<Result<Guid>>;
