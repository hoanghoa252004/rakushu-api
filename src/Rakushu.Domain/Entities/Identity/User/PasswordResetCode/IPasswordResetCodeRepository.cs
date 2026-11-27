using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Identity.User.PasswordResetCode;

public interface IPasswordResetCodeRepository : IBaseRepository<PasswordResetCode, PasswordResetCodeId>
{
	Task<PasswordResetCode?> GetActiveByUserIdAsync(UserId userId, DateTimeOffset now, CancellationToken cancellationToken = default);

	Task<PasswordResetCode?> GetByUserIdAndCodeHashAsync(UserId userId, string codeHash, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<PasswordResetCode>> GetNotYetFlaggedExpiredAsync(DateTimeOffset now, CancellationToken cancellationToken = default);
}
