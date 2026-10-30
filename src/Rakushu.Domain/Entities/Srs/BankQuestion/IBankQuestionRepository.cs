using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;

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
		string? difficulty = null,
		bool? isActive = null,
		int page = 1,
		int pageSize = 20,
		CancellationToken cancellationToken = default);
}
