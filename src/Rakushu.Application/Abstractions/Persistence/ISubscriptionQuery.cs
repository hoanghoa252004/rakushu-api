using Rakushu.Application.Usecases.Subscription.GetMySubscriptions;
using Rakushu.Application.Usecases.Subscription.GetSubscriptionById;
using Rakushu.Application.Usecases.Subscription.GetSubscriptions;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Subscription;

namespace Rakushu.Application.Abstractions.Persistence;

public interface ISubscriptionQuery
{
	Task<(IReadOnlyCollection<SubscriptionSummaryDto> Items, int TotalCount)> GetMySubscriptionsAsync(
		UserId userId,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyCollection<SubscriptionAdminDto> Items, int TotalCount)> GetSubscriptionsAsync(
		GetSubscriptionsQuery query,
		CancellationToken cancellationToken = default);

	Task<SubscriptionDetailDto?> GetByIdAsync(
		SubscriptionId id,
		CancellationToken cancellationToken = default);
}
