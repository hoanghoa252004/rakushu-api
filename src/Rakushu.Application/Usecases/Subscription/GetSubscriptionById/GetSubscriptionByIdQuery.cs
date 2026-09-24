using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.GetSubscriptionById;

public sealed record GetSubscriptionByIdQuery(
	Guid SubscriptionId
) : IRequest<Result<SubscriptionDetailDto>>;
