using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

public interface IShadowingRoleLineRepository : IBaseRepository<ShadowingRoleLine, ShadowingRoleLineId>
{
	Task<ShadowingRoleLine?> GetByIdWithSegmentsAsync(
		ShadowingRoleLineId id,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<TranscriptSegment>> GetVideoSegmentsAsync(
		VideoId videoId,
		IReadOnlyCollection<TranscriptSegmentId> segmentIds,
		CancellationToken cancellationToken = default);

	Task<bool> AnySegmentUsedAsync(
		ShadowingMaterialId materialId,
		IReadOnlyCollection<TranscriptSegmentId> segmentIds,
		ShadowingRoleLineId? excludeLineId,
		CancellationToken cancellationToken = default);

	// Reassigns SortOrder (1..n, by transcript order) for every line of the material. Persisted state must be saved first.
	Task RecomputeSortOrdersAsync(
		ShadowingMaterialId materialId,
		CancellationToken cancellationToken = default);

	Task<bool> SortOrderExistsAsync(
		ShadowingMaterialId materialId,
		int sortOrder,
		ShadowingRoleLineId? excludeId,
		CancellationToken cancellationToken = default);

	Task<int> GetNextSortOrderAsync(
		ShadowingMaterialId materialId,
		CancellationToken cancellationToken = default);

	Task<ShadowingRoleLine?> GetPreviousAsync(
		ShadowingMaterialId materialId,
		int sortOrder,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<ShadowingRoleLine> Items, int TotalCount)> GetPagedAsync(
		ShadowingMaterialId materialId,
		SpeakerId? speakerId,
		ShadowingRoleId? roleId,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default);
}
