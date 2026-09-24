using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.CancelSubscription;

public sealed record CancelSubscriptionCommand(Guid SubscriptionId) : IRequest<Result>;
