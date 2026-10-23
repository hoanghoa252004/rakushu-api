namespace Rakushu.Application.Usecases.Learning.Srs.Common;

public sealed record QuizChoiceDto(string Id, string Text);

public sealed record QuizQuestionDto(
	Guid ItemId,
	int Position,
	string QuestionType,
	string Prompt,
	string? Reading,
	IReadOnlyList<QuizChoiceDto> Choices);

public sealed record QuizQuestionResultDto(
	Guid ItemId,
	int Position,
	string QuestionType,
	string Prompt,
	string? Reading,
	string SelectedOptionId,
	string CorrectOptionId,
	bool IsCorrect,
	string Feedback,
	IReadOnlyList<QuizChoiceDto> Choices,
	DateTimeOffset? NextReviewAt = null);

public sealed record QuizAttemptStartedResponse(
	Guid AttemptId,
	string Status,
	string AttemptMode,
	DateTimeOffset StartedAt,
	IReadOnlyList<QuizQuestionDto> Questions);

public sealed record QuizAttemptResultResponse(
	Guid AttemptId,
	string Status,
	int Score,
	int TotalQuestions,
	int CorrectAnswers,
	DateTimeOffset CompletedAt,
	IReadOnlyList<QuizQuestionResultDto> Results);
