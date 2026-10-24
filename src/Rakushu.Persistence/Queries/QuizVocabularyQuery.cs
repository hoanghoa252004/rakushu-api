using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Abstractions.Persistence.Queries;

namespace Rakushu.Persistence.Queries;

internal sealed class QuizVocabularyQuery : IQuizVocabularyQuery
{
	private readonly RakushuDbContext _context;

	public QuizVocabularyQuery(RakushuDbContext context)
	{
		_context = context;
	}

	public async Task<IReadOnlyList<QuizVocabularyEntry>> GetQuizVocabularyEntriesAsync(
		CancellationToken cancellationToken = default)
	{
		var entries = await _context.KnowledgeMeanings
			.AsNoTracking()
			.Where(km =>
				km.KnowledgeUnit.Expression != null &&
				km.KnowledgeUnit.Expression.Trim() != string.Empty &&
				km.Meaning != null &&
				km.Meaning.Trim() != string.Empty)
			.Select(km => new
			{
				Expression = km.KnowledgeUnit.Expression,
				Reading = km.KnowledgeUnit.Reading,
				Meaning = km.Meaning
			})
			.Distinct()
			.ToListAsync(cancellationToken);

		return entries
			.Select(entry => new QuizVocabularyEntry(entry.Expression, entry.Reading, entry.Meaning))
			.ToList();
	}
}
