using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

namespace Rakushu.Persistence.Repositories;

public sealed class ShadowingMaterialRepository
	: BaseRepository<ShadowingMaterial, ShadowingMaterialId>, IShadowingMaterialRepository
{
	public ShadowingMaterialRepository(RakushuDbContext context) : base(context) { }

	public async Task<bool> HasProcessingByLearnerAsync(UserId learnerId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingMaterials
			.AnyAsync(m => m.Video.LearnerId == learnerId && m.Status == ShadowingMaterialStatus.Processing, cancellationToken);
	}

	public async Task<bool> RangeExistsAsync(
		TranscriptSegmentId startSegmentId,
		TranscriptSegmentId endSegmentId,
		ShadowingMaterialId? excludeId,
		CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingMaterials.AnyAsync(
			m => m.StartTranscriptSegmentId == startSegmentId
				&& m.EndTranscriptSegmentId == endSegmentId
				&& (excludeId == null || m.Id != excludeId),
			cancellationToken);
	}

	public async Task<bool> HasSessionsAsync(ShadowingMaterialId materialId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingSessions.AnyAsync(s => s.ShadowingMaterialId == materialId, cancellationToken);
	}

	public async Task<TranscriptSegment?> GetVideoSegmentAsync(
		VideoId videoId,
		TranscriptSegmentId segmentId,
		CancellationToken cancellationToken = default)
	{
		return await _context.TranscriptSegments
			.AsNoTracking()
			.FirstOrDefaultAsync(s => s.Id == segmentId && s.Transcript.VideoId == videoId, cancellationToken);
	}

	public async Task<IReadOnlyList<TranscriptSegment>> GetSegmentsBetweenAsync(
		VideoId videoId,
		int startSequence,
		int endSequence,
		CancellationToken cancellationToken = default)
	{
		return await _context.TranscriptSegments
			.AsNoTracking()
			.Include(s => s.TranscriptSpeaker)
			.Where(s => s.Transcript.VideoId == videoId && s.Sequence >= startSequence && s.Sequence <= endSequence)
			.OrderBy(s => s.Sequence)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<TranscriptSpeaker>> GetVideoTranscriptSpeakersAsync(
		VideoId videoId,
		IReadOnlyCollection<TranscriptSpeakerId> ids,
		CancellationToken cancellationToken = default)
	{
		return await _context.TranscriptSpeakers
			.AsNoTracking()
			.Where(ts => ids.Contains(ts.Id) && ts.Transcript.VideoId == videoId)
			.ToListAsync(cancellationToken);
	}

	public async Task<int> GetNextVersionAsync(VideoId videoId, CancellationToken cancellationToken = default)
	{
		var max = await _context.ShadowingMaterials
			.Where(m => m.VideoId == videoId)
			.Select(m => (int?)m.Version)
			.MaxAsync(cancellationToken);

		return (max ?? 0) + 1;
	}

	public async Task<(IReadOnlyList<ShadowingMaterial> Items, int TotalCount)> GetPagedAsync(
		int pageNumber,
		int pageSize,
		VideoId? videoId,
		ShadowingMaterialStatus? status,
		UserId? visibleToUserId,
		CancellationToken cancellationToken = default)
	{
		var query = _context.ShadowingMaterials.AsNoTracking().AsQueryable();

		if (visibleToUserId is not null)
			query = query.Where(m => m.CreatedById == visibleToUserId);
		if (videoId is not null)
			query = query.Where(m => m.VideoId == videoId);
		if (status is not null)
			query = query.Where(m => m.Status == status);

		var total = await query.CountAsync(cancellationToken);
		var items = await query
			.OrderByDescending(m => m.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, total);
	}

	public async Task<bool> IsVisibleToUserAsync(
		ShadowingMaterialId materialId,
		UserId userId,
		CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingMaterials.AnyAsync(m => m.Id == materialId
			&& m.CreatedById == userId,
			cancellationToken);
	}

	public async Task<IReadOnlyList<ShadowingMaterial>> GetProcessingAsync(
		int batchSize,
		CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingMaterials
			.Where(m => m.Status == ShadowingMaterialStatus.Processing)
			.OrderBy(m => m.CreatedAt)
			.Take(batchSize)
			.ToListAsync(cancellationToken);
	}

	public async Task<string?> GetVideoSourceKeyAsync(
		VideoId videoId,
		CancellationToken cancellationToken = default)
	{
		return await _context.Set<Rakushu.Domain.Entities.Material.Video.MediaAsset.MediaAsset>()
			.AsNoTracking()
			.Where(a => a.VideoId == videoId && a.AssetType == Rakushu.Domain.Entities.Material.Video.MediaAsset.MediaAssetType.Video)
			.Select(a => a.StorageKey)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<TranscriptSegment>> GetTranscriptSegmentsAsync(
		VideoId videoId,
		CancellationToken cancellationToken = default)
	{
		return await _context.TranscriptSegments
			.AsNoTracking()
			.Include(s => s.TranscriptSpeaker)
			.Where(s => s.Transcript.VideoId == videoId)
			.OrderBy(s => s.Sequence)
			.ToListAsync(cancellationToken);
	}
}
