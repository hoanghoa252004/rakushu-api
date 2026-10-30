using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GetBankQuestions;

public sealed record GetBankQuestionsQuery(
	Guid? MeaningId = null,
	Guid? QuestionTypeId = null,
	string? Difficulty = null,
	bool? IsActive = null,
	int Page = 1,
	int PageSize = 20) : IRequest<Result<IReadOnlyList<BankQuestionSummaryDto>>>;
