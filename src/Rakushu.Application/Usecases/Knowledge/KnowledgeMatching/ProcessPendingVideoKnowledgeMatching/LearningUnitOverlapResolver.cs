using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMatchingCandidate;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeMatching.ProcessPendingVideoKnowledgeMatching;

public sealed class LearningUnitOverlapResolver
{
	public IReadOnlyList<LearningUnit> Resolve(
		IEnumerable<LearningUnit> learningUnits,
		IReadOnlyDictionary<KnowledgeCollectionId, int> collectionPriorities)
	{
		var result = new List<LearningUnit>();

		var groups = learningUnits.GroupBy(x => x.TranscriptSegmentId);

		foreach (var group in groups)
		{
			var ordered = group
				.Select(unit => new
				{
					Unit = unit,
					Priority = GetPriority(unit, collectionPriorities)
				})
				.OrderBy(x => x.Priority)
				.ThenByDescending(x =>
					x.Unit.EndTokenIndex - x.Unit.StartTokenIndex)
				.ToList();

			var selected = new List<LearningUnit>();

			foreach (var item in ordered)
			{
				var candidate = item.Unit;

				var overlaps = selected.Any(existing =>
					candidate.StartTokenIndex <= existing.EndTokenIndex &&
					existing.StartTokenIndex <= candidate.EndTokenIndex);

				if (overlaps)
					continue;

				selected.Add(candidate);
			}

			result.AddRange(selected);
		}

		return result;
	}

	private static int GetPriority(
		LearningUnit learningUnit,
		IReadOnlyDictionary<KnowledgeCollectionId, int> collectionPriorities)
	{
		var collectionId =
			learningUnit.KnowledgeMeaning.KnowledgeUnit.KnowledgeCollectionId;

		return collectionPriorities.TryGetValue(collectionId, out var priority)
			? priority
			: int.MaxValue;
	}
}