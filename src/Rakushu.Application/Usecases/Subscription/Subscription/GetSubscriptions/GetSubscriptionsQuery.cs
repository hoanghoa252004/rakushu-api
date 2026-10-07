using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptions;

public sealed record GetSubscriptionsQuery(
	int PageNumber,
	int PageSize,
	Guid? UserId = null,
	Guid? PlanId = null,
	string? Status = null
) : IRequest<Result<PaginatedList<SubscriptionDto>>>;

