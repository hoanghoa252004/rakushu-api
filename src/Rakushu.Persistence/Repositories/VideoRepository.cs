using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Material.Bunsetsu;
using Rakushu.Domain.Entities.Material.Token;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;
using Rakushu.Domain.Entities.Processing.ProcessingJobLog;

namespace Rakushu.Persistence.Repositories;

public sealed class VideoRepository : BaseRepository<Video, VideoId>, IVideoRepository
{
	public VideoRepository(RakushuDbContext context) : base(context) { }

	public override async Task<Video?> GetByIdAsync(VideoId id, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.Learner)
			.Include(v => v.ContentCategory)
			.Include(v => v.Series)
			.Include(v => v.MediaAssets)
			.Include(v => v.Transcript)
				.ThenInclude(t => t.TranscriptSegments)
					.ThenInclude(ts => ts.Bunsetsus)
						.ThenInclude(b => b.Tokens)
			.Include(v => v.Transcript)
				.ThenInclude(t => t!.TranscriptSpeakers)
			.Include(v => v.Transcript)
				.ThenInclude(t => t!.TranscriptSegments)
					.ThenInclude(ts => ts.TranscriptItems)
			.Include(v => v.Transcript)
				.ThenInclude(t => t!.TranscriptSegments)
					.ThenInclude(ts => ts.Subtitle)
			.Include(v => v.Transcript)
				.ThenInclude(t => t!.TranscriptSegments)
					.ThenInclude(ts => ts.LearningUnits)
			.AsSplitQuery()
			.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
	}

	public async Task<Video?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.Series)
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
					.ThenInclude(s => s.Bunsetsus)
			.FirstOrDefaultAsync(v => v.Transcript != null && v.Transcript.TranscriptSegments.Any(s => s.Bunsetsus.Any(b => b.Id == bunsetsuId)), cancellationToken);
	}

	public async Task<Video?> GetByTokenIdAsync(TokenId tokenId, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.Transcript)
				.ThenInclude(t => t.TranscriptSegments)
					.ThenInclude(s => s.Bunsetsus)
						.ThenInclude(b => b.Tokens)
			.FirstOrDefaultAsync(v => v.Transcript != null && v.Transcript.TranscriptSegments.Any(s => s.Bunsetsus.Any(b => b.Tokens.Any(tok => tok.Id == tokenId))), cancellationToken);
	}

	public override async Task<IEnumerable<Video>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.Learner)
			.Include(v => v.ContentCategory)
			.Include(v => v.Series)
			.Include(v => v.MediaAssets)
			.Include(v => v.Transcript)
				.ThenInclude(t => t.TranscriptSegments)
					.ThenInclude(ts => ts.Bunsetsus)
						.ThenInclude(b => b.Tokens)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Video>> GetProcessingVideosWithReviewedTranscriptAsync(
		int batchSize,
		CancellationToken cancellationToken = default)
	{
		// Bỏ qua video đã matching xong (kể cả khi không tìm được learning unit nào).
		var matchedVideoIds = (await _context.ProcessingJobLogs
			.AsNoTracking()
			.Where(l =>
				l.JobType == ProcessingJobType.LearningUnitMatching &&
				l.TargetType == ProcessingJobTargetType.Video &&
				l.IsSuccess)
			.Select(l => l.TargetId)
			.Distinct()
			.ToListAsync(cancellationToken))
			.Select(VideoId.From)
			.ToList();

		return await _context.Videos
			//.AsNoTracking()
			.Include(v => v.Transcript)
				.ThenInclude(t => t!.TranscriptSegments)
					.ThenInclude(s => s.LearningUnits)
			.Include(t => t!.Transcript)
				.ThenInclude(t => t!.TranscriptSegments)
					.ThenInclude(s => s.Bunsetsus)
						.ThenInclude(b => b.Tokens)
			.Where(v =>
				v.Status == VideoStatus.Processing &&
				v.Transcript != null &&
				v.Transcript.IsReviewed
				&& v.Transcript.TranscriptSegments.All(s => s.LearningUnits.Any() == false)
				&& !matchedVideoIds.Contains(v.Id))
			.OrderBy(v => v.CreatedAt)
			.Take(batchSize)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Video>> GetProcessingVideosWithReviewedTranscriptForTranslateAsync(int batchSize, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.Transcript)
				.ThenInclude(t => t!.TranscriptSegments)
					.ThenInclude(s => s.Subtitle)
			.Where(v =>
				v.Status == VideoStatus.Processing &&
				v.Transcript != null &&
				v.Transcript.IsReviewed
				//&& v.Transcript.TranscriptSegments.Any(s => s.LearningUnits.All(lu => lu.IsReviewed))
				&& v.Transcript.TranscriptSegments.All(s => s.Subtitle == null)
			)
			.Take(batchSize)
			.OrderBy(v => v.CreatedAt)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Video>> GetModeratingVideosAsync(int batchSize, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.MediaAssets)
			.Where(v => v.Status == VideoStatus.Moderating)
			.OrderBy(v => v.CreatedAt)
			.Take(batchSize)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Video>> GetImportingVideosAsync(int batchSize, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.Include(v => v.MediaAssets)
			.Where(v => v.Status == VideoStatus.Importing)
			.OrderBy(v => v.CreatedAt)
			.Take(batchSize)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Video>> GetProcessingVideosWithProcessedResultAsync(int batchSize, CancellationToken cancellationToken = default)
	{
		var processingVideoIds = (await _context.Videos
			.AsNoTracking()
			.Where(v => v.Status == VideoStatus.Processing && v.Transcript != null)
			.Select(v => v.Id)
			.ToListAsync(cancellationToken))
			.Select(id => id.Value)
			.ToList();

		if (processingVideoIds.Count == 0)
			return [];

		var logs = await _context.ProcessingJobLogs
			.AsNoTracking()
			.Where(l =>
				l.TargetType == ProcessingJobTargetType.Video &&
				processingVideoIds.Contains(l.TargetId) &&
				(l.JobType == ProcessingJobType.LearningUnitMatching || l.JobType == ProcessingJobType.Translation))
			.Select(l => new { l.TargetId, l.JobType, l.IsSuccess, l.EndAt, l.StartAt })
			.ToListAsync(cancellationToken);

		// Video sẵn sàng review khi: LearningUnitMatching đã thành công và mọi job Translation (tất cả chunk) đã kết thúc,
		// job Translation mới nhất (chunk cuối / batch) thành công.
		var readyIds = logs
			.GroupBy(l => l.TargetId)
			.Where(g =>
			{
				var matchingDone = g.Any(l => l.JobType == ProcessingJobType.LearningUnitMatching && l.IsSuccess);
				var translations = g.Where(l => l.JobType == ProcessingJobType.Translation).ToList();
				return matchingDone
					&& translations.Count > 0
					&& translations.All(l => l.EndAt != null)
					&& translations.OrderByDescending(l => l.StartAt).First().IsSuccess;
			})
			.Select(g => VideoId.From(g.Key))
			.ToList();

		if (readyIds.Count == 0)
			return [];

		return await _context.Videos
			.Where(v => v.Status == VideoStatus.Processing && readyIds.Contains(v.Id))
			.OrderBy(v => v.CreatedAt)
			.Take(batchSize)
			.ToListAsync(cancellationToken);
	}
}
