using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Video.MediaAsset;
using Rakushu.Domain.Entities.Video.Transcript;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;

namespace Rakushu.Domain.Entities.Video;

public interface IVideoRepository : IBaseRepository<Video, VideoId>
{
	Task<Video?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
	Task<Video?> GetByMediaAssetIdAsync(MediaAssetId mediaAssetId, CancellationToken cancellationToken = default);
	Task<Video?> GetByTranscriptIdAsync(TranscriptId transcriptId, CancellationToken cancellationToken = default);
	Task<Video?> GetByTranscriptSegmentIdAsync(TranscriptSegmentId segmentId, CancellationToken cancellationToken = default);
	Task<Video?> GetByBunsetsuIdAsync(BunsetsuId bunsetsuId, CancellationToken cancellationToken = default);
	Task<Video?> GetByTokenIdAsync(TokenId tokenId, CancellationToken cancellationToken = default);
}
