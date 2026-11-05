using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

public interface IShadowingMaterialRepository : IBaseRepository<ShadowingMaterial, ShadowingMaterialId>
{
	Task<int> GetNextVersionAsync(VideoId videoId, CancellationToken cancellationToken = default);

	Task<bool> ExistsByVideoAndTypeAsync(VideoId videoId, MaterialType type, CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<ShadowingMaterial> Items, int TotalCount)> GetPagedAsync(
		int pageNumber,
		int pageSize,
		VideoId? videoId,
		MaterialType? type,
		ShadowingMaterialStatus? status,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<ShadowingMaterial>> GetProcessingAsync(
		int batchSize,
		CancellationToken cancellationToken = default);

	Task<string?> GetVideoSourceKeyAsync(
		VideoId videoId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<TranscriptSegment>> GetTranscriptSegmentsAsync(
		VideoId videoId,
		CancellationToken cancellationToken = default);
}
