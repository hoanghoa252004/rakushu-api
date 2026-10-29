using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Repositories;

public sealed class LearningUnitRepository : BaseRepository<LearningUnit, LearningUnitId>, ILearningUnitRepository
{
	public LearningUnitRepository(RakushuDbContext context) : base(context) { }

	public async Task<LearningUnit?> GetDetailByIdAsync(LearningUnitId id, CancellationToken cancellationToken = default)
	{
		return await _context.LearningUnits
			.AsNoTracking()
			.Include(l => l.KnowledgeMeaning)
				.ThenInclude(m => m.KnowledgeUnit)
					.ThenInclude(u => u.KnowledgeCollection)
			.Include(l => l.KnowledgeMeaning)
				.ThenInclude(m => m.KnowledgeUnit)
					.ThenInclude(u => u.Meanings)
			.AsSplitQuery()
			.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
	}

	public async Task<(IReadOnlyList<LearningUnit> Items, int TotalCount)> GetPagedByVideoIdAsync(
		VideoId videoId,
		KnowledgeUnitType? knowledgeType,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = _context.LearningUnits
			.AsNoTracking()
			.Where(l => l.TranscriptSegment.Transcript.VideoId == videoId);

		if (knowledgeType.HasValue)
			query = query.Where(l => l.KnowledgeMeaning.KnowledgeUnit.KnowledgeType == knowledgeType.Value);

		var total = await query.CountAsync(cancellationToken);

		var items = await query
			.Include(l => l.KnowledgeMeaning)
				.ThenInclude(m => m.KnowledgeUnit)
					.ThenInclude(u => u.KnowledgeCollection)
			.OrderBy(l => l.TranscriptSegment.Sequence)
			.ThenBy(l => l.StartTokenIndex)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, total);
	}

	public async Task<IReadOnlyDictionary<Guid, LearningUnitMediaContext>> GetMediaContextByMeaningIdsAsync(
		IReadOnlyCollection<Guid> meaningIds,
		CancellationToken cancellationToken = default)
	{
		if (meaningIds == null || meaningIds.Count == 0)
			return new Dictionary<Guid, LearningUnitMediaContext>();

		var typedMeaningIds = meaningIds.Select(KnowledgeMeaningId.From).ToList();

		var learningUnits = await _context.LearningUnits
			.AsNoTracking()
			.Where(lu => typedMeaningIds.Contains(lu.KnowledgeMeaningId))
			.Include(lu => lu.TranscriptSegment)
				.ThenInclude(ts => ts.Transcript)
					.ThenInclude(t => t.Video)
						.ThenInclude(v => v.MediaAssets)
			.Include(lu => lu.TranscriptSegment)
				.ThenInclude(ts => ts.Subtitle)
			.ToListAsync(cancellationToken);

		var result = new Dictionary<Guid, LearningUnitMediaContext>();

		foreach (var lu in learningUnits)
		{
			var meaningGuid = lu.KnowledgeMeaningId.Value;
			if (result.ContainsKey(meaningGuid))
				continue;

			var segment = lu.TranscriptSegment;
			var video = segment?.Transcript?.Video;
			if (video is null || segment is null)
				continue;

			var videoAsset = video.MediaAssets
				.FirstOrDefault(a => a.AssetType == Rakushu.Domain.Entities.Material.Video.MediaAsset.MediaAssetType.Video);

			result[meaningGuid] = new LearningUnitMediaContext(
				meaningGuid,
				video.Id.Value,
				video.Title,
				videoAsset?.StorageKey,
				video.SourceUrl,
				segment.StartTime,
				segment.EndTime,
				segment.Text,
				segment.Subtitle?.Text,
				lu.StartTokenIndex,
				lu.EndTokenIndex,
				segment.Id.Value);
		}

		return result;
	}

	public async Task<IReadOnlyDictionary<Guid, LearningUnitMediaContext>> GetMediaContextBySegmentIdsAsync(
		IReadOnlyCollection<Guid> segmentIds,
		CancellationToken cancellationToken = default)
	{
		if (segmentIds == null || segmentIds.Count == 0)
			return new Dictionary<Guid, LearningUnitMediaContext>();

		var typedSegmentIds = segmentIds.Select(TranscriptSegmentId.From).ToList();

		var learningUnits = await _context.LearningUnits
			.AsNoTracking()
			.Where(lu => typedSegmentIds.Contains(lu.TranscriptSegmentId))
			.Include(lu => lu.TranscriptSegment)
				.ThenInclude(ts => ts.Transcript)
					.ThenInclude(t => t.Video)
						.ThenInclude(v => v.MediaAssets)
			.Include(lu => lu.TranscriptSegment)
				.ThenInclude(ts => ts.Subtitle)
			.ToListAsync(cancellationToken);

		var result = new Dictionary<Guid, LearningUnitMediaContext>();

		foreach (var lu in learningUnits)
		{
			var segmentGuid = lu.TranscriptSegmentId.Value;
			if (result.ContainsKey(segmentGuid))
				continue;

			var segment = lu.TranscriptSegment;
			var video = segment?.Transcript?.Video;
			if (video is null || segment is null)
				continue;

			var videoAsset = video.MediaAssets
				.FirstOrDefault(a => a.AssetType == Rakushu.Domain.Entities.Material.Video.MediaAsset.MediaAssetType.Video);

			result[segmentGuid] = new LearningUnitMediaContext(
				lu.KnowledgeMeaningId.Value,
				video.Id.Value,
				video.Title,
				videoAsset?.StorageKey,
				video.SourceUrl,
				segment.StartTime,
				segment.EndTime,
				segment.Text,
				segment.Subtitle?.Text,
				lu.StartTokenIndex,
				lu.EndTokenIndex,
				segmentGuid);
		}

		return result;
	}
}
