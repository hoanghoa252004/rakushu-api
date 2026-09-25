using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.CancelPaymentHandler;

public sealed record CancelPaymentCommand(
	Guid PaymentId
) : IRequest<Result>;
