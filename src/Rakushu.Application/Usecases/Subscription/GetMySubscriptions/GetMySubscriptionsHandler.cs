using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Subscription.GetMySubscriptions;

public sealed class GetMySubscriptionsHandler : IRequestHandler<GetMySubscriptionsQuery, Result<PaginatedList<SubscriptionSummaryDto>>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISubscriptionQuery _subscriptionQuery;

	public GetMySubscriptionsHandler(
		ICurrentUserContext currentUserContext,
		ISubscriptionQuery subscriptionQuery)
	{
		_currentUserContext = currentUserContext;
		_subscriptionQuery = subscriptionQuery;
	}

	public async Task<Result<PaginatedList<SubscriptionSummaryDto>>> Handle(GetMySubscriptionsQuery request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId == null)
		{
			return Result.Failure<PaginatedList<SubscriptionSummaryDto>>(UserError.NotFound);
		}

		var (items, totalCount) = await _subscriptionQuery.GetMySubscriptionsAsync(
			userId,
			request.PageNumber,
			request.PageSize,
			cancellationToken);

		var result = PaginatedList<SubscriptionSummaryDto>.Create(
			items,
			totalCount,
			request.PageNumber,
			request.PageSize);

		return Result.Success(result);
	}
}
