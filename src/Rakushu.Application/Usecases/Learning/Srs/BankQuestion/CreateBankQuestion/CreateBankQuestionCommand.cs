using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.CreateBankQuestion;

public sealed record CreateBankQuestionCommand(
	Guid KnowledgeMeaningId,
	Guid QuestionTypeId,
	string Prompt,
	IReadOnlyList<BankQuestionChoiceDto> Choices,
	string CorrectChoiceId,
	string? Explanation = null,
	string Difficulty = "Medium",
	bool IsActive = true) : IRequest<Result<Guid>>;
