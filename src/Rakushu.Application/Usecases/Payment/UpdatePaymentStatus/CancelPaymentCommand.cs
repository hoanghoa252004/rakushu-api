using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.UpdatePaymentStatus;

public sealed record CancelPaymentCommand(
	Guid PaymentId
) : IRequest<Result>;
