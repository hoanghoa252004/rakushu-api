using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

namespace Rakushu.Persistence.Repositories;

public sealed class ShadowingMaterialRepository
	: BaseRepository<ShadowingMaterial, ShadowingMaterialId>, IShadowingMaterialRepository
{
	public ShadowingMaterialRepository(RakushuDbContext context) : base(context) { }

	public async Task<int> GetNextVersionAsync(VideoId videoId, CancellationToken cancellationToken = default)
	{
		var max = await _context.ShadowingMaterials
			.Where(m => m.VideoId == videoId)
			.Select(m => (int?)m.Version)
			.MaxAsync(cancellationToken);

		return (max ?? 0) + 1;
	}

	public async Task<ShadowingMaterial?> GetByRequestIdAsync(
		ShadowingMaterialRequestId requestId,
		CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingMaterials
			.FirstOrDefaultAsync(m => m.ShadowingMaterialRequestId == requestId, cancellationToken);
	}

	public async Task<(IReadOnlyList<ShadowingMaterial> Items, int TotalCount)> GetPagedAsync(
		int pageNumber,
		int pageSize,
		VideoId? videoId,
		ShadowingMaterialRequestId? requestId,
		MaterialType? type,
		ShadowingMaterialStatus? status,
		CancellationToken cancellationToken = default)
	{
		var query = _context.ShadowingMaterials.AsNoTracking().AsQueryable();

		if (videoId is not null)
			query = query.Where(m => m.VideoId == videoId);
		if (requestId is not null)
			query = query.Where(m => m.ShadowingMaterialRequestId == requestId);
		if (type is not null)
			query = query.Where(m => m.Type == type);
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
