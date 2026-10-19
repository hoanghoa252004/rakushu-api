using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Infrastructure;

public interface IQuizContentGenerator
{
	Task<IReadOnlyList<GeneratedQuizQuestion>> GenerateQuestionsAsync(
		string questionFormat,
		IReadOnlyList<QuizGenerationCard> cards,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyDictionary<Guid, string>> GenerateAdaptiveFeedbackAsync(
		IReadOnlyList<QuizFeedbackPrompt> questions,
		CancellationToken cancellationToken = default);
}

public sealed record QuizGenerationCard(
	Guid CardId,
	string Expression,
	string? Reading,
	string Meaning,
	string QuestionDirection = "JapaneseToVietnamese");

public sealed record GeneratedQuizQuestion(
	Guid CardId,
	string Prompt);

public sealed record QuizFeedbackPrompt(
	Guid ItemId,
	bool IsCorrect,
	bool HasPreviousAttempt);
