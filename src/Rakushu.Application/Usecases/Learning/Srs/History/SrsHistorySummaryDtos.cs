namespace Rakushu.Application.Usecases.Learning.Srs.History;

public sealed record FlashcardHistorySummaryDto(
	int Easy,
	int Hard,
	int Forgot);

public sealed record QuizHistorySummaryDto(
	int Correct,
	int Incorrect,
	int Unanswered);

public sealed record SrsHistorySummaryDto(
	FlashcardHistorySummaryDto Flashcard,
	QuizHistorySummaryDto Quiz);
