using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;

namespace Rakushu.Domain.Entities.Srs.BankQuestion;

public interface IBankQuestionRepository : IBaseRepository<BankQuestion, BankQuestionId>
{
	Task<IReadOnlyList<BankQuestion>> GetByMeaningIdsAsync(
		IEnumerable<KnowledgeMeaningId> meaningIds,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<BankQuestion>> GetActiveByMeaningIdAsync(
		KnowledgeMeaningId meaningId,
		CancellationToken cancellationToken = default);
}
