using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

public interface IShadowingMaterialRepository : IBaseRepository<ShadowingMaterial, ShadowingMaterialId>
{
	Task<int> GetNextVersionAsync(VideoId videoId, CancellationToken cancellationToken = default);

	Task<bool> HasProcessingByLearnerAsync(UserId learnerId, CancellationToken cancellationToken = default);

	Task<bool> RangeExistsAsync(
		TranscriptSegmentId startSegmentId,
		TranscriptSegmentId endSegmentId,
		ShadowingMaterialId? excludeId,
		CancellationToken cancellationToken = default);

	Task<bool> HasSessionsAsync(ShadowingMaterialId materialId, CancellationToken cancellationToken = default);

	Task<TranscriptSegment?> GetVideoSegmentAsync(
		VideoId videoId,
		TranscriptSegmentId segmentId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<TranscriptSegment>> GetSegmentsBetweenAsync(
		VideoId videoId,
		int startSequence,
		int endSequence,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<TranscriptSpeaker>> GetVideoTranscriptSpeakersAsync(
		VideoId videoId,
		IReadOnlyCollection<TranscriptSpeakerId> ids,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<ShadowingMaterial> Items, int TotalCount)> GetPagedAsync(
		int pageNumber,
		int pageSize,
		VideoId? videoId,
		ShadowingMaterialStatus? status,
		UserId? visibleToUserId,
		CancellationToken cancellationToken = default);

	Task<bool> IsVisibleToUserAsync(
		ShadowingMaterialId materialId,
		UserId userId,
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
