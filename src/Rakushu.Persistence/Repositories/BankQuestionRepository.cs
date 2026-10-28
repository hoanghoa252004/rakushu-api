using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Persistence.Repositories;

public sealed class BankQuestionRepository : BaseRepository<BankQuestion, BankQuestionId>, IBankQuestionRepository
{
	public BankQuestionRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<IReadOnlyList<BankQuestion>> GetByMeaningIdsAsync(
		IEnumerable<KnowledgeMeaningId> meaningIds,
		CancellationToken cancellationToken = default)
	{
		var idList = meaningIds.ToList();
		return await _context.BankQuestions
			.Include(bq => bq.QuestionType)
			.Include(bq => bq.KnowledgeMeaning)
			.Where(bq => idList.Contains(bq.KnowledgeMeaningId) && bq.IsActive)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<BankQuestion>> GetActiveByMeaningIdAsync(
		KnowledgeMeaningId meaningId,
		CancellationToken cancellationToken = default)
	{
		return await _context.BankQuestions
			.Include(bq => bq.QuestionType)
			.Include(bq => bq.KnowledgeMeaning)
			.Where(bq => bq.KnowledgeMeaningId == meaningId && bq.IsActive)
			.ToListAsync(cancellationToken);
	}
}
