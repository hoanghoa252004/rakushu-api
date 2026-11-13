namespace Rakushu.Application.Usecases.Learning.Srs;

public static class QuizAnswerHelper
{
	public const string Skip = "SKIP";

	public static bool IsSkip(string? answer)
	{
		if (string.IsNullOrWhiteSpace(answer))
			return false;

		return string.Equals(answer.Trim(), Skip, StringComparison.OrdinalIgnoreCase);
	}
}

public sealed record QuizOptionDto(string Id, string Text);

public sealed record QuizQuestionDto(
	Guid ItemId,
	int Sequence,
	string Question,
	IReadOnlyList<QuizOptionDto> Choices,
	string? SelectedAnswer,
	int TimeSpentMs,
	string? QuestionType = null,
	string? CorrectAnswer = null,
	bool? IsCorrect = null,
	int? Quality = null,
	string? Feedback = null,
	string? Explanation = null,
	string? MediaType = null,
	string? MediaUrl = null,
	QuestionMediaMetadataDto? MediaMetadata = null);

public sealed record QuizAttemptDetailDto(
	Guid Id,
	string Mode,
	string Status,
	decimal? Score,
	int TotalItems,
	int AnsweredItems,
	int? CorrectItems,
	DateTimeOffset CreatedAt,
	DateTimeOffset ExpiresAt,
	DateTimeOffset? CompletedAt,
	bool IsEvaluated,
	IReadOnlyList<QuizQuestionDto> Questions);

public sealed record QuizAnswerResultDto(
	Guid ItemId,
	int Sequence,
	string Question,
	IReadOnlyList<QuizOptionDto> Choices,
	string? SelectedAnswer,
	string CorrectAnswer,
	bool IsCorrect,
	int TimeSpentMs,
	int Quality,
	string? Feedback,
	string? Explanation,
	string? QuestionType = null,
	string? MediaType = null,
	string? MediaUrl = null,
	QuestionMediaMetadataDto? MediaMetadata = null);

public sealed record QuizAttemptResultDto(
	Guid Id,
	Guid QuizId,
	string Mode,
	string Status,
	decimal Score,
	int TotalItems,
	int AnsweredItems,
	int CorrectItems,
	DateTimeOffset CreatedAt,
	DateTimeOffset? CompletedAt,
	bool IsEvaluated,
	IReadOnlyList<QuizAnswerResultDto> Answers);

public sealed record QuizAttemptSummaryDto(
	Guid Id,
	string Mode,
	string Status,
	decimal? Score,
	int TotalItems,
	int AnsweredItems,
	int? CorrectItems,
	DateTimeOffset CreatedAt,
	DateTimeOffset ExpiresAt,
	DateTimeOffset? CompletedAt,
	bool IsEvaluated);
