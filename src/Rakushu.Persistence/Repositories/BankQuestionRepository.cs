using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.BankQuestion;
using Rakushu.Domain.Entities.Srs.QuestionType;

namespace Rakushu.Persistence.Repositories;

public sealed class BankQuestionRepository : BaseRepository<BankQuestion, BankQuestionId>, IBankQuestionRepository
{
	public BankQuestionRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<BankQuestion?> GetByIdWithDetailsAsync(
		BankQuestionId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.BankQuestions
			.Include(bq => bq.QuestionType)
			.Include(bq => bq.KnowledgeMeaning)
				.ThenInclude(km => km.KnowledgeUnit)
			.FirstOrDefaultAsync(bq => bq.Id == id, cancellationToken);
	}

	public async Task<bool> QuestionTypeExistsAsync(
		Guid questionTypeId,
		CancellationToken cancellationToken = default)
	{
		var id = QuestionTypeId.From(questionTypeId);
		return await _context.QuestionTypes
			.AnyAsync(qt => qt.Id == id, cancellationToken);
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

	public async Task<(IReadOnlyList<BankQuestion> Items, int TotalCount)> GetQuestionsAsync(
		KnowledgeMeaningId? meaningId = null,
		Guid? questionTypeId = null,
		string? difficulty = null,
		bool? isActive = null,
		int page = 1,
		int pageSize = 20,
		CancellationToken cancellationToken = default)
	{
		var query = _context.BankQuestions
			.Include(bq => bq.QuestionType)
			.Include(bq => bq.KnowledgeMeaning)
				.ThenInclude(km => km.KnowledgeUnit)
			.AsQueryable();

		if (meaningId is not null)
		{
			query = query.Where(bq => bq.KnowledgeMeaningId == meaningId);
		}

		if (questionTypeId.HasValue)
		{
			query = query.Where(bq => bq.QuestionTypeId.Value == questionTypeId.Value);
		}

		if (!string.IsNullOrWhiteSpace(difficulty))
		{
			query = query.Where(bq => bq.Difficulty.ToLower() == difficulty.ToLower());
		}

		if (isActive.HasValue)
		{
			query = query.Where(bq => bq.IsActive == isActive.Value);
		}

		int totalCount = await query.CountAsync(cancellationToken);

		var items = await query
			.OrderByDescending(bq => bq.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, totalCount);
	}
}
