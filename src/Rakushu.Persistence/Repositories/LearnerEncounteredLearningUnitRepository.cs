using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Repositories;

public sealed class LearnerEncounteredLearningUnitRepository
	: BaseRepository<LearnerEncounteredLearningUnit, LearnerEncounteredLearningUnitId>, ILearnerEncounteredLearningUnitRepository
{
	public LearnerEncounteredLearningUnitRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<IReadOnlyList<LearnerEncounteredLearningUnit>> GetByUserAndLearningUnitIdsAsync(
		UserId userId,
		IReadOnlyCollection<LearningUnitId> learningUnitIds,
		CancellationToken cancellationToken = default)
	{
		if (learningUnitIds.Count == 0)
		{
			return [];
		}

		return await _context.LearnerEncounteredLearningUnits
			.Where(x => x.UserId == userId && learningUnitIds.Contains(x.LearningUnitId))
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<LearnerEncounteredLearningUnit>> GetByUserAndVideoAsync(
		UserId userId,
		VideoId videoId,
		CancellationToken cancellationToken = default)
	{
		return await _context.LearnerEncounteredLearningUnits
			.AsNoTracking()
			.Where(x => x.UserId == userId && x.VideoId == videoId)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<LearnerEncounteredLearningUnit>> GetByUserWithDetailsAsync(
		UserId userId,
		CancellationToken cancellationToken = default)
	{
		return await _context.LearnerEncounteredLearningUnits
			.AsNoTracking()
			.Include(x => x.KnowledgeMeaning)
				.ThenInclude(km => km.KnowledgeUnit)
					.ThenInclude(ku => ku.ProficiencyLevel)
			.Include(x => x.Video)
				.ThenInclude(v => v.ContentCategory)
			.Include(x => x.LearningUnit)
				.ThenInclude(lu => lu.TranscriptSegment)
			.Where(x => x.UserId == userId)
			.OrderByDescending(x => x.LastEncounteredAt)
			.AsSplitQuery()
			.ToListAsync(cancellationToken);
	}

	public async Task<int> DeleteByUserAndVideoAsync(
		UserId userId,
		VideoId videoId,
		CancellationToken cancellationToken = default)
	{
		var rows = await _context.LearnerEncounteredLearningUnits
			.Where(x => x.UserId == userId && x.VideoId == videoId)
			.ToListAsync(cancellationToken);

		if (rows.Count == 0)
		{
			return 0;
		}

		_context.LearnerEncounteredLearningUnits.RemoveRange(rows);
		return rows.Count;
	}

	public async Task<IReadOnlyList<Video>> GetReadyVideosWithLearningUnitsAsync(
		CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.AsNoTracking()
			.Include(v => v.MediaAssets)
			.Include(v => v.Series)
			.Include(v => v.ContentCategory)
			.Include(v => v.Transcript)
				.ThenInclude(t => t.TranscriptSegments)
					.ThenInclude(ts => ts.LearningUnits)
						.ThenInclude(lu => lu.KnowledgeMeaning)
							.ThenInclude(km => km.KnowledgeUnit)
								.ThenInclude(ku => ku.ProficiencyLevel)
			.Where(v => v.Status == VideoStatus.Ready
				|| (v.SourceType == VideoSource.Curated && v.Status != VideoStatus.Draft && v.Status != VideoStatus.Rejected))
			.OrderBy(v => v.SortOrder)
			.AsSplitQuery()
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<(string LevelCode, string LevelName, int SortOrder, int TotalUnitsCount)>> GetSystemVocabularyCountsByLevelAsync(
		CancellationToken cancellationToken = default)
	{
		var levels = await _context.ProficiencyLevels
			.AsNoTracking()
			.OrderBy(pl => pl.SortOrder)
			.ToListAsync(cancellationToken);

		var countsByLevelId = await _context.KnowledgeUnits
			.AsNoTracking()
			.Where(ku => ku.IsActive)
			.GroupBy(ku => ku.ProficiencyLevelId)
			.Select(g => new
			{
				LevelId = g.Key,
				Count = g.Count()
			})
			.ToDictionaryAsync(x => x.LevelId, x => x.Count, cancellationToken);

		return levels
			.Select(pl => (
				pl.Code,
				pl.Name,
				pl.SortOrder,
				countsByLevelId.GetValueOrDefault(pl.Id, 0)))
			.ToList();
	}

	public async Task<IReadOnlyList<(Guid CategoryId, string CategoryName, IReadOnlyCollection<Guid> KnowledgeUnitIds)>> GetSystemVocabularyByCategoryAsync(
		CancellationToken cancellationToken = default)
	{
		var categories = await _context.ContentCategories
			.AsNoTracking()
			.Where(c => c.IsActive)
			.OrderBy(c => c.Name)
			.ToListAsync(cancellationToken);

		var categoryLinks = await _context.ContentKnowledgeCollections
			.AsNoTracking()
			.ToListAsync(cancellationToken);

		var activeUnits = await _context.KnowledgeUnits
			.AsNoTracking()
			.Where(ku => ku.IsActive)
			.Select(ku => new { ku.Id, ku.KnowledgeCollectionId })
			.ToListAsync(cancellationToken);

		var unitsByCollection = activeUnits
			.GroupBy(u => u.KnowledgeCollectionId)
			.ToDictionary(g => g.Key, g => g.Select(u => u.Id.Value).ToList());

		var videoUnitRows = await _context.LearningUnits
			.AsNoTracking()
			.Select(lu => new
			{
				CategoryId = lu.TranscriptSegment.Transcript.Video.ContentCategoryId,
				KnowledgeUnitId = lu.KnowledgeMeaning.KnowledgeUnitId
			})
			.ToListAsync(cancellationToken);

		var result = new List<(Guid CategoryId, string CategoryName, IReadOnlyCollection<Guid> KnowledgeUnitIds)>();

		foreach (var cat in categories)
		{
			var unitSet = new HashSet<Guid>();

			foreach (var link in categoryLinks.Where(l => l.ContentCategoryId == cat.Id))
			{
				if (unitsByCollection.TryGetValue(link.KnowledgeCollectionId, out var cUnits))
				{
					foreach (var uid in cUnits)
					{
						unitSet.Add(uid);
					}
				}
			}

			foreach (var vRow in videoUnitRows.Where(r => r.CategoryId == cat.Id))
			{
				unitSet.Add(vRow.KnowledgeUnitId.Value);
			}

			if (unitSet.Count > 0)
			{
				result.Add((cat.Id.Value, cat.Name, unitSet));
			}
		}

		return result;
	}
}
