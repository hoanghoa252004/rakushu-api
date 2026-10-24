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
}
