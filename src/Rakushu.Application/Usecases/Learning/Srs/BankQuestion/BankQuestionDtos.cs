namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion;

public sealed record QuestionAnswerDto(
	string Id,
	string Text);

public sealed record BankQuestionChoiceDto(
	string Id,
	string Text);

public sealed record BankQuestionSummaryDto(
	Guid Id,
	Guid MeaningId,
	string? Expression,
	string? Meaning,
	Guid QuestionTypeId,
	string? QuestionTypeCode,
	string Question,
	string Difficulty,
	bool IsActive,
	DateTimeOffset CreatedAt);

public sealed record BankQuestionDetailDto(
	Guid Id,
	Guid MeaningId,
	string? Expression,
	string? Meaning,
	Guid QuestionTypeId,
	string? QuestionTypeCode,
	string Question,
	IReadOnlyList<QuestionAnswerDto> Answers,
	string CorrectAnswer,
	string? Explanation,
	string Difficulty,
	bool IsActive,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt);
