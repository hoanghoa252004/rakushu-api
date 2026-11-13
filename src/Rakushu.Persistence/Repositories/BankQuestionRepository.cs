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
			.Where(bq => idList.Contains(bq.KnowledgeMeaningId) && bq.IsActive && bq.Status == QuestionStatus.Published)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<BankQuestion>> GetActiveByMeaningIdAsync(
		KnowledgeMeaningId meaningId,
		CancellationToken cancellationToken = default)
	{
		return await _context.BankQuestions
			.Include(bq => bq.QuestionType)
			.Include(bq => bq.KnowledgeMeaning)
			.Where(bq => bq.KnowledgeMeaningId == meaningId && bq.IsActive && bq.Status == QuestionStatus.Published)
			.ToListAsync(cancellationToken);
	}

	public async Task<(IReadOnlyList<BankQuestion> Items, int TotalCount)> GetQuestionsAsync(
		KnowledgeMeaningId? meaningId = null,
		Guid? questionTypeId = null,
		bool? isActive = null,
		QuestionStatus? status = null,
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
			var targetTypeId = QuestionTypeId.From(questionTypeId.Value);
			query = query.Where(bq => bq.QuestionTypeId == targetTypeId);
		}

		if (status.HasValue)
		{
			query = query.Where(bq => bq.Status == status.Value);
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

	public async Task<IReadOnlyList<string>> GetRandomDistractorMeaningsAsync(
		IEnumerable<KnowledgeMeaningId> excludeMeaningIds,
		int count,
		CancellationToken cancellationToken = default)
	{
		var excludeList = excludeMeaningIds.ToList();

		var meanings = await _context.KnowledgeMeanings
			.AsNoTracking()
			.Where(km => !excludeList.Contains(km.Id) && !string.IsNullOrWhiteSpace(km.Meaning))
			.Select(km => km.Meaning)
			.Distinct()
			.Take(count * 4)
			.ToListAsync(cancellationToken);

		if (meanings.Count < count)
		if (meanings.Count < count)
		{
			var defaultPool = new[]
			{
				"Điểm thắc mắc, nghi vấn",
				"Nội dung được thảo luận",
				"Vấn đề cần giải quyết",
				"Lịch trình, kế hoạch dự kiến",
				"Phương pháp thực hiện",
				"Kết quả kiểm tra, đánh giá",
				"Tùy trường hợp, có khi ...",
				"Nếu có điều gì xảy ra",
				"Nội dung được ghi chép",
				"Khi ..., vào dịp ..."
			};

			meanings = meanings.Concat(defaultPool.Where(p => !meanings.Contains(p))).ToList();
		}

		return meanings.OrderBy(_ => Random.Shared.Next()).Take(count).ToList();
	}

	public async Task<bool> HasBeenUsedInQuizAsync(
		BankQuestionId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.Questions
			.AnyAsync(qi => qi.BankQuestionId == id, cancellationToken);
	}

	public async Task<bool> ExistsDuplicateAsync(
		KnowledgeMeaningId meaningId,
		Guid questionTypeId,
		string questionText,
		BankQuestionId? excludeId = null,
		CancellationToken cancellationToken = default)
	{
		var typedQuestionTypeId = QuestionTypeId.From(questionTypeId);
		var trimmedText = questionText.Trim();

		var query = _context.BankQuestions
			.Where(bq =>
				bq.KnowledgeMeaningId == meaningId &&
				bq.QuestionTypeId == typedQuestionTypeId &&
				bq.Question == trimmedText &&
				bq.IsActive);

		if (excludeId != null)
		{
			query = query.Where(bq => bq.Id != excludeId);
		}

		return await query.AnyAsync(cancellationToken);
	}

	public async Task<BankQuestion?> FindDuplicateAsync(
		KnowledgeMeaningId meaningId,
		Guid questionTypeId,
		string questionText,
		CancellationToken cancellationToken = default)
	{
		var typedQuestionTypeId = QuestionTypeId.From(questionTypeId);
		var trimmedText = questionText.Trim();

		return await _context.BankQuestions
			.FirstOrDefaultAsync(bq =>
				bq.KnowledgeMeaningId == meaningId &&
				bq.QuestionTypeId == typedQuestionTypeId &&
				bq.Question == trimmedText,
				cancellationToken);
	}

	public async Task<bool> HasDraftOrPublishedQuestionAsync(
		KnowledgeMeaningId meaningId,
		CancellationToken cancellationToken = default)
	{
		return await _context.BankQuestions
			.AnyAsync(bq => bq.KnowledgeMeaningId == meaningId &&
				(bq.Status == QuestionStatus.Draft || (bq.Status == QuestionStatus.Published && bq.IsActive)),
				cancellationToken);
	}

	public async Task<QuestionTypeId?> GetQuestionTypeIdByCodeAsync(
		string code,
		CancellationToken cancellationToken = default)
	{
		var normalizedCode = code.Trim().ToUpperInvariant();
		var type = await _context.QuestionTypes
			.FirstOrDefaultAsync(qt => qt.Code == normalizedCode && qt.IsActive, cancellationToken);

		return type?.Id;
	}

	public async Task<QuestionTypeId> GetDefaultQuestionTypeIdAsync(
		CancellationToken cancellationToken = default)
	{
		var type = await _context.QuestionTypes
			.FirstOrDefaultAsync(qt => qt.IsActive, cancellationToken);

		return type?.Id ?? QuestionTypeId.Create();
	}

	public async Task<IReadOnlyList<KnowledgeMeaning>> GetMeaningsNeedingDraftQuestionsAsync(
		int limit,
		CancellationToken cancellationToken = default)
	{
		var meaningIdsWithQuestions = _context.BankQuestions
			.Select(bq => bq.KnowledgeMeaningId);

		return await _context.SavedItems
			.Where(si => si.IsActive && !meaningIdsWithQuestions.Contains(si.KnowledgeMeaningId))
			.Select(si => si.KnowledgeMeaningId)
			.Distinct()
			.Join(
				_context.KnowledgeMeanings
					.Include(km => km.KnowledgeUnit)
					.Where(km => km.IsActive),
				id => id,
				km => km.Id,
				(id, km) => km)
			.Take(limit)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlySet<Guid>> GetMeaningIdsWithPublishedQuestionsAsync(
		IEnumerable<KnowledgeMeaningId> meaningIds,
		CancellationToken cancellationToken = default)
	{
		var idList = meaningIds.Distinct().ToList();
		if (idList.Count == 0)
			return new HashSet<Guid>();

		var result = await _context.BankQuestions
			.Where(bq => idList.Contains(bq.KnowledgeMeaningId)
				&& bq.IsActive
				&& bq.Status == QuestionStatus.Published)
			.Select(bq => bq.KnowledgeMeaningId)
			.Distinct()
			.ToListAsync(cancellationToken);

		return result.Select(id => id.Value).ToHashSet();
	}
}