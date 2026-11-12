using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.CreateBankQuestion;

public sealed record CreateBankQuestionCommand(
	Guid KnowledgeMeaningId,
	Guid QuestionTypeId,
	string Question,
	IReadOnlyList<QuestionAnswerDto>? Answers,
	string CorrectAnswer,
	string? Explanation = null,
	bool IsActive = true,
	string MediaType = "NONE",
	string? MediaUrl = null,
	string? MediaMetadata = null) : IRequest<Result<Guid>>;