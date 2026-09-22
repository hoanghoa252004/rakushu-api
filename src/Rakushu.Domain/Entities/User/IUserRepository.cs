using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.User.RefreshToken;

namespace Rakushu.Domain.Entities.User;

public interface IUserRepository : IBaseRepository<User, UserId>
{
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
	Task<User?> GetByIdWithSubscriptionsAsync(UserId id, CancellationToken cancellationToken = default);
	Task<User?> GetBySubscriptionIdAsync(Subscription.SubscriptionId subscriptionId, CancellationToken cancellationToken = default);
}
