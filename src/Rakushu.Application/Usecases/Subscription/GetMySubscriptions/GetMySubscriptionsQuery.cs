using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.GetMySubscriptions;

public sealed record GetMySubscriptionsQuery(
	int PageNumber = 1,
	int PageSize = 10
) : IRequest<Result<PaginatedList<SubscriptionSummaryDto>>>;
