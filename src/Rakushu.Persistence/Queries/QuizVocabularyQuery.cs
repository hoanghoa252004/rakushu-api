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
		// Filter mapped entity properties before projecting to the record; EF Core cannot translate
		// a Where over a constructed record.
		var entries = await _context.LearningUnits
			.AsNoTracking()
			.Where(unit =>
				unit.KnowledgeMeaning.KnowledgeUnit.Expression != null &&
				unit.KnowledgeMeaning.KnowledgeUnit.Expression.Trim() != string.Empty &&
				unit.KnowledgeMeaning.Meaning != null &&
				unit.KnowledgeMeaning.Meaning.Trim() != string.Empty)
			.Select(unit => new
			{
				Expression = unit.KnowledgeMeaning.KnowledgeUnit.Expression,
				Reading = unit.KnowledgeMeaning.KnowledgeUnit.Reading,
				Meaning = unit.KnowledgeMeaning.Meaning
			})
			.Distinct()
			.ToListAsync(cancellationToken);

		return entries
			.Select(entry => new QuizVocabularyEntry(entry.Expression, entry.Reading, entry.Meaning))
			.ToList();
	}
}
