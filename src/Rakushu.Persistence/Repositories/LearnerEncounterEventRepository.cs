using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Learning.LearnerEncounterEvent;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Repositories;

public sealed class LearnerEncounterEventRepository
	: BaseRepository<LearnerEncounterEvent, LearnerEncounterEventId>, ILearnerEncounterEventRepository
{
	public LearnerEncounterEventRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<IReadOnlyDictionary<KnowledgeMeaningId, int>> GetEncounterCountsByMeaningsAsync(
		UserId userId,
		IReadOnlyCollection<KnowledgeMeaningId> meaningIds,
		DateTimeOffset sinceUtc,
		CancellationToken cancellationToken = default)
	{
		if (meaningIds.Count == 0)
		{
			return new Dictionary<KnowledgeMeaningId, int>();
		}

		var rows = await _context.LearnerEncounterEvents
			.AsNoTracking()
			.Where(x => x.UserId == userId
				&& meaningIds.Contains(x.KnowledgeMeaningId)
				&& x.EncounteredAt >= sinceUtc)
			.GroupBy(x => x.KnowledgeMeaningId)
			.Select(g => new
			{
				MeaningId = g.Key,
				Count = g.Count()
			})
			.ToListAsync(cancellationToken);

		return rows.ToDictionary(r => r.MeaningId, r => r.Count);
	}

	public async Task<IReadOnlyDictionary<KnowledgeMeaningId, int>> GetEncounterCountsInWindowAsync(
		UserId userId,
		IReadOnlyDictionary<KnowledgeMeaningId, DateTimeOffset?> exclusiveAfterByMeaning,
		DateTimeOffset minSinceUtc,
		CancellationToken cancellationToken = default)
	{
		if (exclusiveAfterByMeaning.Count == 0)
		{
			return new Dictionary<KnowledgeMeaningId, int>();
		}

		var meaningIds = exclusiveAfterByMeaning.Keys.ToList();
		var rows = await _context.LearnerEncounterEvents
			.AsNoTracking()
			.Where(x => x.UserId == userId
				&& meaningIds.Contains(x.KnowledgeMeaningId)
				&& x.EncounteredAt >= minSinceUtc)
			.Select(x => new
			{
				x.KnowledgeMeaningId,
				x.EncounteredAt
			})
			.ToListAsync(cancellationToken);

		var counts = new Dictionary<KnowledgeMeaningId, int>();
		foreach (var row in rows)
		{
			if (exclusiveAfterByMeaning.TryGetValue(row.KnowledgeMeaningId, out var exclusiveAfter)
				&& (!exclusiveAfter.HasValue || row.EncounteredAt > exclusiveAfter.Value))
			{
				counts[row.KnowledgeMeaningId] = counts.GetValueOrDefault(row.KnowledgeMeaningId, 0) + 1;
			}
		}

		return counts;
	}

	public async Task<int> DeleteByUserAndVideoAsync(
		UserId userId,
		VideoId videoId,
		CancellationToken cancellationToken = default)
	{
		var rows = await _context.LearnerEncounterEvents
			.Where(x => x.UserId == userId && x.VideoId == videoId)
			.ToListAsync(cancellationToken);

		if (rows.Count == 0)
		{
			return 0;
		}

		_context.LearnerEncounterEvents.RemoveRange(rows);
		return rows.Count;
	}
}
