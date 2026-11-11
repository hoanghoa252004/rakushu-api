using Rakushu.Domain.Common.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Subscriptions.Subscription;

public interface ISubscriptionRepository : IBaseRepository<Subscription, SubscriptionId>
{
	Task<(IReadOnlyList<Subscription> Items, int TotalCount)> GetPagedByUserAsync(
		Identity.User.UserId userId,
		Plan.PlanId? planId,
		SubscriptionStatus? status,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default);
}
