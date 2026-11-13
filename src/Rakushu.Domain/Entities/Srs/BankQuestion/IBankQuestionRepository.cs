using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.QuestionType;

namespace Rakushu.Domain.Entities.Srs.BankQuestion;

public interface IBankQuestionRepository : IBaseRepository<BankQuestion, BankQuestionId>
{
	Task<BankQuestion?> GetByIdWithDetailsAsync(
		BankQuestionId id,
		CancellationToken cancellationToken = default);

	Task<bool> QuestionTypeExistsAsync(
		Guid questionTypeId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<BankQuestion>> GetByMeaningIdsAsync(
		IEnumerable<KnowledgeMeaningId> meaningIds,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<BankQuestion>> GetActiveByMeaningIdAsync(
		KnowledgeMeaningId meaningId,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<BankQuestion> Items, int TotalCount)> GetQuestionsAsync(
		KnowledgeMeaningId? meaningId = null,
		Guid? questionTypeId = null,
		bool? isActive = null,
		QuestionStatus? status = null,
		int page = 1,
		int pageSize = 20,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<string>> GetRandomDistractorMeaningsAsync(
		IEnumerable<KnowledgeMeaningId> excludeMeaningIds,
		int count,
		CancellationToken cancellationToken = default);

	Task<bool> HasBeenUsedInQuizAsync(
		BankQuestionId id,
		CancellationToken cancellationToken = default);

	Task<bool> ExistsDuplicateAsync(
		KnowledgeMeaningId meaningId,
		Guid questionTypeId,
		string questionText,
		BankQuestionId? excludeId = null,
		CancellationToken cancellationToken = default);

	Task<BankQuestion?> FindDuplicateAsync(
		KnowledgeMeaningId meaningId,
		Guid questionTypeId,
		string questionText,
		CancellationToken cancellationToken = default);

	Task<bool> HasDraftOrPublishedQuestionAsync(
		KnowledgeMeaningId meaningId,
		CancellationToken cancellationToken = default);

	Task<QuestionTypeId?> GetQuestionTypeIdByCodeAsync(
		string code,
		CancellationToken cancellationToken = default);

	Task<QuestionTypeId> GetDefaultQuestionTypeIdAsync(
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<KnowledgeMeaning>> GetMeaningsNeedingDraftQuestionsAsync(
		int limit,
		CancellationToken cancellationToken = default);

	Task<IReadOnlySet<Guid>> GetMeaningIdsWithPublishedQuestionsAsync(
		IEnumerable<KnowledgeMeaningId> meaningIds,
		CancellationToken cancellationToken = default);
}
