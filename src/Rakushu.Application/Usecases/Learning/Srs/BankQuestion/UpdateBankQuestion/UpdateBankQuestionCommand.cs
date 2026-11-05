using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.UpdateBankQuestion;

public sealed record UpdateBankQuestionCommand(
	Guid Id,
	string Prompt,
	IReadOnlyList<BankQuestionChoiceDto> Choices,
	string CorrectChoiceId,
	string? Explanation = null,
	string Difficulty = "Medium") : IRequest<Result>;
