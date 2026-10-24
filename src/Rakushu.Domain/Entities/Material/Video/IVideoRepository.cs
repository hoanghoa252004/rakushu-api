using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Material.Bunsetsu;
using Rakushu.Domain.Entities.Material.Token;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;

namespace Rakushu.Domain.Entities.Material.Video;

public interface IVideoRepository : IBaseRepository<Video, VideoId>
{
	Task<Video?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
	Task<Video?> GetByMediaAssetIdAsync(MediaAssetId mediaAssetId, CancellationToken cancellationToken = default);
	Task<Video?> GetByTranscriptIdAsync(TranscriptId transcriptId, CancellationToken cancellationToken = default);
	Task<Video?> GetByTranscriptSegmentIdAsync(TranscriptSegmentId segmentId, CancellationToken cancellationToken = default);
	Task<Video?> GetByBunsetsuIdAsync(BunsetsuId bunsetsuId, CancellationToken cancellationToken = default);
	Task<Video?> GetByTokenIdAsync(TokenId tokenId, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Video>> GetProcessingVideosWithReviewedTranscriptAsync(
		int batchSize,
	CancellationToken cancellationToken = default);

	Task<IReadOnlyList<Video>> GetProcessingVideosWithReviewedTranscriptForTranslateAsync(
		int batchSize,
	CancellationToken cancellationToken = default);

	Task<IReadOnlyList<Video>> GetProcessingVideosWithProcessedResultAsync(
		int batchSize,
		CancellationToken cancellationToken = default);
}
