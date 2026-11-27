using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Domain.Entities.Learning.LearnerEncounterEvent;

public interface ILearnerEncounterEventRepository
	: IBaseRepository<LearnerEncounterEvent, LearnerEncounterEventId>
{
	/// <summary>
	/// Counts effective encounter events per KnowledgeMeaningId occurring at or after <paramref name="sinceUtc"/>.
	/// Used for the 7-day SRS immersion badge and Quiz priority scoring.
	/// </summary>
	Task<IReadOnlyDictionary<KnowledgeMeaningId, int>> GetEncounterCountsByMeaningsAsync(
		UserId userId,
		IReadOnlyCollection<KnowledgeMeaningId> meaningIds,
		DateTimeOffset sinceUtc,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Counts effective encounter events per KnowledgeMeaningId where EncounteredAt &gt;= <paramref name="minSinceUtc"/>
	/// and strictly greater than each meaning's optional watermark timestamp (e.g., last passive boost or active review).
	/// </summary>
	Task<IReadOnlyDictionary<KnowledgeMeaningId, int>> GetEncounterCountsInWindowAsync(
		UserId userId,
		IReadOnlyDictionary<KnowledgeMeaningId, DateTimeOffset?> exclusiveAfterByMeaning,
		DateTimeOffset minSinceUtc,
		CancellationToken cancellationToken = default);

	Task<int> DeleteByUserAndVideoAsync(
		UserId userId,
		VideoId videoId,
		CancellationToken cancellationToken = default);
}
