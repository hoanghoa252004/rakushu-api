using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Payment.Checkout;

public sealed record CheckoutCommand(Guid PlanId) : IRequest<Result<CheckoutResponse>>;
