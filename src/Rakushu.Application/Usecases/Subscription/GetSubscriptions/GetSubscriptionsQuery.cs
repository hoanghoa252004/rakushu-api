using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.GetSubscriptions;

public sealed record GetSubscriptionsQuery(
	int PageNumber = 1,
	int PageSize = 10,
	string? Status = null,
	Guid? PlanId = null,
	Guid? UserId = null
) : IRequest<Result<PaginatedList<SubscriptionAdminDto>>>;
