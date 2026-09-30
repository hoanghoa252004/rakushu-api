using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.MediaAsset;
using Rakushu.Domain.Entities.Video.Transcript;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;

namespace Rakushu.Persistence.Repositories;

public sealed class VideoRepository : BaseRepository<Video, VideoId>, IVideoRepository
{
	public VideoRepository(RakushuDbContext context) : base(context) { }

	public override async Task<Video?> GetByIdAsync(VideoId id, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.MediaAssets)
			.Include(v => v.Transcript)
				.ThenInclude(t => t.TranscriptSegments)
					.ThenInclude(ts => ts.Bunsetsu)
						.ThenInclude(b => b.Tokens)
			.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
	}

	public async Task<Video?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.MediaAssets)
			.Include(v => v.Transcript)
			.FirstOrDefaultAsync(v => v.Slug == slug, cancellationToken);
	}

	public async Task<Video?> GetByMediaAssetIdAsync(MediaAssetId mediaAssetId, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.MediaAssets)
			.FirstOrDefaultAsync(v => v.MediaAssets.Any(m => m.Id == mediaAssetId), cancellationToken);
	}

	public async Task<Video?> GetByTranscriptIdAsync(TranscriptId transcriptId, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.Transcript)
			.FirstOrDefaultAsync(v => v.Transcript != null && v.Transcript.Id == transcriptId, cancellationToken);
	}

	public async Task<Video?> GetByTranscriptSegmentIdAsync(TranscriptSegmentId segmentId, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.Transcript)
				.ThenInclude(t => t.TranscriptSegments)
			.FirstOrDefaultAsync(v => v.Transcript != null && v.Transcript.TranscriptSegments.Any(s => s.Id == segmentId), cancellationToken);
	}

	public async Task<Video?> GetByBunsetsuIdAsync(BunsetsuId bunsetsuId, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.Transcript)
				.ThenInclude(t => t.TranscriptSegments)
					.ThenInclude(s => s.Bunsetsu)
			.FirstOrDefaultAsync(v => v.Transcript != null && v.Transcript.TranscriptSegments.Any(s => s.Bunsetsu.Any(b => b.Id == bunsetsuId)), cancellationToken);
	}

	public async Task<Video?> GetByTokenIdAsync(TokenId tokenId, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.Transcript)
				.ThenInclude(t => t.TranscriptSegments)
					.ThenInclude(s => s.Bunsetsu)
						.ThenInclude(b => b.Tokens)
			.FirstOrDefaultAsync(v => v.Transcript != null && v.Transcript.TranscriptSegments.Any(s => s.Bunsetsu.Any(b => b.Tokens.Any(tok => tok.Id == tokenId))), cancellationToken);
	}

	public override async Task<IEnumerable<Video>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.MediaAssets)
			.Include(v => v.Transcript)
				.ThenInclude(t => t.TranscriptSegments)
					.ThenInclude(ts => ts.Bunsetsu)
						.ThenInclude(b => b.Tokens)
			.ToListAsync(cancellationToken);
	}
}
