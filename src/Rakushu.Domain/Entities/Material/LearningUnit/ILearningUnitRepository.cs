using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Domain.Entities.Material.LearningUnit;

public interface ILearningUnitRepository : IBaseRepository<LearningUnit, LearningUnitId>
{
	Task<LearningUnit?> GetDetailByIdAsync(LearningUnitId id, CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<LearningUnit> Items, int TotalCount)> GetPagedByVideoIdAsync(
		VideoId videoId,
		KnowledgeUnitType? knowledgeType,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyDictionary<Guid, LearningUnitMediaContext>> GetMediaContextByMeaningIdsAsync(
		IReadOnlyCollection<Guid> meaningIds,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyDictionary<Guid, IReadOnlyList<LearningUnitMediaContext>>> GetMediaContextsByMeaningIdsAsync(
		IReadOnlyCollection<Guid> meaningIds,
		int maxVideosPerMeaning = 5,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyDictionary<Guid, LearningUnitMediaContext>> GetMediaContextBySegmentIdsAsync(
		IReadOnlyCollection<Guid> segmentIds,
		CancellationToken cancellationToken = default);
}
