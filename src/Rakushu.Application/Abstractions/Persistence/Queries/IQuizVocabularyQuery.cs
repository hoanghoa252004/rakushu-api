namespace Rakushu.Application.Abstractions.Persistence.Queries;

/// <summary>Vocabulary data projected for generating quiz questions and answer choices.</summary>
public sealed record QuizVocabularyEntry(string Expression, string? Reading, string Meaning);

public interface IQuizVocabularyQuery
{
	Task<IReadOnlyList<QuizVocabularyEntry>> GetQuizVocabularyEntriesAsync(CancellationToken cancellationToken = default);
}
