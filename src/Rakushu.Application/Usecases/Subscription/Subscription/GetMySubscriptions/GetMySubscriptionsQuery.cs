using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptionById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.Subscription.GetMySubscriptions;

public sealed record GetMySubscriptionsQuery(
	int PageNumber,
	int PageSize,
	Guid? PlanId = null,
	string? Status = null
) : IRequest<Result<PaginatedList<SubscriptionDetailDto>>>;


