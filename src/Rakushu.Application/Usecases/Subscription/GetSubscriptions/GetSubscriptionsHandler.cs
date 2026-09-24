using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Subscription.GetSubscriptions;

public sealed class GetSubscriptionsHandler : IRequestHandler<GetSubscriptionsQuery, Result<PaginatedList<SubscriptionAdminDto>>>
{
	private readonly ISubscriptionQuery _subscriptionQuery;

	public GetSubscriptionsHandler(ISubscriptionQuery subscriptionQuery)
	{
		_subscriptionQuery = subscriptionQuery;
	}

	public async Task<Result<PaginatedList<SubscriptionAdminDto>>> Handle(GetSubscriptionsQuery request, CancellationToken cancellationToken)
	{
		var (items, totalCount) = await _subscriptionQuery.GetSubscriptionsAsync(request, cancellationToken);

		var result = PaginatedList<SubscriptionAdminDto>.Create(
			items,
			totalCount,
			request.PageNumber,
			request.PageSize);

		return Result.Success(result);
	}
}
