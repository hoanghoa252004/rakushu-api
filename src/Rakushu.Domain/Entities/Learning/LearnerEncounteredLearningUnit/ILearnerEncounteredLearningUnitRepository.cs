using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;

public interface ILearnerEncounteredLearningUnitRepository
	: IBaseRepository<LearnerEncounteredLearningUnit, LearnerEncounteredLearningUnitId>
{
	Task<IReadOnlyList<LearnerEncounteredLearningUnit>> GetByUserAndLearningUnitIdsAsync(
		UserId userId,
		IReadOnlyCollection<LearningUnitId> learningUnitIds,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<LearnerEncounteredLearningUnit>> GetByUserAndVideoAsync(
		UserId userId,
		VideoId videoId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<LearnerEncounteredLearningUnit>> GetByUserWithDetailsAsync(
		UserId userId,
		CancellationToken cancellationToken = default);

	Task<int> DeleteByUserAndVideoAsync(
		UserId userId,
		VideoId videoId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<Video>> GetReadyVideosWithLearningUnitsAsync(
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<(string LevelCode, string LevelName, int SortOrder, int TotalUnitsCount)>> GetSystemVocabularyCountsByLevelAsync(
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<(Guid CategoryId, string CategoryName, IReadOnlyCollection<Guid> KnowledgeUnitIds)>> GetSystemVocabularyByCategoryAsync(
		CancellationToken cancellationToken = default);
}
