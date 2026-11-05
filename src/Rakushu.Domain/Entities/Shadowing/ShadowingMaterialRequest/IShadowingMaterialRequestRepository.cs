using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

public interface IShadowingMaterialRequestRepository : IBaseRepository<ShadowingMaterialRequest, ShadowingMaterialRequestId>
{
	Task<string> GenerateNextCodeAsync(CancellationToken cancellationToken = default);

	Task<bool> ExistsByVideoIdAsync(VideoId videoId, CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<ShadowingMaterialRequest> Items, int TotalCount)> GetPagedAsync(
		int pageNumber,
		int pageSize,
		VideoId? videoId,
		UserId? learnerId,
		ShadowingMaterialRequestStatus? status,
		CancellationToken cancellationToken = default);
}
