namespace Rakushu.Application.Abstractions.Infrastructure.AiService;

public interface IQuizContentGenerator
{
	Task<IReadOnlyDictionary<Guid, string>> GenerateAdaptiveFeedbackAsync(
		IReadOnlyList<QuizFeedbackPrompt> questions,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyDictionary<Guid, GeneratedQuizPromptAndExplanation>> GenerateQuizContentBatchAsync(
		IReadOnlyList<GenerateQuizContentPrompt> prompts,
		CancellationToken cancellationToken = default);
}

public sealed record GenerateQuizContentPrompt(
	Guid MeaningId,
	string Expression,
	string? Reading,
	string Meaning,
	string? ContextSentence = null
);

public sealed record GeneratedQuizPromptAndExplanation(
	Guid MeaningId,
	string Prompt,
	string Explanation
);

public sealed record QuizFeedbackPrompt(
	Guid QuestionId,
	Guid SavedItemId,
	bool IsCorrect,
	int PreviousAttemptCount,
	int PreviousIncorrectCount,
	IReadOnlyList<QuizFeedbackHistoryEntry> PreviousFeedbackHistory,
	string? QuestionPrompt = null,
	string? Reading = null,
	string? SelectedAnswer = null,
	string? CorrectAnswer = null,
	string? Explanation = null)
{
	public string LearningContext => !IsCorrect
		? PreviousIncorrectCount >= 2
			? "RepeatedIncorrect"
			: PreviousIncorrectCount == 1
				? "IncorrectAgain"
				: PreviousAttemptCount > 0
					? "IncorrectAfterPriorCorrect"
					: "FirstIncorrect"
		: PreviousIncorrectCount > 0
			? "CorrectAfterPriorMistakes"
			: PreviousAttemptCount > 0
				? "CorrectAgain"
				: "FirstCorrect";
}

public sealed record QuizFeedbackHistoryEntry(
	bool IsCorrect,
	string SelectedAnswer,
	string CorrectAnswer,
	string FeedbackGiven,
	DateTimeOffset AnsweredAt);
