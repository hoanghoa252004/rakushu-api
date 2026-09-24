using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.CreatePayment;

public sealed record CreatePaymentCommand(
	Guid PlanId
) : IRequest<Result<Guid>>;
