using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Domain.Entities.Learning.LearnerVideoProgress;

public interface ILearnerVideoProgressRepository : IBaseRepository<LearnerVideoProgress, LearnerVideoProgressId>
{
	Task<LearnerVideoProgress?> GetByUserAndVideoAsync(
		UserId userId,
		VideoId videoId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<LearnerVideoProgress>> GetByUserWithVideoDetailsAsync(
		UserId userId,
		CancellationToken cancellationToken = default);

	Task<bool> DeleteByUserAndVideoAsync(
		UserId userId,
		VideoId videoId,
		CancellationToken cancellationToken = default);
}
