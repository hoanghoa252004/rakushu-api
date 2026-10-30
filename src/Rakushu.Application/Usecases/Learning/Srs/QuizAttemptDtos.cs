namespace Rakushu.Application.Usecases.Learning.Srs;

public sealed record QuizOptionDto(string Id, string Text);

public sealed record QuizQuestionDto(
	Guid Id,
	int Position,
	string Prompt,
	string? Reading,
	IReadOnlyList<QuizOptionDto> Choices);

public sealed record QuizAttemptStartedResponse(
	Guid AttemptId,
	Guid QuizId,
	string Mode,
	int TotalQuestions,
	DateTimeOffset StartedAt,
	DateTimeOffset ExpiresAt,
	IReadOnlyList<QuizQuestionDto> Questions);

public sealed record QuizAnswerResultDto(
	Guid QuestionId,
	int Position,
	string Prompt,
	string? Reading,
	IReadOnlyList<QuizOptionDto> Choices,
	string? SelectedOptionId,
	string CorrectOptionId,
	bool IsCorrect,
	int ElapsedMs,
	int Quality,
	string? Feedback,
	string? Explanation);

public sealed record QuizAttemptResultResponse(
	Guid AttemptId,
	Guid QuizId,
	string Mode,
	string Status,
	decimal Score,
	int TotalItems,
	int AnsweredItems,
	int CorrectItems,
	DateTimeOffset StartedAt,
	DateTimeOffset? CompletedAt,
	IReadOnlyList<QuizAnswerResultDto> Answers);
