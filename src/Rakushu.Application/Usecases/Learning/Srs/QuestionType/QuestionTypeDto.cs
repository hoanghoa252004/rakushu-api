namespace Rakushu.Application.Usecases.Learning.Srs.QuestionType;

public sealed record QuestionTypeDto(
	Guid Id,
	string Code,
	string Name,
	string? Description,
	int ExpectedDurationSeconds,
	bool IsActive);
