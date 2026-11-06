using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.UpdateBankQuestion;

public sealed record UpdateBankQuestionCommand(
	Guid Id,
	string Question,
	IReadOnlyList<QuestionAnswerDto> Answers,
	string CorrectAnswer,
	string? Explanation = null,
	string Difficulty = "Medium") : IRequest<Result>;
