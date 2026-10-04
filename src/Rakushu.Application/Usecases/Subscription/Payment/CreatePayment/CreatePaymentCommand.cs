using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Payment.CreatePayment;

public sealed record CreatePaymentCommand(Guid PlanId) : IRequest<Result<Guid>>;
