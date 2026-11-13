using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GetBankQuestions;

public sealed record GetBankQuestionsQuery(
	Guid? MeaningId = null,
	Guid? QuestionTypeId = null,
	bool? IsActive = null,
	QuestionStatus? Status = null,
	int Page = 1,
	int PageSize = 20) : IRequest<Result<IReadOnlyList<BankQuestionSummaryDto>>>;
