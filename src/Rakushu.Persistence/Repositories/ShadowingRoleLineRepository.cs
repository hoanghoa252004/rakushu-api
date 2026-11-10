using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Persistence.Repositories;

public sealed class ShadowingRoleLineRepository
	: BaseRepository<ShadowingRoleLine, ShadowingRoleLineId>, IShadowingRoleLineRepository
{
	public ShadowingRoleLineRepository(RakushuDbContext context) : base(context) { }

	public async Task<ShadowingRoleLine?> GetByIdWithSegmentsAsync(
		ShadowingRoleLineId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingRoleLines
			.Include(l => l.TranscriptSegments)
			.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
	}

	public async Task<IReadOnlyList<TranscriptSegment>> GetVideoSegmentsAsync(
		VideoId videoId,
		IReadOnlyCollection<TranscriptSegmentId> segmentIds,
		CancellationToken cancellationToken = default)
	{
		return await _context.Set<TranscriptSegment>()
			.AsNoTracking()
			.Where(s => segmentIds.Contains(s.Id) && s.Transcript.VideoId == videoId)
			.OrderBy(s => s.Sequence)
			.ToListAsync(cancellationToken);
	}

	public async Task<bool> AnySegmentUsedAsync(
		ShadowingMaterialId materialId,
		IReadOnlyCollection<TranscriptSegmentId> segmentIds,
		ShadowingRoleLineId? excludeLineId,
		CancellationToken cancellationToken = default)
	{
		return await _context.Set<ShadowingRoleLineTranscriptSegment>().AnyAsync(
			x => x.ShadowingRoleLine.ShadowingRole.ShadowingMaterialId == materialId
				&& segmentIds.Contains(x.TranscriptSegmentId)
				&& (excludeLineId == null || x.ShadowingRoleLineId != excludeLineId),
			cancellationToken);
	}

	public async Task RecomputeSortOrdersAsync(
		ShadowingMaterialId materialId,
		CancellationToken cancellationToken = default)
	{
		var lines = await _context.ShadowingRoleLines
			.Include(l => l.TranscriptSegments).ThenInclude(x => x.TranscriptSegment)
			.Where(l => l.ShadowingRole.ShadowingMaterialId == materialId)
			.ToListAsync(cancellationToken);

		var order = 1;
		foreach (var line in lines
			.OrderBy(l => l.TranscriptSegments.Count == 0 ? int.MaxValue : l.TranscriptSegments.Min(x => x.TranscriptSegment.Sequence))
			.ThenBy(l => l.StartTime))
		{
			line.SetSortOrder(order++);
		}
	}

	public async Task<bool> SortOrderExistsAsync(
		ShadowingMaterialId materialId,
		int sortOrder,
		ShadowingRoleLineId? excludeId,
		CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingRoleLines.AnyAsync(
			l => l.ShadowingRole.ShadowingMaterialId == materialId
				&& l.SortOrder == sortOrder
				&& (excludeId == null || l.Id != excludeId),
			cancellationToken);
	}

	public async Task<int> GetNextSortOrderAsync(
		ShadowingMaterialId materialId,
		CancellationToken cancellationToken = default)
	{
		var max = await _context.ShadowingRoleLines
			.Where(l => l.ShadowingRole.ShadowingMaterialId == materialId)
			.Select(l => (int?)l.SortOrder)
			.MaxAsync(cancellationToken);

		return (max ?? 0) + 1;
	}

	public async Task<ShadowingRoleLine?> GetPreviousAsync(
		ShadowingMaterialId materialId,
		int sortOrder,
		CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingRoleLines.AsNoTracking()
			.Where(l => l.ShadowingRole.ShadowingMaterialId == materialId && l.SortOrder < sortOrder)
			.OrderByDescending(l => l.SortOrder)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<(IReadOnlyList<ShadowingRoleLine> Items, int TotalCount)> GetPagedAsync(
		ShadowingMaterialId materialId,
		SpeakerId? speakerId,
		ShadowingRoleId? roleId,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = _context.ShadowingRoleLines
			.AsNoTracking()
			.Where(l => l.ShadowingRole.ShadowingMaterialId == materialId);

		if (speakerId is not null)
			query = query.Where(l => l.ShadowingRole.SpeakerId == speakerId);
		if (roleId is not null)
			query = query.Where(l => l.ShadowingRoleId == roleId);

		var total = await query.CountAsync(cancellationToken);
		var items = await query
			.OrderBy(l => l.SortOrder)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, total);
	}
}
