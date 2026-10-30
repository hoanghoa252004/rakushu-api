namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion;

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
	string Prompt,
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
	string Prompt,
	IReadOnlyList<BankQuestionChoiceDto> Choices,
	string CorrectChoiceId,
	string? Explanation,
	string Difficulty,
	bool IsActive,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt);
