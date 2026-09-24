using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.CancelPaymentTransaction;

public sealed record CancelPaymentTransactionCommand(Guid PaymentId, Guid TransactionId) : IRequest<Result>;
